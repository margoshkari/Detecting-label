using Amazon.Rekognition;
using Amazon.Rekognition.Model;
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
using Image = Amazon.Rekognition.Model.Image;
using Label = Amazon.Rekognition.Model.Label;

namespace Detecting_label
{
    public partial class Form1 : Form
    {
        string path = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }
        private void startBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "png|*.png|jpeg|*.jpeg|jpg|*.jpg";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    path = ofd.FileName;
                    this.pictureBox1.Image = System.Drawing.Image.FromFile(path);
                    this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
                    this.startBtn.Enabled = false;
                    Main();
                }
            }
        }
        public async Task Main()
        {
            string photo = path;

            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials("AKIA3BZMVBVKD2IHGKEZ", "rlTB6Ne7BJxDgX1nCOrMU98uDVdP8ux/VZkrgPqe");
            var rekognitionClient = new Amazon.Rekognition.AmazonRekognitionClient(awsCreden‌tials, Amazon.RegionEndpoint.USEast1);

            var image = new Amazon.Rekognition.Model.Image();
            try
            {
                using var fs = new FileStream(photo, FileMode.Open, FileAccess.Read);
                byte[] data = null;
                data = new byte[fs.Length];
                fs.Read(data, 0, (int)fs.Length);
                image.Bytes = new MemoryStream(data);
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to load file " + photo);
                return;
            }
            var detectlabelsRequest = new DetectLabelsRequest()
            {
                Image = image,
                MaxLabels = 10,
                MinConfidence = 75F,
            };

            try
            {
                DetectLabelsResponse detectLabelsResponse = await rekognitionClient.DetectLabelsAsync(detectlabelsRequest);
                foreach (Label label in detectLabelsResponse.Labels)
                {
                    label.Instances.ForEach(instance => ShowBoundingBoxPositions(instance.BoundingBox));

                    MessageBox.Show($"Name: {label.Name} Confidence: {label.Confidence}");
                }
                this.startBtn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void ShowBoundingBoxPositions(BoundingBox box)
        {
            float top = (box.Top * pictureBox1.Height);
            float left = (box.Left * pictureBox1.Width);
            float right = pictureBox1.Width * box.Width;
            float bottom = pictureBox1.Height * box.Height;
            Pen pen = new Pen(Color.Aqua, 2);
            using (Graphics g = pictureBox1.CreateGraphics())
            {
                g.DrawRectangle(pen, left, top, right, bottom);
            }
        }

    }
}
