using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class ColorImage
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        public KinectSensor kinectSensor = null;
        public CheckBox displayImage;

        /// <summary>
        /// Logger
        /// </summary>
        public Logger logger = null;

        /// <summary>
        /// Reader for color frames
        /// </summary>
        public ColorFrameReader colorFrameReader = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        public WriteableBitmap colorBitmap = null;

        /// <summary>
        /// was the last frame saved?
        /// </summary>
        public Boolean was_last_frame_saved = false;
        public double fps;

        /// <summary>
        /// counter for the number of images displayed
        /// </summary>
        public int counter = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        public ColorImage(KinectSensor kinectSensor, Logger logger)
        {
            // get a reference to the kinect sensor
            this.kinectSensor = kinectSensor;
            this.logger = logger;

            // open the reader for the color frames
            this.colorFrameReader = this.kinectSensor.ColorFrameSource.OpenReader();

            // wire handler for frame arrival
            this.colorFrameReader.FrameArrived += this.Reader_ColorFrameArrived;

            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            FrameDescription colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // create the bitmap to display
            this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);

        }


        /// <summary>
        /// Handles the color frame data arriving from the sensor
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Reader_ColorFrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // increment the counter
            counter += 1;

            // we skip this frame if we don't need it
            //if (logger.videowriter == null && this.counter % logger.frequency != 0) return;

            // ColorFrame is IDisposable
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {

                if (colorFrame != null)
                {
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    // detects if we are running at 30 or 15 fps
                    this.fps = 1.0 / colorFrame.ColorCameraSettings.FrameInterval.TotalSeconds;

                    // if we are running at 30 fps and the last frame was saved, we skip it
                    if (was_last_frame_saved && fps > 30)
                    {
                        was_last_frame_saved = false;
                        return;
                    }

                    // grab the color frame
                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            if(this.displayImage.IsChecked.Value)
                                this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }
                        
                        this.colorBitmap.Unlock();

                        logger.WriteFrameToVideo(colorBitmap, colorFrame);

                        was_last_frame_saved = true;
                    }
                    
                }
            }
        }


        /// <summary>
        /// closes the color image reader
        /// </summary>
        public void close()
        {
            if (this.colorFrameReader != null)
            {
                // ColorFrameReder is IDisposable
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }
        }
    }
}
