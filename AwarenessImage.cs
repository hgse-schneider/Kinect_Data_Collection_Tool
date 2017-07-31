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
        /// reference to the two bodies we chose
        /// </summary>
        public Body left = null;
        public Body right = null;

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

        /// <summary>
        /// Constructor
        /// </summary>
        public AwarenessImage(KinectSensor kinectSensor, Bodies bodies, CheckBox displayTalk)
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

            // get the reference to the checkbox
            this.displayTalk = displayTalk;

            // get the reference to the bodies
            this.bodies = bodies;
        }
        
        private void findBodies()
        {
            for (int i = 0; i < this.bodies.bodyCount; i++)
            {
                Body body = this.bodies.bodies[i];

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

        private void updateAwarenessImage(object sender, BodyFrameArrivedEventArgs e)
        {
            // we don't do anything if we have less than 2 bodies
            if (this.bodies == null) return;

            // we display the image if the checkbox is checked
            if (this.displayTalk.IsChecked.Value)
            {
                // define who's on which side
                if (this.right == null && this.left == null) findBodies();

                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    if((Application.Current.MainWindow) != null)
                    {
                        var h = ((Panel)Application.Current.MainWindow.Content).ActualHeight;
                        var w = ((Panel)Application.Current.MainWindow.Content).ActualWidth;
                        Rect displayRect = new Rect(0, 0, w, h);

                        Pen drawingPen = new Pen(Brushes.Green, 10);
                        dc.DrawRectangle(Brushes.Green, drawingPen, new Rect(0, 0, 1, 1));

                        this.drawingGroup.ClipGeometry = new RectangleGeometry(displayRect);
                    }
                }
            }
            else
            {
                this.drawingGroup.Children.Clear();
            }
        }
    }
}
