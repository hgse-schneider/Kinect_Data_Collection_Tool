using Emgu.CV;
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
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    class Logger
    {
        /// <summary>
        /// information for logging the data from the interface
        /// </summary>
        public OpeningPrompt openingPrompt;

        // general information
        public bool recording = false;
        public string session;

        // what do we need to log
        public bool log_upperbody = false;
        public bool log_lowerbody = false;
        public bool log_angles = false;
        public bool log_movements = false;
        public bool log_pitch_yaw_roll = false;
        public bool log_mouth_eyes = false;
        public bool log_are_talking = false;
        public bool log_sound = false;

        // frequency at which to save the data 
        public int frequency = 1;

        // type of output file
        public bool outputcsv = true;
        public bool outputxlsx = true;

        // folder and filename for the log
        public string destination;
        public string logFilename;
        public System.IO.StreamWriter logFile;

        // counters
        public int current_sec = 0;
        public int num_rows = 0;
        public int row_count = 0;

        // header information
        public string header = "";
        public string[] header_array;

        // annotation and previous data point
        public string annotation = "";
        public string[] previousLine = new string[] { "","","","","","","","" };

        // video writer and list of people speaking
        public VideoWriter videowriter = null;
        public List<ulong> trackingIDSpeaking = new List<ulong>();

        /// <summary>
        /// Initializes a new instance of the Logger class.
        /// </summary>
        public Logger(OpeningPrompt openingPrompt)
        {
            // saves data from the opening prompt,
            // (those values disappear as soon as the user closes the window)
            this.openingPrompt = openingPrompt;
            session = openingPrompt.SessionID.Text;
            destination = openingPrompt.savingDataPath.Text;
            log_upperbody = openingPrompt.captureUpperSkeletons.Checked;
            log_lowerbody = openingPrompt.captureLowerSkeleton.Checked;
            log_angles = openingPrompt.computeJointAngles.Checked;
            log_movements = openingPrompt.quantify_movements.Checked;
            log_pitch_yaw_roll = openingPrompt.pitch_yaw_roll.Checked;
            log_mouth_eyes = openingPrompt.mouth_eyes.Checked;
            log_are_talking = openingPrompt.are_talking.Checked;
            log_sound = openingPrompt.are_talking.Checked;
            outputcsv = openingPrompt.outputCSV.Checked;
            outputxlsx = openingPrompt.outputXLSX.Checked;
            frequency = (openingPrompt.hertz.Value - 1) *5;
            if (frequency == 0) frequency = 1;
            current_sec = DateTime.Now.Second;

            // define the logs filenames
            logFilename = string.Format(@"{0}-Kinect-Log-{1}.csv", session, Helpers.getTimestamp("filename"));

            // combine the path and the filenames
            logFilename = Path.Combine(destination, logFilename);

            // start log files
            logFile = new System.IO.StreamWriter(logFilename, true);

            // print headers (ugly,should be re-written more cleanly)
            header = "Timestamp,Index,BodyID,";

            if (log_upperbody) header +=
                 "SpineBase_X,SpineBase_Y,SpineBase_Z,SpineBase_inferred," +
                 "SpineMid_X,SpineMid_Y,SpineMid_Z,SpineMid_inferred," +
                 "Neck_X,Neck_Y,Neck_Z,Neck_inferred," +
                 "Head_X,Head_Y,Head_Z,Head_inferred," +
                 "ShoulderLeft_X,ShoulderLeft_Y,ShoulderLeft_Z,ShoulderLeft_inferred," +
                 "ElbowLeft_X,ElbowLeft_Y,ElbowLeft_Z,ElbowLeft_inferred," +
                 "WristLeft_X,WristLeft_Y,WristLeft_Z,WristLeft_inferred," +
                 "HandLeft_X,HandLeft_Y,HandLeft_Z,HandLeft_inferred," +
                 "ShoulderRight_X,ShoulderRight_Y,ShoulderRight_Z,ShoulderRight_inferred," +
                 "ElbowRight_X,ElbowRight_Y,ElbowRight_Z,ElbowRight_inferred," +
                 "WristRight_X,WristRight_Y,WristRight_Z,WristRight_inferred," +
                 "HandRight_X,HandRight_Y,HandRight_Z,HandRight_inferred," +
                 "HipLeft_X,HipLeft_Y,HipLeft_Z,HipLeft_inferred," +
                 "HipRight_X,HipRight_Y,HipRight_Z,HipRight_inferred," +
                 "SpineShoulder_X,SpineShoulder_Y,SpineShoulder_Z,SpineShoulder_inferred," +
                "HandTipLeft_X,HandTipLeft_Y,HandTipLeft_Z,HandTipLeft_inferred," +
                "ThumbLeft_X,ThumbLeft_Y,ThumbLeft_Z,ThumbLeft_inferred," +
                "HandTipRight_X,HandTipRight_Y,HandTipRight_Z,HandTipRight_inferred," +
                "ThumbRight_X,ThumbRight_Y,ThumbRight_Z,ThumbRight_inferred,";
            if (log_lowerbody) header +=
                 "KneeLeft_X,KneeLeft_Y,KneeLeft_Z,KneeLeft_inferred," +
                 "AnkleLeft_X,AnkleLeft_Y,AnkleLeft_Z,AnkleLeft_inferred," +
                 "FootLeft_X,FootLeft_Y,FootLeft_Z,FootLeft_inferred," +
                 "KneeRight_X,KneeRight_Y,KneeRight_Z,KneeRight_inferred," +
                 "AnkleRight_X,AnkleRight_Y,AnkleRight_Z,AnkleRight_inferred," +
                 "FootRight_X,FootRight_Y,FootRight_Z,FootRight_inferred,";
            if (log_upperbody) header +=
                "HandLeftState,HandLeftStateConfidence,HandRightState,HandRightStateConfidence," +
                "Lean_X,Lean_Y,Lean_TrackingState,";
            if (log_movements)
            {
                if (log_upperbody)
                    foreach (string joint in Helpers.upper_body_joints)
                        header += joint + "_movement,";
                if (log_lowerbody)
                    foreach (string joint in Helpers.lower_body_joints)
                        header += joint + "_movement,";
            }
            if (log_angles) header +=
                 "Neck_angle,Spine_angle,Hip_angle," +
                 "ShoulderL_angle,ShoulderR_angle," +
                 "ElbowL_angle,ElbowR_angle," +
                 "WristL_angle,WristR_angle," +
                 "HandL_angle,HandR_angle,";
            if (log_mouth_eyes) header +=
                "Happy,Engaged,WearingGlasses,LeftEyeClosed,RightEyeClosed," +
                "MouthOpen,MouthMoved,LookingAway,";
            if (log_pitch_yaw_roll) header +=
                "FaceYaw,FacePitch,FacenRoll,";
            if (log_sound) header +=
                "Talking,";

            header += "Posture";

            // save header to the csv file
            logFile.WriteLine(header);

            this.header_array =  header.Split(',');

        }

        public void WriteFrameToVideo(WriteableBitmap colorBitmap, ColorFrame frame)
        {
            if (this.openingPrompt.videoNo.Checked) return;

            // get the scaling factor from the opening prompt
            int scaleFactor = 1;
            if (this.openingPrompt.videoMedium.Checked) scaleFactor = 2;
            if (this.openingPrompt.videoSmall.Checked) scaleFactor = 4;

            // resize the image if necessary
            int width = colorBitmap.PixelWidth / scaleFactor;
            int height = colorBitmap.PixelHeight / scaleFactor;
            Size frameSize = new Size(width, height);
            
            // create the videowriter if it doesn't exist yet
            if (videowriter == null)
            {
                String videoFilename = string.Format(@"{0}-Kinect-video-{1}.avi", session, Helpers.getTimestamp("datetime"));

                videoFilename = Path.Combine(this.destination, videoFilename);

                videowriter = new VideoWriter(videoFilename, //File name
                                        -1, //Video format -1 opens a dialogue window
                                        15, //FPS
                                        frameSize, //frame size
                                        true); //Color
            }

            // convert the color frame into an EMGU mat and resize it
            Image<Emgu.CV.Structure.Bgra, byte> img = Helpers.ToImage(frame);
            Image<Emgu.CV.Structure.Bgra, byte> cpimg = img.Resize(width, height, Emgu.CV.CvEnum.Inter.Linear);

            // save the frame to the videowriter
            videowriter.Write(cpimg.Mat);
        }

        /// <summary>
        /// returns an angle between three 3D points
        /// </summary>
        public String GetAngle(CameraSpacePoint a, CameraSpacePoint b, CameraSpacePoint c)
        {
            // b is the center point and becomes the origin
            Point3D ba = new Point3D( a.X - b.X, a.Y - b.Y, a.Z - b.Z );
            Point3D bc = new Point3D( c.X - b.X, c.Y - b.Y, c.Z - b.Z );
            
            double baVec = Math.Sqrt(ba.X * ba.X + ba.Y * ba.Y + ba.Z * ba.Z);
            double bcVec = Math.Sqrt(bc.X * bc.X + bc.Y * bc.Y + bc.Z * bc.Z);

            Point3D baNorm = new Point3D ( ba.X / baVec, ba.Y / baVec, ba.Z / baVec );
            Point3D bcNorm = new Point3D ( bc.X / bcVec, bc.Y / bcVec, bc.Z / bcVec );

            double res = baNorm.X * bcNorm.X + baNorm.Y * bcNorm.Y + baNorm.Z * bcNorm.Z;

            return Convert.ToString(Math.Acos(res) * 180.0 / Math.PI);
        }

        /// <summary>
        /// depending on the sampling frequency, we might skip some data points
        /// </summary>
        public Boolean skip_data()
        {
            // check if we need to record this data
            this.num_rows += 1;

            if (DateTime.Now.Second != this.current_sec)
            {
                this.current_sec = DateTime.Now.Second;
                this.num_rows = 0;
            }
            else
            {
                if (this.num_rows % (30 / frequency) != 0)
                    return true;
            }
            return false;
        }

        public string distance_between_joints(string joint, string[] cur, string[] pre)
        {
            int index = Array.IndexOf(this.header_array, joint + "_X");

            double cur_x = Convert.ToDouble(cur[index]);
            double cur_y = Convert.ToDouble(cur[index + 1]);
            double cur_z = Convert.ToDouble(cur[index + 2]);
            int cur_inferred = Int32.Parse((cur[index + 3]));

            double pre_x = Convert.ToDouble(pre[index]);
            double pre_y = Convert.ToDouble(pre[index + 1]);
            double pre_z = Convert.ToDouble(pre[index + 2]);
            int pre_inferred = Int32.Parse((pre[index + 3]));

            if (cur_inferred == 1 || pre_inferred == 1) return "";

            double total = Math.Sqrt(Math.Pow(cur_x - pre_x, 2) + Math.Pow(cur_y - pre_y, 2) + Math.Pow(cur_z - pre_z, 2));

            return ""+total;
        }


        /// <summary>
        /// saves skeleton data to the log file
        /// </summary>
        public String record_body_data(String data, Bodies drawingBodies, int bodyIndex, Body body)
        {
            // get the joints
            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                String trackingStateBinary = "";

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    trackingStateBinary = "0";
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    trackingStateBinary = "1";
                }

                if (this.header.Contains(jointType.ToString()))
                    data += joints[jointType].Position.X.ToString() + ","
                         + joints[jointType].Position.Y.ToString() + ","
                         + joints[jointType].Position.Z.ToString() + ","
                         + trackingStateBinary + ",";
            }

            // in addition, if we log the upper body we also save information about hand states
            if (this.log_upperbody)
            {
                data += body.HandLeftState + ","
                    + body.HandLeftConfidence + ","
                    + body.HandRightState + ","
                    + body.HandRightConfidence + ",";

                // Leaning left and right corresponds to X movement and leaning forward and back corresponds to Y movement.
                data += body.Lean.X + ","
                    + body.Lean.Y + ","
                    + body.LeanTrackingState + ",";
            }

            // record joint movements
            if (log_movements)
            {
                // skip if if null
                if (previousLine[bodyIndex] == "")
                {
                    if (log_upperbody)
                        foreach (string joint in Helpers.upper_body_joints)
                            data += ',';
                    if (log_lowerbody)
                        foreach (string joint in Helpers.upper_body_joints)
                            data += ',';
                }
                else
                {
                    string[] pre = this.previousLine[bodyIndex].Split(',');
                    string[] cur = data.Split(',');

                    if (log_upperbody)
                        foreach (string joint in Helpers.upper_body_joints)
                        {
                            data += distance_between_joints(joint, cur, pre) + ",";
                        }
                    if (log_lowerbody)
                        foreach (string joint in Helpers.lower_body_joints)
                        {
                            data += distance_between_joints(joint, cur, pre) + ",";
                        }
                }
            }

            // ----- RECORDING ANGLES INFO ------- // 
            if (log_angles)
            {
                data += GetAngle(joints[JointType.Head].Position, joints[JointType.Neck].Position, joints[JointType.SpineShoulder].Position) + ","
                     + GetAngle(joints[JointType.Neck].Position, joints[JointType.SpineShoulder].Position, joints[JointType.SpineMid].Position) + ","
                     + GetAngle(joints[JointType.HipRight].Position, joints[JointType.SpineBase].Position, joints[JointType.HipLeft].Position) + ","

                     // shoulders
                     + GetAngle(joints[JointType.SpineShoulder].Position, joints[JointType.ShoulderLeft].Position, joints[JointType.ElbowLeft].Position) + ","
                     + GetAngle(joints[JointType.SpineShoulder].Position, joints[JointType.ShoulderRight].Position, joints[JointType.ElbowRight].Position) + ","

                     // Elbows
                     + GetAngle(joints[JointType.ShoulderLeft].Position, joints[JointType.ElbowLeft].Position, joints[JointType.WristLeft].Position) + ","
                     + GetAngle(joints[JointType.ShoulderRight].Position, joints[JointType.ElbowRight].Position, joints[JointType.WristRight].Position) + ","

                     // Wrists
                     + GetAngle(joints[JointType.ElbowLeft].Position, joints[JointType.WristLeft].Position, joints[JointType.HandLeft].Position) + ","
                     + GetAngle(joints[JointType.ElbowRight].Position, joints[JointType.WristRight].Position, joints[JointType.HandRight].Position) + ","

                     // Hands (angle between thumb and hand tip)
                     + GetAngle(joints[JointType.HandTipLeft].Position, joints[JointType.HandLeft].Position, joints[JointType.ThumbLeft].Position) + ","
                     + GetAngle(joints[JointType.HandTipRight].Position, joints[JointType.HandRight].Position, joints[JointType.ThumbRight].Position) + ",";
            }
            return data;
        }


        /// <summary>
        /// saves face information to the log file
        /// </summary>
        public String record_face_data(String data, Bodies drawingBodies, Boolean faceTracked, FaceFrameResult faceResult)
        {
            if (log_mouth_eyes || log_pitch_yaw_roll)
            {
                if (faceResult == null) return data;

                // extract each face property information and store it in faceText
                if (log_mouth_eyes && faceResult.FaceProperties != null)
                {
                    foreach (var item in faceResult.FaceProperties)
                    {
                        data += item.Value.ToString() + ",";
                    }
                }

                // extract face rotation in degrees as Euler angles
                if (faceResult.FaceRotationQuaternion != null && log_pitch_yaw_roll)
                {
                    int pitch, yaw, roll;
                    drawingBodies.ExtractFaceRotationInDegrees(faceResult.FaceRotationQuaternion, out pitch, out yaw, out roll);
                    data += yaw + "," + pitch + "," + roll + ",";
                }
                data = data.Replace("Unknown", "");
            }
            return data;
        }
        
        /// <summary>
        /// checks whether the current body is talking
        /// </summary>
        public String record_talk_data(String data, Body body)
        {
            if (log_are_talking)
            {
                if (this.trackingIDSpeaking.Contains(body.TrackingId))
                    data += "1,";
                else data += "0,";
            }

            return data;
        }

        /// <summary>
        /// save data to file
        /// </summary>
        public void saveData(Bodies drawingBodies, int bodyIndex, Body body, Boolean faceTracked, FaceFrameResult faceResult)
        {
            Console.WriteLine(bodyIndex);
            if (recording)
            {
                // depending on the sampling frequency, we might skip some data
                if (skip_data()) return;
                row_count += 1;

                // get the timestamp and creat the line for the log
                String data = Helpers.getTimestamp("datetime").ToString() + "," + this.row_count + "," + bodyIndex + ",";

                // ----- RECORD BODY INFO ------- // 
                data = record_body_data(data, drawingBodies, bodyIndex, body);

                // ----- RECORD FACE INFO ------- // 
                data = record_face_data(data, drawingBodies, faceTracked, faceResult);

                // ----- RECORD AUDIO INFO ------- // 
                data = record_talk_data(data, body);

                // add annotation (posture)
                data += this.annotation;

                // save the data to the log file
                logFile.WriteLine(data);

                // save the data
                previousLine[bodyIndex] = data;
            }
        }

        /// <summary>
        /// convert the csv file into an xslx file (with correct timestamp)
        /// </summary>
        public void convert_csv_to_xlsx()
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
        
        /// <summary>
        /// close the data logger object
        /// </summary>
        public void close()
        {
            logFile.Close();

            if (this.outputxlsx == true)
                convert_csv_to_xlsx();

            if(this.outputcsv == false)
                File.Delete(this.logFilename);
        }
    }
}
