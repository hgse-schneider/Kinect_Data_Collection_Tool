using Emgu.CV;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
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

    }
}
