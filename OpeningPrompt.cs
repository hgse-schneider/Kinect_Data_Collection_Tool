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
    }
}
