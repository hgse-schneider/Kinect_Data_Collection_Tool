using System;
using System.Windows.Forms;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    partial class OpeningPrompt
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SessionID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ok = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.captureUpperSkeletons = new System.Windows.Forms.CheckBox();
            this.pitch_yaw_roll = new System.Windows.Forms.CheckBox();
            this.captureLowerSkeleton = new System.Windows.Forms.CheckBox();
            this.chooseFolder = new System.Windows.Forms.Button();
            this.savingDataLabel = new System.Windows.Forms.Label();
            this.savingDataPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.quantify_movements = new System.Windows.Forms.CheckBox();
            this.mouth_eyes = new System.Windows.Forms.CheckBox();
            this.hertzLabel = new System.Windows.Forms.Label();
            this.hertz = new System.Windows.Forms.TrackBar();
            this.computeJointAngles = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.outputXLSX = new System.Windows.Forms.CheckBox();
            this.outputCSV = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.are_talking = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.videoLarge = new System.Windows.Forms.RadioButton();
            this.videoMedium = new System.Windows.Forms.RadioButton();
            this.videoSmall = new System.Windows.Forms.RadioButton();
            this.videoNo = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.capture_dyad = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hertz)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // SessionID
            // 
            this.SessionID.Location = new System.Drawing.Point(39, 56);
            this.SessionID.Margin = new System.Windows.Forms.Padding(4);
            this.SessionID.Name = "SessionID";
            this.SessionID.Size = new System.Drawing.Size(243, 31);
            this.SessionID.TabIndex = 0;
            this.SessionID.Text = "Default";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Session Name";
            // 
            // ok
            // 
            this.ok.Location = new System.Drawing.Point(39, 650);
            this.ok.Margin = new System.Windows.Forms.Padding(4);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(100, 42);
            this.ok.TabIndex = 2;
            this.ok.Text = "Ok";
            this.ok.UseVisualStyleBackColor = true;
            this.ok.Click += new System.EventHandler(this.ok_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(165, 650);
            this.cancel.Margin = new System.Windows.Forms.Padding(4);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(117, 42);
            this.cancel.TabIndex = 3;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // captureUpperSkeletons
            // 
            this.captureUpperSkeletons.AutoSize = true;
            this.captureUpperSkeletons.Checked = true;
            this.captureUpperSkeletons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.captureUpperSkeletons.Location = new System.Drawing.Point(19, 70);
            this.captureUpperSkeletons.Margin = new System.Windows.Forms.Padding(4);
            this.captureUpperSkeletons.Name = "captureUpperSkeletons";
            this.captureUpperSkeletons.Size = new System.Drawing.Size(239, 29);
            this.captureUpperSkeletons.TabIndex = 4;
            this.captureUpperSkeletons.Text = "Capture Upper Body";
            this.captureUpperSkeletons.UseVisualStyleBackColor = true;
            // 
            // pitch_yaw_roll
            // 
            this.pitch_yaw_roll.AutoSize = true;
            this.pitch_yaw_roll.Checked = true;
            this.pitch_yaw_roll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pitch_yaw_roll.Location = new System.Drawing.Point(367, 70);
            this.pitch_yaw_roll.Margin = new System.Windows.Forms.Padding(4);
            this.pitch_yaw_roll.Name = "pitch_yaw_roll";
            this.pitch_yaw_roll.Size = new System.Drawing.Size(195, 29);
            this.pitch_yaw_roll.TabIndex = 5;
            this.pitch_yaw_roll.Text = "Pitch, Yaw, Roll";
            this.pitch_yaw_roll.UseVisualStyleBackColor = true;
            this.pitch_yaw_roll.CheckedChanged += new System.EventHandler(this.captureFaces_CheckedChanged);
            // 
            // captureLowerSkeleton
            // 
            this.captureLowerSkeleton.AutoSize = true;
            this.captureLowerSkeleton.Checked = true;
            this.captureLowerSkeleton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.captureLowerSkeleton.Location = new System.Drawing.Point(19, 108);
            this.captureLowerSkeleton.Margin = new System.Windows.Forms.Padding(4);
            this.captureLowerSkeleton.Name = "captureLowerSkeleton";
            this.captureLowerSkeleton.Size = new System.Drawing.Size(239, 29);
            this.captureLowerSkeleton.TabIndex = 6;
            this.captureLowerSkeleton.Text = "Capture Lower Body";
            this.captureLowerSkeleton.UseVisualStyleBackColor = true;
            this.captureLowerSkeleton.CheckedChanged += new System.EventHandler(this.captureSound_CheckedChanged);
            // 
            // chooseFolder
            // 
            this.chooseFolder.Location = new System.Drawing.Point(19, 311);
            this.chooseFolder.Margin = new System.Windows.Forms.Padding(4);
            this.chooseFolder.Name = "chooseFolder";
            this.chooseFolder.Size = new System.Drawing.Size(244, 39);
            this.chooseFolder.TabIndex = 7;
            this.chooseFolder.Text = "Choose Folder";
            this.chooseFolder.UseVisualStyleBackColor = true;
            this.chooseFolder.Click += new System.EventHandler(this.chooseFolderClick);
            // 
            // savingDataLabel
            // 
            this.savingDataLabel.AutoSize = true;
            this.savingDataLabel.Location = new System.Drawing.Point(13, 230);
            this.savingDataLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.savingDataLabel.Name = "savingDataLabel";
            this.savingDataLabel.Size = new System.Drawing.Size(156, 25);
            this.savingDataLabel.TabIndex = 8;
            this.savingDataLabel.Text = "Saving data to:";
            // 
            // savingDataPath
            // 
            this.savingDataPath.Location = new System.Drawing.Point(19, 271);
            this.savingDataPath.Margin = new System.Windows.Forms.Padding(4);
            this.savingDataPath.Name = "savingDataPath";
            this.savingDataPath.Size = new System.Drawing.Size(636, 31);
            this.savingDataPath.TabIndex = 9;
            this.savingDataPath.Text = "C:\\Users\\SchneiderBertrand\\Desktop";
            this.savingDataPath.TextChanged += new System.EventHandler(this.savingDataPath_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.quantify_movements);
            this.groupBox1.Controls.Add(this.mouth_eyes);
            this.groupBox1.Controls.Add(this.hertzLabel);
            this.groupBox1.Controls.Add(this.hertz);
            this.groupBox1.Controls.Add(this.computeJointAngles);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.outputXLSX);
            this.groupBox1.Controls.Add(this.outputCSV);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.are_talking);
            this.groupBox1.Controls.Add(this.captureUpperSkeletons);
            this.groupBox1.Controls.Add(this.savingDataPath);
            this.groupBox1.Controls.Add(this.pitch_yaw_roll);
            this.groupBox1.Controls.Add(this.savingDataLabel);
            this.groupBox1.Controls.Add(this.chooseFolder);
            this.groupBox1.Controls.Add(this.captureLowerSkeleton);
            this.groupBox1.Location = new System.Drawing.Point(39, 114);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(677, 529);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log file";
            // 
            // quantify_movements
            // 
            this.quantify_movements.AutoSize = true;
            this.quantify_movements.Checked = true;
            this.quantify_movements.CheckState = System.Windows.Forms.CheckState.Checked;
            this.quantify_movements.Location = new System.Drawing.Point(19, 182);
            this.quantify_movements.Margin = new System.Windows.Forms.Padding(4);
            this.quantify_movements.Name = "quantify_movements";
            this.quantify_movements.Size = new System.Drawing.Size(241, 29);
            this.quantify_movements.TabIndex = 19;
            this.quantify_movements.Text = "Quantify Movements";
            this.quantify_movements.UseVisualStyleBackColor = true;
            // 
            // mouth_eyes
            // 
            this.mouth_eyes.AutoSize = true;
            this.mouth_eyes.Checked = true;
            this.mouth_eyes.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mouth_eyes.Location = new System.Drawing.Point(365, 108);
            this.mouth_eyes.Margin = new System.Windows.Forms.Padding(4);
            this.mouth_eyes.Name = "mouth_eyes";
            this.mouth_eyes.Size = new System.Drawing.Size(164, 29);
            this.mouth_eyes.TabIndex = 18;
            this.mouth_eyes.Text = "Mouth, Eyes";
            this.mouth_eyes.UseVisualStyleBackColor = true;
            this.mouth_eyes.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // hertzLabel
            // 
            this.hertzLabel.AutoSize = true;
            this.hertzLabel.Location = new System.Drawing.Point(13, 376);
            this.hertzLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.hertzLabel.Name = "hertzLabel";
            this.hertzLabel.Size = new System.Drawing.Size(176, 25);
            this.hertzLabel.TabIndex = 17;
            this.hertzLabel.Text = "Frequency: 15Hz";
            this.hertzLabel.Click += new System.EventHandler(this.label5_Click);
            // 
            // hertz
            // 
            this.hertz.LargeChange = 1;
            this.hertz.Location = new System.Drawing.Point(188, 376);
            this.hertz.Margin = new System.Windows.Forms.Padding(4);
            this.hertz.Maximum = 7;
            this.hertz.Minimum = 1;
            this.hertz.Name = "hertz";
            this.hertz.Size = new System.Drawing.Size(468, 90);
            this.hertz.TabIndex = 1;
            this.hertz.Value = 4;
            this.hertz.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // computeJointAngles
            // 
            this.computeJointAngles.AutoSize = true;
            this.computeJointAngles.Checked = true;
            this.computeJointAngles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.computeJointAngles.Location = new System.Drawing.Point(19, 145);
            this.computeJointAngles.Margin = new System.Windows.Forms.Padding(4);
            this.computeJointAngles.Name = "computeJointAngles";
            this.computeJointAngles.Size = new System.Drawing.Size(252, 29);
            this.computeJointAngles.TabIndex = 16;
            this.computeJointAngles.Text = "Compute Joint angles";
            this.computeJointAngles.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(360, 39);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 25);
            this.label4.TabIndex = 15;
            this.label4.Text = "Face";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 39);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 25);
            this.label3.TabIndex = 14;
            this.label3.Text = "Skeleton";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // outputXLSX
            // 
            this.outputXLSX.AutoSize = true;
            this.outputXLSX.Enabled = false;
            this.outputXLSX.Location = new System.Drawing.Point(19, 481);
            this.outputXLSX.Margin = new System.Windows.Forms.Padding(4);
            this.outputXLSX.Name = "outputXLSX";
            this.outputXLSX.Size = new System.Drawing.Size(88, 29);
            this.outputXLSX.TabIndex = 13;
            this.outputXLSX.Text = ".xlsx";
            this.outputXLSX.UseVisualStyleBackColor = true;
            // 
            // outputCSV
            // 
            this.outputCSV.AutoSize = true;
            this.outputCSV.Checked = true;
            this.outputCSV.CheckState = System.Windows.Forms.CheckState.Checked;
            this.outputCSV.Location = new System.Drawing.Point(19, 444);
            this.outputCSV.Margin = new System.Windows.Forms.Padding(4);
            this.outputCSV.Name = "outputCSV";
            this.outputCSV.Size = new System.Drawing.Size(83, 29);
            this.outputCSV.TabIndex = 12;
            this.outputCSV.Text = ".csv";
            this.outputCSV.UseVisualStyleBackColor = true;
            this.outputCSV.CheckedChanged += new System.EventHandler(this.outputCSV_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 415);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 25);
            this.label2.TabIndex = 11;
            this.label2.Text = "File format:";
            // 
            // are_talking
            // 
            this.are_talking.AutoSize = true;
            this.are_talking.Checked = true;
            this.are_talking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.are_talking.Location = new System.Drawing.Point(365, 145);
            this.are_talking.Margin = new System.Windows.Forms.Padding(4);
            this.are_talking.Name = "are_talking";
            this.are_talking.Size = new System.Drawing.Size(238, 29);
            this.are_talking.TabIndex = 10;
            this.are_talking.Text = "Are People Talking?";
            this.are_talking.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.videoLarge);
            this.groupBox2.Controls.Add(this.videoMedium);
            this.groupBox2.Controls.Add(this.videoSmall);
            this.groupBox2.Controls.Add(this.videoNo);
            this.groupBox2.Location = new System.Drawing.Point(743, 114);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(257, 232);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Video";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // videoLarge
            // 
            this.videoLarge.AutoSize = true;
            this.videoLarge.Enabled = false;
            this.videoLarge.Location = new System.Drawing.Point(23, 149);
            this.videoLarge.Margin = new System.Windows.Forms.Padding(4);
            this.videoLarge.Name = "videoLarge";
            this.videoLarge.Size = new System.Drawing.Size(211, 29);
            this.videoLarge.TabIndex = 3;
            this.videoLarge.Text = "Large 1920x1080";
            this.videoLarge.UseVisualStyleBackColor = true;
            this.videoLarge.CheckedChanged += new System.EventHandler(this.videoLarge_CheckedChanged);
            // 
            // videoMedium
            // 
            this.videoMedium.AutoSize = true;
            this.videoMedium.Location = new System.Drawing.Point(23, 111);
            this.videoMedium.Margin = new System.Windows.Forms.Padding(4);
            this.videoMedium.Name = "videoMedium";
            this.videoMedium.Size = new System.Drawing.Size(208, 29);
            this.videoMedium.TabIndex = 2;
            this.videoMedium.Text = "Medium 960x540";
            this.videoMedium.UseVisualStyleBackColor = true;
            // 
            // videoSmall
            // 
            this.videoSmall.AutoSize = true;
            this.videoSmall.Location = new System.Drawing.Point(23, 74);
            this.videoSmall.Margin = new System.Windows.Forms.Padding(4);
            this.videoSmall.Name = "videoSmall";
            this.videoSmall.Size = new System.Drawing.Size(182, 29);
            this.videoSmall.TabIndex = 1;
            this.videoSmall.Text = "small 480x270";
            this.videoSmall.UseVisualStyleBackColor = true;
            // 
            // videoNo
            // 
            this.videoNo.AutoSize = true;
            this.videoNo.Checked = true;
            this.videoNo.Location = new System.Drawing.Point(23, 36);
            this.videoNo.Margin = new System.Windows.Forms.Padding(4);
            this.videoNo.Name = "videoNo";
            this.videoNo.Size = new System.Drawing.Size(202, 29);
            this.videoNo.TabIndex = 0;
            this.videoNo.TabStop = true;
            this.videoNo.Text = "Don\'t save video";
            this.videoNo.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.capture_dyad);
            this.groupBox3.Location = new System.Drawing.Point(743, 368);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(257, 275);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Dyads";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // capture_dyad
            // 
            this.capture_dyad.AutoSize = true;
            this.capture_dyad.Enabled = false;
            this.capture_dyad.Location = new System.Drawing.Point(23, 31);
            this.capture_dyad.Margin = new System.Windows.Forms.Padding(4);
            this.capture_dyad.Name = "capture_dyad";
            this.capture_dyad.Size = new System.Drawing.Size(168, 29);
            this.capture_dyad.TabIndex = 20;
            this.capture_dyad.Text = "Capture data";
            this.capture_dyad.UseVisualStyleBackColor = true;
            this.capture_dyad.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged_1);
            // 
            // OpeningPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 708);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SessionID);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "OpeningPrompt";
            this.Text = "Kinect Data Collection Tool";
            this.Load += new System.EventHandler(this.OpeningPrompt_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.hertz)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox SessionID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Button cancel;
        public System.Windows.Forms.CheckBox captureUpperSkeletons;
        public System.Windows.Forms.CheckBox pitch_yaw_roll;
        public System.Windows.Forms.CheckBox captureLowerSkeleton;
        private System.Windows.Forms.Button chooseFolder;
        private System.Windows.Forms.Label savingDataLabel;
        public System.Windows.Forms.TextBox savingDataPath;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.CheckBox are_talking;
        public System.Windows.Forms.CheckBox outputXLSX;
        public System.Windows.Forms.CheckBox outputCSV;
        private System.Windows.Forms.Label label2;
        private Label label3;
        public CheckBox computeJointAngles;
        private Label label4;
        public TrackBar hertz;
        private Label hertzLabel;
        private GroupBox groupBox2;
        public RadioButton videoNo;
        public RadioButton videoLarge;
        public RadioButton videoMedium;
        public RadioButton videoSmall;
        private GroupBox groupBox3;
        public CheckBox mouth_eyes;
        public CheckBox quantify_movements;
        public CheckBox capture_dyad;
    }
}