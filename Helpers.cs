using Emgu.CV;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public static class Helpers
    {
        public static string[] upper_body_joints = {
                 "SpineBase",
                 "SpineShoulder",
                 "SpineMid",
                 "Neck",
                 "Head",
                 "ShoulderLeft",
                 "ElbowLeft",
                 "WristLeft",
                 "HandLeft",
                 "ShoulderRight",
                 "ElbowRight",
                 "WristRight",
                 "HandRight",
                 "HipLeft",
                 "HipRight",
                "HandTipLeft",
                "ThumbLeft",
                "HandTipRight",
                "ThumbRight"
                };

        public static string[] lower_body_joints = {
                 "KneeLeft",
                 "AnkleLeft",
                 "FootLeft",
                 "KneeRight",
                 "AnkleRight",
                 "FootRight"
                };

        /// <summary>
        /// cnoverts a color frame into an EMgu image
        /// </summary>
        public static Image<Emgu.CV.Structure.Bgra, byte> ToImage(this ColorFrame frame)
        {
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            System.Windows.Media.PixelFormat format = System.Windows.Media.PixelFormats.Bgr32;

            byte[] pixels = new byte[width * height * ((format.BitsPerPixel + 7) / 8)];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(pixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
            }

            Image<Emgu.CV.Structure.Bgra, byte> img = new Image<Emgu.CV.Structure.Bgra, byte>(width, height);
            img.Bytes = pixels;

            return img;
        }

        /// <summary>
        /// get the current timestamp
        /// </summary>
        public static string getTimestamp(String type)
        {
            if (type == "filename")
                return DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss");
            else if (type == "datetime")
                return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            else if (type == "date")
                return DateTime.Now.ToString("yyyy-MM-dd");
            else if (type == "time")
                return DateTime.Now.ToString("HH:mm:ss");
            else if (type == "second")
                return DateTime.Now.ToString("ss");
            else if (type == "unix")
                return ""+(Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            else
                return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
        }

        /// <summary>
        /// simple math helper to compute the distance between two 3D points
        /// </summary>
        public static double distance_between_3D_points(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            double deltaX = x1 - x2;
            double deltaY = y1 - y2;
            double deltaZ = z1 - z2;

            double distance = (double)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            return distance;

            //return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2) + Math.Pow(z1 - z2, 2));
        }

        /// <summary>
        /// compute thes distance between two Kinect joints in 3D space
        /// </summary>
        public static double distance_between_kinect_joints(Joint j1, Joint j2)
        {
            return distance_between_3D_points(
                j1.Position.X, j1.Position.Y, j1.Position.Z,
                j2.Position.X, j2.Position.Y, j2.Position.Z);
        }

        // log exceptions in a file
        internal static Exception Log(this Exception ex, string path, string[] cur, string[] pre)
        {
            string filename = "CaughtExceptions" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            string destination = Path.Combine(path, filename);
            File.AppendAllText(destination, DateTime.Now.ToString("HH:mm:ss") + ": " + ex.Message + "\n" + ex.ToString() + "\n PRE: " + pre + "\n CUR: " + cur + "\n");
            return ex;
        }

    }
}
