//------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Microsoft.Samples.Kinect.BodyBasics
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;
    using Microsoft.Kinect;
    using Microsoft.Kinect.Face;
    using System.Collections;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        /// <summary>
        /// Active Kinect sensor
        /// </summary>
        private KinectSensor kinectSensor = null;

        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private Logger logger;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private AwarenessImage awarenessImage;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private ColorImage drawingColorImage;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        //private DepthImage drawingDepthImage;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private Bodies drawingBodies;
        
        /// <summary>
        /// object to deal with audio
        /// </summary>
        public Audio audio;

        /// <summary>
        /// Current status text to display
        /// </summary>
        private string statusText = null;
        


        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            // open a dialog prompt to give users options
            OpeningPrompt openingPrompt = new OpeningPrompt();
            openingPrompt.ShowDialog();     //Show secondary form, code execution stop until dialog is closed

            // one sensor is currently supported
            this.kinectSensor = KinectSensor.GetDefault();

            // create an object that will manage the log files
            this.logger = new Logger(openingPrompt, this);

            // get the coordinate mapper
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            // create an object to manage the color frames
            this.drawingColorImage = new ColorImage(this.kinectSensor, logger);

            // create an object to manage the color frames
            //this.drawingDepthImage = new DepthImage(this.kinectSensor);

            // create an object to manage the skeletons
            this.drawingBodies = new Bodies(this.kinectSensor, this.logger);

            // create an object to manage the awareness frames
            this.awarenessImage = new AwarenessImage(this.kinectSensor, drawingBodies, this.displayTalk);

            // creates an object to manage the audio
            this.audio = new Audio(this.kinectSensor, this.logger);

            // set IsAvailableChanged event notifier
            this.kinectSensor.IsAvailableChanged += this.Sensor_IsAvailableChanged;
            
            // Open the sensor
            this.kinectSensor.Open();

            // set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.NoSensorStatusText;

            // use the window object as the view model in this simple example
            this.DataContext = this;

            // initialize the components (controls) of the window
            this.InitializeComponent();

            // change the display
            this.savingTo.Content += openingPrompt.savingDataPath.Text;

            // update reference to interface elements
            this.drawingColorImage.displayImage = this.displayImage;
            this.drawingBodies.displayBodies = this.displayBodies;
            this.awarenessImage.displayTalk = this.displayTalk;
        }

        /// <summary>
        /// INotifyPropertyChangedPropertyChanged event to allow window controls to bind to changeable data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ImageSource
        {
            get { return this.drawingBodies.imageSource; }
        }

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
       /*
        public ImageSource DepthImageSource
        {
            get { return this.drawingDepthImage.depthBitmap; }
        }
        */

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource ColorImageSource
        {
            get { return this.drawingColorImage.colorBitmap; }
        }

        /// <summary>
        /// Gets the bitmap to display
        /// </summary>
        public ImageSource AwarenessImageSource
        {
            get { return this.awarenessImage.imageSource; }
        }

        /// <summary>
        /// Gets or sets the current status text to display
        /// </summary>
        public string StatusText
        {
            get { return this.statusText; }

            set
            {
                if (this.statusText != value)
                {
                    this.statusText = value;

                    // notify any bound elements that the text has changed
                    if (this.PropertyChanged != null)
                        this.PropertyChanged(this, new PropertyChangedEventArgs("StatusText"));
                }
            }
        }


        /// <summary>
        /// Execute start up tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.audio.reader != null)
            {
                // Subscribe to new audio frame arrived events
                this.audio.reader.FrameArrived += this.audio.Reader_FrameArrived;
            }

            for (int i = 0; i < this.drawingBodies.bodyCount; i++)
            {
                if (this.drawingBodies.faceFrameReaders[i] != null)
                {
                    // wire handler for face frame arrival
                    this.drawingBodies.faceFrameReaders[i].FrameArrived += this.drawingBodies.Reader_FaceFrameArrived;
                }
            }

            if (this.drawingBodies.bodyFrameReader != null)
            {
                // wire handler for body frame arrival
                this.drawingBodies.bodyFrameReader.FrameArrived += this.drawingBodies.Reader_FrameArrived;
            }
        }

        /// <summary>
        /// Execute shutdown tasks
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // close the depth image reader
            //this.drawingDepthImage.close();

            // close the body frame reader
            this.drawingBodies.close();

            // close the color image reader
            this.drawingColorImage.close();

            // close the audio object
            //this.audio.close();
            
            // make sure we finish writing our data to the file
            this.logger.close();

            // close the kinect sensor
            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }
        

        /// <summary>
        /// Handles the event which the sensor becomes unavailable (E.g. paused, closed, unplugged).
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Sensor_IsAvailableChanged(object sender, IsAvailableChangedEventArgs e)
        {
            // on failure, set the status text
            this.StatusText = this.kinectSensor.IsAvailable ? Properties.Resources.RunningStatusText
                                                            : Properties.Resources.SensorNotAvailableStatusText;
        }

        /// <summary>
        /// Handles the recording button at the top of the main window
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void startRecording_Click(object sender, RoutedEventArgs e)
        {
            if(logger.recording)
            {
                logger.recording = false;
                this.startRecording.Content = "Not Recording";
                this.startRecording.Background = Brushes.Red;

                //this.displayBodies.IsChecked = true;
                //this.displayImage.IsChecked = true;
            }
            else if(!logger.recording)
            {
                logger.recording = true;
                this.startRecording.Content = "Recording !";
                this.startRecording.Background = Brushes.LightGreen;

                //this.displayBodies.IsChecked = false;
                //this.displayImage.IsChecked = false;
            }
        }

        private void displayImage_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DisplayTalk_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            this.displayTalk.IsChecked = false;
            this.displayBodies.IsChecked = false;
        }
    }
}