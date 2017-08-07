using Emgu.CV;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
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
        // size of the color image
        public int colorWidth = 1920;
        public int ColorHeight = 1080;

        // scale factor defined in the opening prompt
        public int scaleFactor = 1;

        // number of frames currently recorded and current second
        public int videoFrameCounter = 0;
        public int currentSec = DateTime.Now.Second; 

        // references to object instances
        public Logger logger;
        public VideoWriter videowriter = null;

        public VideoRecorder(Logger logger)
        {
            // save information
            this.logger = logger;
        }

        public void createVideoFrameWriter()
        {
            // create the videowriter if it doesn't exist yet
            if (videowriter == null)
            {
                // resize the image if necessary
                int width = colorWidth / scaleFactor;
                int height = ColorHeight / scaleFactor;
                Size frameSize = new Size(width, height);

                String videoFilename = string.Format(@"{0}-Kinect-video-{1}.avi", logger.session, Helpers.getTimestamp("filename"));

                videoFilename = Path.Combine(logger.destination, videoFilename);

                videowriter = new VideoWriter(videoFilename, //File name
                                        -1, //Video format -1 opens a dialogue window
                                        15, //FPS
                                        frameSize, //frame size
                                        true); //Color

                // update the time counter
                currentSec = DateTime.Now.Second;
            }
        }

        /// <summary>
        /// write a video frame to the file
        /// </summary>
        public void WriteFrameToVideo(WriteableBitmap colorBitmap, ColorFrame frame)
        {
            if (logger.openingPrompt.videoNo.Checked) return;

            if (videowriter == null) createVideoFrameWriter();

            // resize the image if necessary
            int width = colorBitmap.PixelWidth / scaleFactor;
            int height = colorBitmap.PixelHeight / scaleFactor;
            Size frameSize = new Size(width, height);

            // convert the color frame into an EMGU mat and resize it
            Image<Emgu.CV.Structure.Bgra, byte> img = Helpers.ToImage(frame);
            Image<Emgu.CV.Structure.Bgra, byte> cpimg = img.Resize(width, height, Emgu.CV.CvEnum.Inter.Linear);

            // save the frame to the videowriter
            videowriter.Write(cpimg.Mat);
            videoFrameCounter += 1;
            if(currentSec != DateTime.Now.Second)
            {
                Console.WriteLine("Number of video frames: " + videoFrameCounter);
                currentSec = DateTime.Now.Second;
                videoFrameCounter = 0;
            }
        }
    }
}
