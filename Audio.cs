using Microsoft.Kinect;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace Microsoft.Samples.Kinect.BodyBasics
{
    public class Audio
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// reference to the logger
        /// </summary>
        private Logger logger;

        /// <summary>
        /// Active audio source
        /// </summary>
        private AudioSource audioSource = null;

        /// <summary>
        /// Will be allocated a buffer to hold a single sub frame of audio data read from audio stream.
        /// </summary>
        private readonly byte[] audioBuffer = null;

        /// <summary>
        /// Reader for audio frames
        /// </summary>
        public AudioBeamFrameReader reader = null;

        /// <summary>
        /// Last observed audio beam angle in radians, in the range [-pi/2, +pi/2]
        /// </summary>
        public float beamAngle = 0;
        
        /// <summary>
        /// saves the volume of the current audio frame
        /// </summary>
        public double volume = -1;
        
        /// <summary>
        /// Number of samples captured from Kinect audio stream each millisecond.
        /// </summary>
        private const int SamplesPerMillisecond = 16;

        /// <summary>
        /// Number of bytes in each Kinect audio stream sample (32-bit IEEE float).
        /// </summary>
        private const int BytesPerSample = 4;// sizeof(float);

        /// <summary>
        /// Number of audio samples represented by each column of pixels in wave bitmap.
        /// </summary>
        private const int SamplesPerColumn = 40;

        /// <summary>
        /// Minimum energy of audio to display (a negative number in dB value, where 0 dB is full scale)
        /// </summary>
        private const int MinEnergy = -90;

        /// <summary>
        /// Width of bitmap that stores audio stream energy data ready for visualization.
        /// </summary>
        private const int EnergyBitmapWidth = 780;

        /// <summary>
        /// Height of bitmap that stores audio stream energy data ready for visualization.
        /// </summary>
        private const int EnergyBitmapHeight = 195;

        /// <summary>
        /// Buffer used to store audio stream energy data as we read audio.
        /// We store 25% more energy values than we strictly need for visualization to allow for a smoother
        /// stream animation effect, since rendering happens on a different schedule with respect to audio
        /// capture.
        /// </summary>
        private readonly float[] energy = new float[(uint)(EnergyBitmapWidth * 1.25)];

        /// <summary>
        /// Object for locking energy buffer to synchronize threads.
        /// </summary>
        private readonly object energyLock = new object();

        /// <summary>
        /// Last observed audio beam angle confidence, in the range [0, 1]
        /// </summary>
        private float beamAngleConfidence = 0;

        /// <summary>
        /// Sum of squares of audio samples being accumulated to compute the next energy value.
        /// </summary>
        private float accumulatedSquareSum;

        /// <summary>
        /// Number of audio samples accumulated so far to compute the next energy value.
        /// </summary>
        private int accumulatedSampleCount;

        /// <summary>
        /// Index of next element available in audio energy buffer.
        /// </summary>
        private int energyIndex;

        /// <summary>
        /// Number of newly calculated audio stream energy values that have not yet been
        /// displayed.
        /// </summary>
        private int newEnergyAvailable;

        /// <summary>
        /// Error between time slice we wanted to display and time slice that we ended up
        /// displaying, given that we have to display in integer pixels.
        /// </summary>
        private float energyError;

        /// <summary>
        /// Last time energy visualization was rendered to screen.
        /// </summary>
        private DateTime? lastEnergyRefreshTime;

        /// <summary>
        /// Index of first energy element that has never (yet) been displayed to screen.
        /// </summary>
        private int energyRefreshIndex;
        
        /// <summary>
        /// public reference to the body IDs speaking in the current frame
        /// </summary>
        public List<ulong> trackingIDSpeaking = new List<ulong>();
        
        /// <summary>
        /// audio source and files for saving audio
        /// </summary>
        public WaveIn kinectSource = null;
        public WaveIn builtinSource = null;
        public WaveFileWriter kinectWav = null;
        public WaveFileWriter builtinWav = null;

        // dictionary keeping track of how much people have talked
        public Dictionary<ulong, int> talking_frames = new Dictionary<ulong, int>();


        public Audio(KinectSensor kinectSensor, Logger logger)
        {
            // save a reference to the kinect sensor object
            this.kinectSensor = kinectSensor;

            // save a reference to the logger
            this.logger = logger;

            if (this.kinectSensor != null)
            {
                // Get its audio source
                this.audioSource = this.kinectSensor.AudioSource;

                // Allocate 1024 bytes to hold a single audio sub frame. Duration sub frame 
                // is 16 msec, the sample rate is 16khz, which means 256 samples per sub frame. 
                // With 4 bytes per sample, that gives us 1024 bytes.
                this.audioBuffer = new byte[audioSource.SubFrameLengthInBytes];

                // Open the reader for the audio frames
                this.reader = audioSource.OpenReader();

                // Uncomment these two lines to overwrite the automatic mode of the audio beam.
                // It will change the beam mode to manual and set the desired beam angle.
                // In this example, point it straight forward.
                // Note that setting beam mode and beam angle will only work if the
                // application window is in the foreground.
                // Furthermore, setting these values is an asynchronous operation --
                // it may take a short period of time for the beam to adjust.
                /*
                audioSource.AudioBeams[0].AudioBeamMode = AudioBeamMode.Manual;
                audioSource.AudioBeams[0].BeamAngle = 0;
                */
            }

            // defining the wav header
            if(logger.log_audio)
                initializeAudio();
        }

        void initializeAudio()
        {
            int waveInDevices = WaveIn.DeviceCount;

            // iterate through all the recording devices
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                //Console.WriteLine("Device {0}: {1}, {2} channels", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);

                // if we find a kinect recording device, we use it to record sound
                if(deviceInfo.ProductName.Contains("Xbox") || 
                   deviceInfo.ProductName.Contains("Kinect") ||
                   deviceInfo.ProductName.Contains("NUI"))
                {
                    kinectSource = new WaveIn();
                    kinectSource.DeviceNumber = waveInDevice;
                    kinectSource.WaveFormat = new WaveFormat(44100, 1);
                    kinectSource.DataAvailable += new EventHandler<WaveInEventArgs>(kinectSource_DataAvailable);
                    kinectSource.RecordingStopped += new EventHandler<StoppedEventArgs>(kinectSource_RecordingStopped);
                    kinectSource.StartRecording();
                }
                else // record through the builtin microphone
                {
                    builtinSource = new WaveIn();
                    builtinSource.DeviceNumber = waveInDevice;
                    builtinSource.WaveFormat = new WaveFormat(44100, 1);
                    builtinSource.DataAvailable += new EventHandler<WaveInEventArgs>(waveSource_DataAvailable);
                    builtinSource.RecordingStopped += new EventHandler<StoppedEventArgs>(waveSource_RecordingStopped);
                    builtinSource.StartRecording();
                }
            }

        }

        void initializeFile(string source)
        {
            if(source == "kinect")
            {
                String videoFilename = string.Format(@"{0}-Kinect-audio-{1}.wav", logger.session, Helpers.getTimestamp("filename"));
                kinectWav = new WaveFileWriter(logger.destination + "\\" + videoFilename, kinectSource.WaveFormat);
            }
            if(source == "builtin")
            {
                String videoFilename = string.Format(@"{0}-Builtin-audio-{1}.wav", logger.session, Helpers.getTimestamp("filename"));
                builtinWav = new WaveFileWriter(logger.destination + "\\" + videoFilename, builtinSource.WaveFormat);
            }
        }

        void kinectSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (logger.recording)
            {
                if (kinectWav == null) initializeFile("kinect");
                kinectWav.Write(e.Buffer, 0, e.BytesRecorded);
                kinectWav.Flush();
            }
        }

        void waveSource_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (logger.recording)
            {
                if (builtinWav == null) initializeFile("builtin");
                builtinWav.Write(e.Buffer, 0, e.BytesRecorded);
                builtinWav.Flush();
            }
        }

        void kinectSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (kinectSource != null)
            {
                kinectSource.Dispose();
                kinectSource = null;
            }

            if (kinectWav != null)
            {
                kinectWav.Dispose();
                kinectWav = null;
            }
        }

        void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (builtinSource != null)
            {
                builtinSource.Dispose();
                builtinSource = null;
            }

            if (builtinWav != null)
            {
                builtinWav.Dispose();
                builtinWav = null;
            }
        }

        /// <summary>
        /// Helper to make it easier to check if a person is talking
        /// </summary>
        public Boolean is_bodyID_talking(ulong ID)
        {
            return this.trackingIDSpeaking.Contains(ID);
        }


        /// <summary>
        /// Handles the audio frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void Reader_FrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
            // clear up the list of people talking
            this.trackingIDSpeaking.Clear();

            // acquire the frames
            AudioBeamFrameReference frameReference = e.FrameReference;
            AudioBeamFrameList frameList = frameReference.AcquireBeamFrames();

            if (frameList != null)
            {
                // AudioBeamFrameList is IDisposable
                using (frameList)
                {

                    // Only one audio beam is supported. Get the sub frame list for this beam
                    IReadOnlyList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;

                    // Loop over all sub frames, extract audio buffer and beam information
                    foreach (AudioBeamSubFrame subFrame in subFrameList)
                    {
                        if (subFrame.BeamAngle != this.beamAngle)
                        {
                            // Audio angle
                            this.beamAngle = subFrame.BeamAngle;
                            float beamAngleInDeg = this.beamAngle * 180.0f / (float)Math.PI;

                            // Audio volume
                            long totalSquare = 0;
                            for (int i = 0; i < audioBuffer.Length; i += 2)
                            {
                                short sample = (short)(audioBuffer[i] | (audioBuffer[i + 1] << 8));
                                totalSquare += sample * sample;
                            }
                            long meanSquare = 2 * totalSquare / audioBuffer.Length;
                            double rms = Math.Sqrt(meanSquare);
                            volume = rms / 32768.0;


                            // Body tracking ID of the person speaking
                            foreach (AudioBodyCorrelation abc in subFrame.AudioBodyCorrelations)
                            {
                                // list of people speaking
                                this.trackingIDSpeaking.Add(abc.BodyTrackingId);

                                // number of frames being spoken
                                if (!this.talking_frames.ContainsKey(abc.BodyTrackingId))
                                    this.talking_frames.Add(abc.BodyTrackingId, 0);
                                this.talking_frames[abc.BodyTrackingId] += 1;
                            }
                        }


                        if (subFrame.BeamAngleConfidence != this.beamAngleConfidence)
                        {
                            this.beamAngleConfidence = subFrame.BeamAngleConfidence;
                        }
                        

                        // saves the audio frame to a temporary array
                        subFrame.CopyFrameDataToArray(this.audioBuffer);
                        
                        for (int i = 0; i < this.audioBuffer.Length; i += BytesPerSample)
                        {
                            // Extract the 32-bit IEEE float sample from the byte array
                            float audioSample = BitConverter.ToSingle(this.audioBuffer, i);

                            this.accumulatedSquareSum += audioSample * audioSample;
                            ++this.accumulatedSampleCount;

                            if (this.accumulatedSampleCount < SamplesPerColumn)
                            {
                                continue;
                            }

                            float meanSquare = this.accumulatedSquareSum / SamplesPerColumn;

                            if (meanSquare > 1.0f)
                            {
                                // A loud audio source right next to the sensor may result in mean square values
                                // greater than 1.0. Cap it at 1.0f for display purposes.
                                meanSquare = 1.0f;
                            }

                            // Calculate energy in dB, in the range [MinEnergy, 0], where MinEnergy < 0
                            float energy = MinEnergy;

                            if (meanSquare > 0)
                            {
                                energy = (float)(10.0 * Math.Log10(meanSquare));
                            }

                            lock (this.energyLock)
                            {
                                // Normalize values to the range [0, 1] for display
                                this.energy[this.energyIndex] = (MinEnergy - energy) / MinEnergy;
                                this.energyIndex = (this.energyIndex + 1) % this.energy.Length;
                                ++this.newEnergyAvailable;
                            }

                            this.accumulatedSquareSum = 0;
                            this.accumulatedSampleCount = 0;
                        } 
                    }

                }
            }
        }
        
        
        /// <summary>
        /// Handles rendering energy visualization into a bitmap.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void UpdateEnergy(object sender, EventArgs e)
        {
            lock (this.energyLock)
            {
                // Calculate how many energy samples we need to advance since the last update in order to
                // have a smooth animation effect
                DateTime now = DateTime.UtcNow;
                DateTime? previousRefreshTime = this.lastEnergyRefreshTime;
                this.lastEnergyRefreshTime = now;

                // No need to refresh if there is no new energy available to render
                if (this.newEnergyAvailable <= 0)
                {
                    return;
                }

                if (previousRefreshTime != null)
                {
                    float energyToAdvance = this.energyError + (((float)(now - previousRefreshTime.Value).TotalMilliseconds * SamplesPerMillisecond) / SamplesPerColumn);
                    int energySamplesToAdvance = Math.Min(this.newEnergyAvailable, (int)Math.Round(energyToAdvance));
                    this.energyError = energyToAdvance - energySamplesToAdvance;
                    this.energyRefreshIndex = (this.energyRefreshIndex + energySamplesToAdvance) % this.energy.Length;
                    this.newEnergyAvailable -= energySamplesToAdvance;
                }

            }
        }


        public void close()
        {
            if (this.reader != null)
            {
                // AudioBeamFrameReader is IDisposable
                this.reader.Dispose();
                this.reader = null;
            }
        }
    }
}