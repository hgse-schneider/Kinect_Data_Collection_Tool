using Microsoft.Kinect;
using System;
using System.Collections.Generic;
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
    class AwarenessImage
    {
        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        public KinectSensor kinectSensor = null;

        /// <summary>
        /// the bodies detected by the kinect
        /// </summary>
        public Bodies bodies = null;

        /// <summary>
        /// the audio captured by the kinect
        /// </summary>
        public Audio audio = null;

        /// <summary>
        /// reference to the two bodies we chose
        /// </summary>
        public Body left = null;
        public Body right = null;

        /// <summary>
        /// how much each person has been talking so far
        /// </summary>
        public int left_talking = 0;
        public int right_talking = 0;

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        public CheckBox displayTalk;

        /// <summary>
        /// frame description
        /// </summary>
        public FrameDescription colorFrameDescription;

        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        public DrawingImage imageSource;

        /// <summary>
        /// Reader for body frames
        /// </summary>
        public BodyFrameReader bodyFrameReader = null;

        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;

        private MainWindow main;

        /// <summary>
        /// Constructor
        /// </summary>
        public AwarenessImage(KinectSensor kinectSensor, MainWindow main)
        {
            // get a reference to the kinect sensor
            this.kinectSensor = kinectSensor;

            // open the reader for the body frames
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            // wire handler for body frame arrival
            this.bodyFrameReader.FrameArrived += this.updateAwarenessImage;

            // create the colorFrameDescription from the ColorFrameSource using Bgra format
            this.colorFrameDescription = this.kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

            // Create the drawing group we'll use for drawing
            this.drawingGroup = new DrawingGroup();

            // Create an image source that we can use in our image control
            this.imageSource = new DrawingImage(this.drawingGroup);

            // get the reference to the main window
            this.main = main;

            // get the reference to the checkbox
            this.displayTalk = main.displayTalk;

            // get the reference to the bodies
            this.bodies = main.drawingBodies;

            // get the reference to the audio module
            this.audio = main.audio;
        }

        /// <summary>
        /// Go through the list of bodies detected by the kinect
        /// </summary>
        private void findBodies()
        {
            for (int i = 0; i < this.bodies.bodyCount; i++)
            {
                Body body = this.bodies.bodies[i];

                if (!body.IsTracked) continue;

                // fill out the references it we don't have them already
                if (left == null) this.left = body;
                else if (right == null) right = body;

                // if we have more than two bodies, we keep the two that are the closest to the kinect
                else
                {
                    if (body.Joints[JointType.Head].Position.Z < left.Joints[JointType.Head].Position.Z)
                        left = body;
                    else if (body.Joints[JointType.Head].Position.Z < right.Joints[JointType.Head].Position.Z)
                        right = body;
                }

                // swap the bodies if necessary (right becomes left, left becomes right)
                if(left != null && right != null)
                    if (left.Joints[JointType.Head].Position.X > right.Joints[JointType.Head].Position.X)
                    {
                        Body tmp = left;
                        left = right;
                        right = tmp;
                    }
            }
        }

        /// <summary>
        /// Handles the face frame data arriving from the sensor
        /// </summary>
        private void updateAwarenessImage(object sender, BodyFrameArrivedEventArgs e)
        {
            // we don't do anything if we have less than 2 bodies
            if (this.bodies == null) return;

            // we display the image if the checkbox is checked
            if (this.displayTalk.IsChecked.Value)
            {
                // define who's on which side
                if (this.right == null && this.left == null) findBodies();

                // update the amount of talking we are recording
                if (left != null && audio != null)
                    if (audio.is_bodyID_talking(left.TrackingId)) left_talking += 1;
                if (right != null && audio != null)
                    if (audio.is_bodyID_talking(right.TrackingId)) right_talking += 1;

                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    if(Application.Current.MainWindow != null)
                    {
                        // size of the window
                        float h = (float)((Panel)Application.Current.MainWindow.Content).ActualHeight;
                        float w = (float)((Panel)Application.Current.MainWindow.Content).ActualWidth;
                        

                        // compute width of each rectangle
                        float total = left_talking + right_talking;
                        if (total == 0 || left_talking == 0 || right_talking == 0) return;

                        float left_per = left_talking / total;
                        float right_per = right_talking / total;
                        float vert_left = 1 / left_per;
                        float vert_right = 1 / right_per;

                        // Draw a transparent background to set the render size
                        dc.DrawRectangle(Brushes.Green, null, new Rect(0.0, 0.0, w*left_per, h));
                        dc.DrawRectangle(Brushes.Red, null, new Rect(w*left_per, 0.0, w, h));

                        // print amount of talking
                        //Console.WriteLine("Left: " + left_talking + "     Right: " + right_talking + "     %: " + left_per);
                    }
                }
            }
            // if the checkbox isn't checked, we clear the display
            else
                this.drawingGroup.Children.Clear();
        }
    }
}
