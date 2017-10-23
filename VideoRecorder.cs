using Emgu.CV;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public class VideoRecorder
    {
        // frame per second of the video
        public int fps; 

        // size of the color image
        public int colorWidth = 1920;
        public int ColorHeight = 1080;

        // scale factor defined in the opening prompt
        public int scaleFactor = 1;

        // number of frames currently recorded and current second
        public int videoFrameCounter = 0;
        public int currentSecFrameCounter = 0;
        public int currentMilliSec = 0;
        public int lastSecond = -1;
        public long lastFrameSavedAt = 0;

        // references to object instances
        public Logger logger;
        public VideoWriter videowriter = null;
        public Stopwatch stopwatch = null;

        // constructor
        public VideoRecorder(Logger logger)
        {
            // saves information
            this.logger = logger;
            this.stopwatch = new Stopwatch();
        }

        // creates the video writer
        public void createVideoFrameWriter(int fps)
        {
            // create the videowriter if it doesn't exist yet
            if (videowriter == null)
            {
                // resize the image if necessary
                this.fps = fps;
                int width = colorWidth / scaleFactor;
                int height = ColorHeight / scaleFactor;
                Size frameSize = new Size(width, height);

                String videoFilename = string.Format(@"{0}-Kinect-video-{1}.avi", logger.session, Helpers.getTimestamp("filename"));

                videoFilename = Path.Combine(logger.destination, videoFilename);

                videowriter = new VideoWriter(videoFilename, //File name
                                        -1, //Video format -1 opens a dialogue window
                                        fps, //FPS
                                        frameSize, //frame size
                                        true); //Color

                // update the time counter
                this.stopwatch.Start();
            }
        }

        /// <summary>
        /// write a video frame to the file
        /// </summary>
        public void WriteFrameToVideo(WriteableBitmap colorBitmap, ColorFrame frame)
        {
            // if we are not recording a video, we don't do anything
            if (logger.openingPrompt.videoNo.Checked) return;

            // keep track of the last second being recorded
            if (lastSecond < 0) lastSecond = Int32.Parse(Helpers.getTimestamp("second"));

            // if we haven't waited enough time, we skip this frame
            if (this.stopwatch.ElapsedMilliseconds - lastFrameSavedAt < 1000.0 / fps-5) return;
            else lastFrameSavedAt = this.stopwatch.ElapsedMilliseconds;

            // convert the color frame into an EMGU mat
            Image<Emgu.CV.Structure.Bgra, byte> img = Helpers.ToImage(frame);

            // resize the image if necessary
            if(scaleFactor > 1)
            {
                int width = colorBitmap.PixelWidth / scaleFactor;
                int height = colorBitmap.PixelHeight / scaleFactor;
                Size frameSize = new Size(width, height);
                img = img.Resize(width, height, Emgu.CV.CvEnum.Inter.Linear);
            }

            if (currentSecFrameCounter < fps)
            {
                videowriter.Write(img.Mat);
                videoFrameCounter += 1;
                currentSecFrameCounter += 1;
            }

            if (lastSecond != Int32.Parse(Helpers.getTimestamp("second")))
            {
                // if we don't have enough frames for the current second, we repeat some
                while (currentSecFrameCounter < fps)
                {
                    videowriter.Write(img.Mat);
                    videoFrameCounter += 1;
                    currentSecFrameCounter += 1;
                    Console.WriteLine("adding frame");
                }

                // reset the stopwatch
                this.stopwatch.Reset();
                this.stopwatch.Start();
                lastFrameSavedAt = 0;
                currentSecFrameCounter = 0;
                lastSecond = Int32.Parse(Helpers.getTimestamp("second"));
            }

            Console.WriteLine("Number of video frames: " + videoFrameCounter + "  stopwatch:" + this.stopwatch.ElapsedMilliseconds);
        }
    }
}
