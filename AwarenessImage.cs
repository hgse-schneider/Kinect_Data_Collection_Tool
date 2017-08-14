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
        public int left_talking = 1;
        public int right_talking = 1;
        public Dictionary<int,int> left_talking_dic = new Dictionary<int, int>();
        public Dictionary<int,int> right_talking_dic = new Dictionary<int, int>();

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
        /// reference to the main window
        /// </summary>
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
            // reinitialize the two bodies
            this.left = null;
            this.right = null;

            for (int i = 0; i < this.bodies.bodyCount; i++)
            {
                Body body = this.bodies.bodies[i];

                if (!body.IsTracked) continue;

                // fill out the references it we don't have them already
                if (left == null) this.left = body;
                else if (right == null) this.right = body;

                // if we have more than two bodies, we keep the two that are the closest to the kinect
                else
                {
                    // the new body is closer to the kinect
                    if (body.Joints[JointType.Head].Position.Z < left.Joints[JointType.Head].Position.Z || 
                        body.Joints[JointType.Head].Position.Z < right.Joints[JointType.Head].Position.Z)
                    {
                        // left is closer to the kinect, so we swap right with the new body
                        if (left.Joints[JointType.Head].Position.Z < right.Joints[JointType.Head].Position.Z)
                            right = body;
                        else if (left.Joints[JointType.Head].Position.Z > right.Joints[JointType.Head].Position.Z)
                            left = body;
                        else Console.WriteLine("This is an error; this line should not be executed!");
                    }
                }

                // swap the bodies if necessary (right becomes left, left becomes right)
                if(left != null && right != null)
                    if (left.Joints[JointType.Head].Position.X > right.Joints[JointType.Head].Position.X)
                    { Body tmp = left; left = right; right = tmp; }
            }
        }

        /// <summary>
        /// Updates dictionary of talking time
        /// </summary>
        private void update_dictionary(string side, int talk_time)
        {
            // get the dictionary
            Dictionary<int,int> dic = left_talking_dic;
            if (side == "right") dic = right_talking_dic;

            // update the values
            int time = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            if (!dic.ContainsKey(time)) dic.Add(time, 0);
            dic[time] += talk_time;

            // remove older values
            if (dic.ContainsKey(time - 30)) dic.Remove(time - 30);

        }

        /// <summary>
        /// Handles the face frame data arriving from the sensor
        /// </summary>
        private void update_talking_time()
        {
            // retrieve the amount of talking since last time
            if (left != null && audio != null)
                if (this.audio.talking_frames.ContainsKey(left.TrackingId))
                {
                    left_talking += this.audio.talking_frames[left.TrackingId];
                    update_dictionary("left", this.audio.talking_frames[left.TrackingId]);
                    this.audio.talking_frames[left.TrackingId] = 0;
                }
            // retrieve the amount of talking since last time
            if (right != null && audio != null)
                if (this.audio.talking_frames.ContainsKey(right.TrackingId))
                {
                    right_talking += this.audio.talking_frames[right.TrackingId];
                    update_dictionary("right", this.audio.talking_frames[right.TrackingId]);
                    this.audio.talking_frames[right.TrackingId] = 0;
                }
        }

        /// <summary>
        /// Handles the face frame data arriving from the sensor
        /// </summary>
        private void updateAwarenessImage(object sender, BodyFrameArrivedEventArgs e)
        {
            // we don't do anything if we don't have any body
            if (this.bodies == null) return;

            // we display the image if the checkbox is checked
            if (this.displayTalk.IsChecked.Value)
            {
                // define who's on which side
                findBodies();

                // update how much talking as taken place since last time
                update_talking_time();

                // draw the two rectangles
                using (DrawingContext dc = this.drawingGroup.Open())
                {
                    // the application crashes when it starts otherwise
                    if(Application.Current.MainWindow != null)
                    {
                        // size of the window
                        float h = (float)((Panel)Application.Current.MainWindow.Content).ActualHeight;
                        float w = (float)((Panel)Application.Current.MainWindow.Content).ActualWidth;
                        
                        // compute width of each rectangle
                        float total = left_talking + right_talking;
                        if (total == 0 || left_talking == 0 || right_talking == 0) return;

                        // compute the % of talk for each person
                        //float left_per = left_talking / total;  
                        float tmp_left = left_talking_dic.Sum(x => x.Value);
                        float tmp_right = right_talking_dic.Sum(x => x.Value);
                        float left_per = tmp_left / (tmp_left + tmp_right);

                        // set the colors of the rectangles
                        Brush left_color = Brushes.Green;
                        Brush right_color = Brushes.Blue;
                        if (left == null) right_color = Brushes.LightBlue;
                        if (right == null) left_color = Brushes.LightGreen;

                        // Draw a transparent background to set the render size
                        dc.DrawRectangle(left_color, null, new Rect(0.0, 0.0, w*left_per, h));
                        dc.DrawRectangle(right_color, null, new Rect(w*left_per, 0.0, w, h));

                        // print amount of talking
                        //Console.WriteLine("Left: " + left_talking + "     Right: " + right_talking + "     %: " + left_per);
                        Console.WriteLine("Left: " + tmp_left + "     Right: " + tmp_right + "     %: " + left_per);
                    }
                }
            }
            // if the checkbox isn't checked, we clear the display
            else this.drawingGroup.Children.Clear();
        }
    }
}
