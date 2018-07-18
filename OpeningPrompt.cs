using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.Samples.Kinect.BodyBasics
{
    public partial class OpeningPrompt : Form
    {
        public OpeningPrompt()
        {
            InitializeComponent();

            // initializing the comboBox
            List<int> items = new List<int>() { 1, 5, 10, 15, 30 };
            fpsBox.DataSource = items;
            fpsBox.SelectedIndex = 3;
        }

        private void OpeningPrompt_Load(object sender, EventArgs e)
        {

        }

        private void cancel_Click(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void ok_Click(object sender, EventArgs e)
        {
            if (this.savingDataPath.Text == "")
                MessageBox.Show("please choose the folder where you would like to save your data", "No folder selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                this.Close();
        }

        private void chooseFolderClick(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                this.savingDataPath.Text = fbd.SelectedPath.ToString();
            }
        }

        private void captureFaces_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void captureSound_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int realValue = (hertz.Value - 1) *5;
            if (realValue == 0) realValue = 1;
            hertzLabel.Text = "Frequency: " + realValue + "Hz";
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void videoLarge_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void savingDataPath_TextChanged(object sender, EventArgs e)
        {

        }

        private void outputCSV_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void saveWav_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
