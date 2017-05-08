using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;
using AForge.Imaging.Filters;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;

namespace QRQode
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox1.Text == "- Refresh -")
            {
                get_listCam();
            }
        }
        private FilterInfoCollection webcam = null;
        private VideoCaptureDevice cam = null;
        private void get_listCam()
        {
            try
            {
                toolStripComboBox1.Items.Clear();
                webcam = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                foreach (FilterInfo Vid in webcam)
                {
                    toolStripComboBox1.Items.Add(Vid.Name);
                }
                toolStripComboBox1.Items.Add("- Refresh -");
                toolStripComboBox1.SelectedIndex = 0;
            }
            catch (Exception)
            {
                MessageBox.Show("Error, Please refresh the Video Device List!");
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {

            errorProvider1.Clear();
            if (textBox1.Text == "")
            {
                errorProvider1.SetError(textBox1, "Not Fill");
                return;
            }
            var qr = new ZXing.BarcodeWriter();
            qr.Options = new QrCodeEncodingOptions
            {
                DisableECI = true,
                CharacterSet = "UTF-8",
                Width = Convert.ToInt16(numericUpDown1.Value),
                Height = Convert.ToInt16(numericUpDown2.Value),
            };
            if (comboBox1.Text == "BAR CODE")
            {
                char[] ch = textBox1.Text.ToCharArray();
                for(int i = 0; i < ch.Length; i++)
                {
                    if(!char.IsNumber(ch[i]))
                    {
                        errorProvider1.SetError(textBox1, "Only Numbers");
                        return;
                    }
                }
                qr.Format = ZXing.BarcodeFormat.CODABAR;
            }
            if (comboBox1.Text == "QR CODE")
            {
                qr.Format = ZXing.BarcodeFormat.QR_CODE;
            }
            var result = new Bitmap(qr.Write(textBox1.Text.Trim()));
            pictureBox1.Image = result;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp|GIF|*.gif";
            if(s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox1.Image.Save(s.FileName);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.Filter = "PNG|*.png|JPEG|*.jpg|BMP|*.bmp|GIF|*.gif";
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pictureBox1.ImageLocation = o.FileName;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            BarcodeReader Reader = new BarcodeReader();
            Result result = Reader.Decode((Bitmap)pictureBox1.Image);
            try
            {
                string decoded = result.ToString().Trim();

                if (decoded != "")
                {
                    MessageBox.Show(decoded);
                }
            }
            catch (Exception)
            {

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Start();
            button4.Enabled = false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripButton1.Text == "Start")
                {
                    toolStripButton1.Text = "Close";
                    cam = new VideoCaptureDevice(webcam[toolStripComboBox1.SelectedIndex].MonikerString);
                    cam.NewFrame += Cam_NewFrame;
                    cam.Start();
                }
                else
                {
                    if (cam.IsRunning)
                    {
                        cam.Stop();
                    }
                    toolStripButton1.Text = "Start";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error! Can not start the device.");
            }
        }
        Bitmap temp;
        private void Cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                
                temp = (Bitmap)eventArgs.Frame.Clone();
                pictureBox1.Image = temp;
            }
            catch (Exception)
            {

            }

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cam != null)
                if (cam.IsRunning)
                {
                    cam.Stop();
                }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            get_listCam();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            BarcodeReader Reader = new BarcodeReader();
            Result result = Reader.Decode((Bitmap)pictureBox1.Image);
            try
            {
                string decoded = result.ToString().Trim();

                if (decoded != "")
                {
                    timer1.Stop();
                    timer1.Enabled = false;
                    button4.Enabled = true;
                    MessageBox.Show(decoded);
                }
            }
            catch (Exception)
            {

            }
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}
