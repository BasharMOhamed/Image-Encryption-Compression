using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ImageMatrix = ImageOperations.Encryption_Decryption(ImageMatrix, TapPostion, InitialSeed);
            sw.Stop();
            Console.WriteLine("Encryption Time elapsed: {0:hh\\:mm\\:ss\\.fff}", sw.Elapsed);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void btnOpenCompressed_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                //Open the browsed image and display it
                string OpenedFilePath = openFileDialog2.FileName;
                Stopwatch sw = new Stopwatch();
                sw.Start();
                ImageMatrix = ImageOperations.Load_Decompression(OpenedFilePath, ref InitialSeed, ref TapPostion);
                sw.Stop();
                Console.WriteLine("Load & Decompression Time elapsed: {0:hh\\:mm\\:ss\\.fff}", sw.Elapsed);
                ImageOperations.DisplayImage(ImageMatrix, pictureBox1);
            }
            txtWidth.Text = ImageOperations.GetWidth(ImageMatrix).ToString();
            txtHeight.Text = ImageOperations.GetHeight(ImageMatrix).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ImageMatrix = ImageOperations.Encryption_Decryption(ImageMatrix, TapPostion, InitialSeed);
            sw.Stop();
            Console.WriteLine("Decryption Time elapsed: {0:hh\\:mm\\:ss\\.fff}", sw.Elapsed);
            ImageOperations.DisplayImage(ImageMatrix, pictureBox2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(InitialSeed == "")
            {
                TapPostion = (int)nudMaskSize.Value;
                InitialSeed = txtGaussSigma.Text;
            }
            string path = $@"F:\3rd Year\Second Term\Algo\Project\Compressed Files\{FileName.Text}.bin";

            Stopwatch sw = new Stopwatch();
            sw.Start();
            ImageOperations.Compression(ImageMatrix, InitialSeed, TapPostion, path);
            sw.Stop();
            Console.WriteLine("Compression Time elapsed: {0:hh\\:mm\\:ss\\.fff}", sw.Elapsed);
        }

        private void FileName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}