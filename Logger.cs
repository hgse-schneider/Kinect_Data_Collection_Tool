using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class Logger
    {
        /// <summary>
        /// information for logging the data
        /// </summary>
        public bool recording = false;
        public bool log_upperbody = false;
        public bool log_lowerbody = false;
        public bool log_face = false;
        public bool log_sound = false;
        public bool outputcsv = true;
        public bool outputxlsx = true;
        public string header = "";
        public string annotation = "";
        public string session;
        public string destination;
        public string logFilename;
        public System.IO.StreamWriter logFile;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public Logger(OpeningPrompt openingPrompt)
        {
            // retrieve data from the opening prompt
            session = openingPrompt.SessionID.Text;
            destination = openingPrompt.savingDataPath.Text;
            log_upperbody = openingPrompt.captureUpperSkeletons.Checked;
            log_lowerbody = openingPrompt.captureLowerSkeleton.Checked;
            log_face = openingPrompt.captureFaces.Checked;
            log_sound = openingPrompt.captureSounds.Checked;
            outputcsv = openingPrompt.outputCSV.Checked;
            outputxlsx = openingPrompt.outputXLSX.Checked;

            // define the logs filenames
            logFilename = string.Format(@"{0}-Kinect-Log-{1}.csv", session, this.getTimestamp("datetime"));

            // combine the path and the filenames
            logFilename = Path.Combine(destination, logFilename);

            // start log files
            logFile = new System.IO.StreamWriter(logFilename, true);

            // print headers
            header = "Timestamp, BodyID, ";

            if (log_upperbody) header +=
                 "SpineBase_X, SpineBase_Y, SpineBase_Z, SpineBase_inferred, " +
                 "SpineMid_X, SpineMid_Y, SpineMid_Z, SpineMid_inferred, " +
                 "Neck_X, Neck_Y, Neck_Z, Neck_inferred, " +
                 "Head_X, Head_Y, Head_Z, Head_inferred, " +
                 "ShoulderLeft_X, ShoulderLeft_Y, ShoulderLeft_Z, ShoulderLeft_inferred, " +
                 "ElbowLeft_X, ElbowLeft_Y, ElbowLeft_Z, ElbowLeft_inferred, " +
                 "WristLeft_X, WristLeft_Y, WristLeft_Z, WristLeft_inferred, " +
                 "HandLeft_X, HandLeft_Y, HandLeft_Z, HandLeft_inferred, " +
                 "ShoulderRight_X, ShoulderRight_Y, ShoulderRight_Z, ShoulderRight_inferred, " +
                 "ElbowRight_X, ElbowRight_Y, ElbowRight_Z, ElbowRight_inferred, " +
                 "WristRight_X, WristRight_Y, WristRight_Z, WristRight_inferred, " +
                 "HandRight_X, HandRight_Y, HandRight_Z, HandRight_inferred, " +
                 "HipLeft_X, HipLeft_Y, HipLeft_Z, HipLeft_inferred, ";
            if (log_lowerbody) header +=
                 "KneeLeft_X, KneeLeft_Y, KneeLeft_Z, KneeLeft_inferred, " +
                 "AnkleLeft_X, AnkleLeft_Y, AnkleLeft_Z, AnkleLeft_inferred, " +
                 "FootLeft_X, FootLeft_Y, FootLeft_Z, FootLeft_inferred, " +
                 "HipRight_X, HipRight_Y, HipRight_Z, HipRight_inferred, " +
                 "KneeRight_X, KneeRight_Y, KneeRight_Z, KneeRight_inferred, " +
                 "AnkleRight_X, AnkleRight_Y, AnkleRight_Z, AnkleRight_inferred, " +
                 "FootRight_X, FootRight_Y, FootRight_Z, FootRight_inferred, " +
                 "SpineShoulder_X, SpineShoulder_Y, SpineShoulder_Z, SpineShoulder_inferred, ";
            if (log_upperbody) header +=
                "HandTipLeft_X, HandTipLeft_Y, HandTipLeft_Z, HandTipLeft_inferred, " +
                "ThumbLeft_X, ThumbLeft_Y, ThumbLeft_Z, ThumbLeft_inferred, " +
                "HandTipRight_X, HandTipRight_Y, HandTipRight_Z, HandTipRight_inferred, " +
                "ThumbRight_X, ThumbRight_Y, ThumbRight_Z, ThumbRight_inferred, " +
                "HandLeftState, HandLeftStateConfidence, HandRightState, HandRightStateConfidence," +
                "Lean_X, Lean_Y, Lean_TrackingState, ";
            if (log_face) header +=
                "Happy, Engaged, WearingGlasses, LeftEyeClosed, RightEyeClosed," +
                " MouthOpen, MouthMoved, LookingAway, " +
                "FaceYaw, FacePitch, FacenRoll";

            header += ", Posture";
            logFile.WriteLine(header);
        }

        /// <summary>
        /// save data to file
        /// </summary>
        public void saveData(DrawingBodies drawingBodies, int bodyIndex, Body body, Boolean faceTracked, FaceFrameResult faceResult)
        {
            if (recording)
            {
                // ----- RECORDING BODY INFO ------- // 

                // get the timestamp and creat the line for the log
                String data = getTimestamp(null).ToString() + ", " + bodyIndex + ", ";

                // get the joints
                IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                // Draw the joints
                foreach (JointType jointType in joints.Keys)
                {
                    String trackingStateBinary = "";

                    TrackingState trackingState = joints[jointType].TrackingState;

                    if (trackingState == TrackingState.Tracked)
                    {
                        trackingStateBinary = "1";
                    }
                    else if (trackingState == TrackingState.Inferred)
                    {
                        trackingStateBinary = "0";
                    }

                    if (this.header.Contains(jointType.ToString()))
                        data += joints[jointType].Position.X.ToString() + ", "
                             + joints[jointType].Position.Y.ToString() + ", "
                             + joints[jointType].Position.Z.ToString() + ", "
                             + trackingStateBinary + ", ";
                }
                
                if(this.log_upperbody)
                {
                    data += body.HandLeftState + ", "
                        + body.HandLeftConfidence + ", "
                        + body.HandRightState + ", "
                        + body.HandRightConfidence + ", ";

                    // Leaning left and rightd corresponds to X movement and leaning forward and back corresponds to Y movement.
                    data += body.Lean.X + ", "
                        + body.Lean.Y + ", "
                        + body.LeanTrackingState + ", ";
                }

                // ----- RECORDING FACE INFO ------- // 

                if(log_face)
                {
                    if (faceResult == null) return;

                    // extract each face property information and store it in faceText
                    if (faceResult.FaceProperties != null)
                    {
                        foreach (var item in faceResult.FaceProperties)
                        {
                            data += item.Value.ToString() + ", ";
                        }
                    }

                    // extract face rotation in degrees as Euler angles
                    if (faceResult.FaceRotationQuaternion != null)
                    {
                        int pitch, yaw, roll;
                        drawingBodies.ExtractFaceRotationInDegrees(faceResult.FaceRotationQuaternion, out pitch, out yaw, out roll);
                        data += yaw + ", " + pitch + "," + roll;
                    }

                    data = data.Replace("Unknown", "");
                }

                data += ", " + this.annotation;
                logFile.WriteLine(data);
            }
        }

        /// <summary>
        /// get the current timestamp
        /// </summary>
        public string getTimestamp(String type)
        {
            if (type == "datetime")
                return DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            else if (type == "date")
                return DateTime.Now.ToString("yyyy-MM-dd");
            else if (type == "time")
                return DateTime.Now.ToString("HH:mm:ss");
            else
                return DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }
        
        /// <summary>
        /// get the current timestamp
        /// </summary>
        public void close_logger()
        {
            logFile.Close();

            if (this.outputxlsx == true)
            {
                // convert the csv file to xlsx
                string csvFilePath = this.logFilename;
                string excelFilePath = this.logFilename.Substring(0, this.logFilename.Length - 4) + ".xlsx";

                string worksheetsName = "DATA";
                bool firstRowIsHeader = true;

                var excelTextFormat = new ExcelTextFormat();
                excelTextFormat.Delimiter = ',';
                excelTextFormat.EOL = "\r";

                var excelFileInfo = new FileInfo(excelFilePath);
                var csvFileInfo = new FileInfo(csvFilePath);

                using (ExcelPackage package = new ExcelPackage(excelFileInfo))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(worksheetsName);
                    worksheet.Cells["A1"].LoadFromText(csvFileInfo, excelTextFormat, OfficeOpenXml.Table.TableStyles.Medium25, firstRowIsHeader);
                    worksheet.Column(1).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
                    package.Save();
                }
            }

            if(this.outputcsv == false)
            {
                File.Delete(this.logFilename);
            }

        }
    }
}
