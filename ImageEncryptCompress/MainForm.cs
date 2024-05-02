using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ImageEncryptCompress
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        RGBPixel[,] ImageMatrix;

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog1.FileName;
                ImageMatrix = ImageOperations.OpenImage(OpenedFilePath);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }
        int TapPostion = 0;
        string InitialSeed = "";
        private void btnGaussSmooth_Click(object sender, EventArgs e)
        {
            TapPostion = (int)nudMaskSize.Value ;
            InitialSeed = txtGaussSigma.Text;
            ImageMatrix = ImageOperations.Encryption_Decryption(ImageMatrix, TapPostion, InitialSeed);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void btnOpenCompressed_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog2.FileName;
                ImageMatrix = ImageOperations.Load_Decompression(OpenedFilePath, ref InitialSeed, ref TapPostion);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ImageMatrix = ImageOperations.Encryption_Decryption(ImageMatrix, TapPostion, InitialSeed);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*TapPostion = (int)nudMaskSize.Value;
            InitialSeed = txtGaussSigma.Text;*/
            string path = $@"F:\3rd Year\Second Term\Algo\Project\Compressed Files\{FileName.Text}.bin";
            ImageOperations.Compression(ImageMatrix, InitialSeed, TapPostion, path);
        }

        private void FileName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}