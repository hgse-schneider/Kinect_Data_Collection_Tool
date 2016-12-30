using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class Audio
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Active Kinect sensor
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
        private float beamAngle = 0;


        public Audio(KinectSensor kinectSensor)
        {
            // save a reference to the kinect sensor object
            this.kinectSensor = kinectSensor;

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
            }
        }

        /// <summary>
        /// Handles the audio frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        public void Reader_AudioFrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
            AudioBeamFrameReference frameReference = e.FrameReference;
            AudioBeamFrameList frameList = frameReference.AcquireBeamFrames();

            if (frameList != null)
            {
                // Only one audio beam is supported. Get the sub frame list for this beam
                IReadOnlyList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;

                // Loop over all sub frames, extract audio buffer and beam information
                foreach (AudioBeamSubFrame subFrame in subFrameList)
                {
                    // Each time the noise angle change, obtain the new location and volume
                    if (subFrame.BeamAngle != this.beamAngle)
                    {
                        // Audio angle
                        this.beamAngle = subFrame.BeamAngle;
                        float beamAngleInDeg = this.beamAngle * 180.0f / (float)Math.PI;

                        /*
                        // Audio volume
                        long totalSquare = 0;
                        for (int i = 0; i < audioBuffer.Length; i += 2)
                        {
                            short sample = (short)(audioBuffer[i] | (audioBuffer[i + 1] << 8));
                            totalSquare += sample * sample;
                        }
                        long meanSquare = 2 * totalSquare / audioBuffer.Length;
                        double rms = Math.Sqrt(meanSquare);
                        double volume = rms / 32768.0;
                        */

                        // Body tracking ID of the person speaking
                        int numPerson = 0;
                        foreach (AudioBodyCorrelation abc in subFrame.AudioBodyCorrelations)
                        {
                            numPerson++;
                            ulong bodyId = abc.BodyTrackingId;
                            //audioOutput += (", person= " + numPerson + ", bodyid=" + bodyId);
                        }
                    }
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