﻿using Emgu.CV;
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
        // size of the color image
        public int colorWidth = 1920;
        public int ColorHeight = 1080;

        // scale factor defined in the opening prompt
        public int scaleFactor = 1;

        // number of frames currently recorded and current second
        public int videoFrameCounter = 0;
        public int currentSecFrameCounter = 0;
        public int currentMilliSec = 0; 

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

            // the first time that the function is called, it creates the video writer
            if (videowriter == null) createVideoFrameWriter();

            // resize the image if necessary
            int width = colorBitmap.PixelWidth / scaleFactor;
            int height = colorBitmap.PixelHeight / scaleFactor;
            Size frameSize = new Size(width, height);

            // convert the color frame into an EMGU mat and resize it
            Image<Emgu.CV.Structure.Bgra, byte> img = Helpers.ToImage(frame);
            Image<Emgu.CV.Structure.Bgra, byte> cpimg = img.Resize(width, height, Emgu.CV.CvEnum.Inter.Linear);

            // save the frame to the videowriter
            if(currentSecFrameCounter < 15)
            {
                videowriter.Write(cpimg.Mat);
                videoFrameCounter += 1;
                currentSecFrameCounter += 1;
            }

            if (this.stopwatch.ElapsedMilliseconds > 1000)
            {
                // if we don't have enough frames for the current second, we repeat some
                while (currentSecFrameCounter < 15)
                {
                    videowriter.Write(cpimg.Mat);
                    videoFrameCounter += 1;
                    currentSecFrameCounter += 1;
                }

                Console.WriteLine("Number of video frames: " + videoFrameCounter);
                this.stopwatch.Reset();
                this.stopwatch.Start();
                currentSecFrameCounter = 0;
            }
        }
    }
}
