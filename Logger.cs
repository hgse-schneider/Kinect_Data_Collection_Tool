using Emgu.CV;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using OfficeOpenXml;
using System;
using System.Collections;
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
    public class Logger
    {
        /// <summary>
        /// information for logging the data from the interface
        /// </summary>
        public OpeningPrompt openingPrompt;

        // for random numbers
        public Random rnd = new Random();

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
        public bool log_dyad = false;
        public bool log_sound = false;

        // frequency at which to save the data
        public int frequency = 1;

        // type of output file
        public bool outputcsv = true;
        public bool outputxlsx = true;

        // folder and filename for the log
        public string destination;
        public string[] logFilename = new string[] { "", "", "", "", "", "", "", "", "", "" };
        public System.IO.StreamWriter[] logFile = new System.IO.StreamWriter[] { null, null, null, null, null, null, null, null, null, null };
        public System.IO.StreamWriter logDyad = null;

        // counters
        public int current_sec = 0;
        public int[] row_index = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int[] row_count = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public int row_count_dyad = 0;
        public int bodies_tracked = 0;

        // header information
        public string header = "";
        public Dictionary<string, int> header_dic = new Dictionary<string, int>();

        // previous data point (index in the array is the Body ID)
        public string[] faceInfo = new string[] { "", "", "", "", "", "", "", "", "", "" };
        public string[] previousLine = new string[] { "", "", "", "", "", "", "", "", "", "" };

        // video writer and list of people speaking
        public VideoWriter videowriter = null;

        // get a reference to the main window
        private MainWindow main;

        /// <summary>
        /// Initializes a new instance of the Logger class.
        /// </summary>
        public Logger(OpeningPrompt openingPrompt, MainWindow main)
        {
            // get a reference to the main window 
            this.main = main;

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
            log_dyad = openingPrompt.capture_dyad.Checked;
            log_sound = openingPrompt.are_talking.Checked;
            outputcsv = openingPrompt.outputCSV.Checked;
            outputxlsx = openingPrompt.outputXLSX.Checked;
            frequency = (openingPrompt.hertz.Value - 1) *5;
            if (frequency == 0) frequency = 1;
            current_sec = DateTime.Now.Second;

            // create a unique ID for the session if Default
            if (session == "Default") session += rnd.Next(1, 99);

            // print headers (ugly,should be re-written more cleanly)
            header = "Timestamp,Session,Index,BodyID,";

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

            // save the index information of each element
            string[] header_array = header.Split(',');
            for(int i = 0; i < header_array.Length; i++)
            {
                this.header_dic.Add(header_array[i], i);
            }
        }

        public System.IO.StreamWriter logFileWriterForBody(int bodyIndex)
        {
            // define the logs filenames, the path, and start the log file
            logFilename[bodyIndex] = string.Format(@"{0}-{1}-Kinect-Log-{2}.csv", session, bodyIndex, Helpers.getTimestamp("filename"));
            logFilename[bodyIndex] = Path.Combine(destination, logFilename[bodyIndex]);
            System.IO.StreamWriter logFile = new System.IO.StreamWriter(logFilename[bodyIndex], true);

            // save header to the csv file
            logFile.WriteLine(header);

            return logFile;
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
                String videoFilename = string.Format(@"{0}-Kinect-video-{1}.avi", session, Helpers.getTimestamp("filename"));

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
        public Boolean skip_data(int bodyIndex)
        {
            // check if we need to record this data
            this.row_index[bodyIndex] += 1;

            if (DateTime.Now.Second != this.current_sec)
            {
                this.current_sec = DateTime.Now.Second;
                this.row_index[bodyIndex] = 0;
            }
            else
            {
                if (this.row_index[bodyIndex] % (30 / frequency) != 0)
                    return true;
            }
            return false;
        }

        public Tuple<double,double> left_rightmost_points(Body b1)
        {
            double left = 9999.9999;
            double right = -9999.9999;
            
            foreach (JointType jointType in b1.Joints.Keys)
            {
                // easier access
                Joint j = b1.Joints[jointType];

                // skip inferred joints
                if(j.TrackingState == TrackingState.Inferred) continue;

                // update the doubles
                if (j.Position.X < left) left = j.Position.X;
                if (j.Position.X > right) right = j.Position.X;
            }

            return new Tuple<double, double>(left, right);
        }

        public double distance_between_3D_points(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            double deltaX = x1 - x2;
            double deltaY = y1 - y2;
            double deltaZ = z1 - z2;

            double distance = (double)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);

            return distance;

            //return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2) + Math.Pow(z1 - z2, 2));
        }

        public double distance_between_kinect_joints(Joint j1, Joint j2)
        {
            return distance_between_3D_points(
                j1.Position.X, j1.Position.Y, j1.Position.Z, 
                j2.Position.X, j2.Position.Y, j2.Position.Z);
        }

        public string distance_between_string_joints(string joint, string[] cur, string[] pre)
        {
            int index = this.header_dic[joint + "_X"];

            double cur_x = Convert.ToDouble(cur[index]);
            double cur_y = Convert.ToDouble(cur[index + 1]);
            double cur_z = Convert.ToDouble(cur[index + 2]);
            int cur_inferred = Int32.Parse((cur[index + 3]));

            double pre_x = Convert.ToDouble(pre[index]);
            double pre_y = Convert.ToDouble(pre[index + 1]);
            double pre_z = Convert.ToDouble(pre[index + 2]);
            int pre_inferred = Int32.Parse((pre[index + 3]));

            if (cur_inferred == 1 || pre_inferred == 1) return "";

            double total = distance_between_3D_points(cur_x, cur_y, cur_z, pre_x, pre_y, pre_z);

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
                        foreach (string joint in Helpers.lower_body_joints)
                            data += ',';
                }
                else
                {
                    string[] pre = this.previousLine[bodyIndex].Split(',');
                    string[] cur = data.Split(',');

                    if (log_upperbody)
                        foreach (string joint in Helpers.upper_body_joints)
                        {
                            data += distance_between_string_joints(joint, cur, pre) + ",";
                        }
                    if (log_lowerbody)
                        foreach (string joint in Helpers.lower_body_joints)
                        {
                            data += distance_between_string_joints(joint, cur, pre) + ",";
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
        public String record_face_data(String data, Bodies drawingBodies, int bodyIndex, 
            Boolean faceTracked, FaceFrameResult faceResult)
        {
            if (log_mouth_eyes || log_pitch_yaw_roll)
            {
                if (faceResult == null)
                {
                    if (log_mouth_eyes) data += ",,,,,,,,"; // check size of header
                    if (log_pitch_yaw_roll) data += ",,,";
                    this.faceInfo[bodyIndex] = ",,";
                    return data;
                }

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
                    this.faceInfo[bodyIndex] = yaw + "," + pitch + "," + roll;
                }
                data = data.Replace("Unknown", "");
            }
            return data;
        }
        
        /// <summary>
        /// checks whether the current body is talking
        /// </summary>
        public int is_body_speaking(Body body)
        {
            if (this.main.audio.trackingIDSpeaking.Contains(body.TrackingId))
                return 1;
            else return 0;
        }

        public int count_bodies(Bodies drawingBodies)
        {
            int count = 0;

            foreach (Body body in drawingBodies.bodies)
                if (body.IsTracked) count += 1;

            return count;
        }

        /// <summary>
        /// save data to file
        /// </summary>
        public void saveData(Bodies drawingBodies, int bodyIndex, Body body, Boolean faceTracked, FaceFrameResult faceResult)
        {
            if (recording)  
            {
                // if there is no log file writer for this body, we create it
                if (this.logFile[bodyIndex] == null) this.logFile[bodyIndex] = logFileWriterForBody(bodyIndex);

                // depending on the sampling frequency, we might skip some data
                if (skip_data(bodyIndex)) return;

                // increment the counter for the number of observations for this body
                if (row_count[bodyIndex] == 0 && row_count.Max() == 0) row_count[bodyIndex] = 0;
                if (row_count[bodyIndex] == 0 && row_count.Max() > 0) row_count[bodyIndex] = row_count.Max() - 1; // to sync with the previous body
                row_count[bodyIndex] += 1;

                // count the number of bodies
                this.bodies_tracked = count_bodies(drawingBodies);

                // get the timestamp and creat the line for the log
                String data = Helpers.getTimestamp("datetime").ToString() + "," + this.session + "," + this.row_count[bodyIndex] + "," + bodyIndex + ",";

                // ----- RECORD BODY INFO ------- // 
                data = record_body_data(data, drawingBodies, bodyIndex, body);

                // ----- RECORD FACE INFO ------- // 
                data = record_face_data(data, drawingBodies, bodyIndex, faceTracked, faceResult);

                // ----- RECORD AUDIO INFO ------- // 
                if(this.log_are_talking) data += is_body_speaking(body);

                // save the data to the log file
                this.logFile[bodyIndex].WriteLine(data);

                // save the data
                this.previousLine[bodyIndex] = data;

                // saving dyadic data, if necessary
                if (this.log_dyad) save_dyadic_data(drawingBodies);
            }
        }

        public void save_dyadic_data(Bodies drawingBodies)
        {
            // save and sort the bodies based on their proximity to the sensor
            List<int> listBodies = new List<int>();
            //foreach (Body body in drawingBodies.bodies)
            //    if (body.IsTracked) listBodies.Add(body);
            for(int i=0; i< drawingBodies.bodies.Length; i++)
                if (drawingBodies.bodies[i].IsTracked) listBodies.Add(i);
            listBodies = listBodies.OrderBy(o => drawingBodies.bodies[o].Joints[JointType.Head].Position.Z).ToList(); //OrderByDescending

            // skip if we don't have enough bodies
            //if (listBodies.Count < 2) return;

            // initialize the log file
            if (this.logDyad == null)
            {
                string tmp = string.Format(@"{0}-Dyad-Log-{1}.csv", session, Helpers.getTimestamp("filename"));
                this.logDyad = new System.IO.StreamWriter(Path.Combine(this.destination, tmp), true);

                // write header
                this.logDyad.WriteLine("Timestamp,Index,"
                    +"Body1_leftSide,Body2_rightSide,Talking1,Talking2,"
                    +"LeanX1,LeanY1,LeanX2,LeanY2,"
                    + "Pitch1,Yaw1,Roll1,Pitch2,Yaw2,Roll2,"
                    + "leftmostX1,rightmostX1,leftmostX2,rightmostX2,"
                    + "dist_heads,dist_spine,dist_max,dist_min,"
                    //+ "joint_attention"
                    );
            }

            // body 1 is on the left side, body 2 on the right side
            int id1 = listBodies[0];
            int id2 = listBodies[0];
            if (drawingBodies.bodies[id1].Joints[JointType.Head].Position.X > 
                drawingBodies.bodies[id2].Joints[JointType.Head].Position.X)
            {
                id1 = listBodies[1];
                id2 = listBodies[0];
            }

            // for easier referrencing
            Body b1 = drawingBodies.bodies[id1];
            Body b2 = drawingBodies.bodies[id2];

            // save basic data
            this.row_count_dyad += 1;
            String data = Helpers.getTimestamp("datetime").ToString()
                + "," + this.row_count_dyad + "," + id1 + "," + id2
                + "," + is_body_speaking(b1) + "," + is_body_speaking(b2)
                + "," + b1.Lean.X + "," + b1.Lean.Y + "," + b2.Lean.X + "," + b2.Lean.Y
                + "," + this.faceInfo[id1] + "," + this.faceInfo[id2] + ",";

            // get the leftmost and rightmost positions
            Tuple<double, double> rightleftmost1 = left_rightmost_points(b1);
            Tuple<double, double> rightleftmost2 = left_rightmost_points(b2);
            data += rightleftmost1.Item1 + "," + rightleftmost1.Item2 + ","
                + rightleftmost2.Item1 + "," + rightleftmost2.Item2 + ",";

            // compute the distance between people
            data += distance_between_kinect_joints(b1.Joints[JointType.Head], b2.Joints[JointType.Head]) + "," 
                + distance_between_kinect_joints(b1.Joints[JointType.SpineMid], b2.Joints[JointType.SpineMid]) + ","
                + Math.Abs(rightleftmost1.Item1 - rightleftmost2.Item2) + ","
                + Math.Abs(rightleftmost1.Item2 - rightleftmost2.Item1) + ",";

            // does the pitch change when people are leaning forward?
            double jva_probability = 0.5;
            jva_probability += (Math.Abs(b1.Lean.Y - b2.Lean.Y) -1 ) / 10.0;
            if(this.faceInfo[id1].Length > 5 && this.faceInfo[id2].Length > 5)
            {
                string[] face1 = this.faceInfo[id1].Split(',');
                int pitch1 = Int32.Parse(face1[0]);
                int yaw1 = Int32.Parse(face1[1]);
                string[] face2 = this.faceInfo[id2].Split(',');
                int pitch2 = Int32.Parse(face1[0]);
                int yaw2 = Int32.Parse(face1[1]);
                // information from pitch
                if (Math.Abs(pitch1 - pitch2) < 10)
                    jva_probability += 0.1;
                else jva_probability -= 0.1;
                // information from yaw
                if (yaw1 < 0 && yaw2 > 0) // good!
                    jva_probability += 0.2 - (Math.Abs(yaw1 - yaw2)) / 450;
                else jva_probability -= 0.2;
            }
            jva_probability += (Math.Abs(b1.Lean.Y - b2.Lean.Y) - 1) / 10.0;
            //data += jva_probability+",";

            // save to file
            this.logDyad.WriteLine(data);
        }

        /// <summary>
        /// convert the csv file into an xslx file (with correct timestamp)
        /// </summary>
        public void convert_csv_to_xlsx(int bodyIndex)
        {
            // convert the csv file to xlsx
            string csvFilePath = this.logFilename[bodyIndex];
            string excelFilePath = csvFilePath.Substring(0, csvFilePath.Length - 4) + ".xlsx";

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
            for(int i=0; i<this.logFile.Length; i++)
            {
                if (this.logFile[i] != null)
                    this.logFile[i].Close();
            }

            if (this.logDyad != null)
                this.logDyad.Close();

            if (this.outputxlsx == true)
                for (int i = 0; i < this.logFile.Length; i++)
                {
                    if (this.logFile[i] != null)
                        convert_csv_to_xlsx(i);
                }

            if(this.outputcsv == false)
                for (int i = 0; i < this.logFile.Length; i++)
                {
                    if (this.logFile[i] != null)
                        File.Delete(this.logFilename[i]);
                }
        }
    }
}
