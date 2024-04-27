using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
///Algorithms Project
///Intelligent Scissors
///

namespace ImageEncryptCompress
{
    /// <summary>
    /// Holds the pixel color in 3 byte values: red, green and blue
    /// </summary>
    public struct RGBPixel
    {
        public byte red, green, blue;
    }
    public struct RGBPixelD
    {
        public double red, green, blue;
    }


    /// <summary>
    /// Library of static functions that deal with images
    /// </summary>
    public class ImageOperations
    {
        /// <summary>
        /// Open an image and load it into 2D array of colors (size: Height x Width)
        /// </summary>
        /// <param name="ImagePath">Image file path</param>
        /// <returns>2D array of colors</returns>
        public static RGBPixel[,] OpenImage(string ImagePath)
        {
            Bitmap original_bm = new Bitmap(ImagePath);
            int Height = original_bm.Height;
            int Width = original_bm.Width;

            RGBPixel[,] Buffer = new RGBPixel[Height, Width];

            unsafe
            {
                BitmapData bmd = original_bm.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, original_bm.PixelFormat);
                int x, y;
                int nWidth = 0;
                bool Format32 = false;
                bool Format24 = false;
                bool Format8 = false;

                if (original_bm.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    Format24 = true;
                    nWidth = Width * 3;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format32bppArgb || original_bm.PixelFormat == PixelFormat.Format32bppRgb || original_bm.PixelFormat == PixelFormat.Format32bppPArgb)
                {
                    Format32 = true;
                    nWidth = Width * 4;
                }
                else if (original_bm.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    Format8 = true;
                    nWidth = Width;
                }
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (y = 0; y < Height; y++)
                {
                    for (x = 0; x < Width; x++)
                    {
                        if (Format8)
                        {
                            Buffer[y, x].red = Buffer[y, x].green = Buffer[y, x].blue = p[0];
                            p++;
                        }
                        else
                        {
                            Buffer[y, x].red = p[2];
                            Buffer[y, x].green = p[1];
                            Buffer[y, x].blue = p[0];
                            if (Format24) p += 3;
                            else if (Format32) p += 4;
                        }
                    }
                    p += nOffset;
                }
                original_bm.UnlockBits(bmd);
            }

            return Buffer;
        }

        /// <summary>
        /// Get the height of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Height</returns>
        public static int GetHeight(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(0);
        }

        /// <summary>
        /// Get the width of the image 
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <returns>Image Width</returns>
        public static int GetWidth(RGBPixel[,] ImageMatrix)
        {
            return ImageMatrix.GetLength(1);
        }

        /// <summary>
        /// Display the given image on the given PictureBox object
        /// </summary>
        /// <param name="ImageMatrix">2D array that contains the image</param>
        /// <param name="PicBox">PictureBox object to display the image on it</param>
        public static void DisplayImage(RGBPixel[,] ImageMatrix, PictureBox PicBox)
        {
            // Create Image:
            //==============
            int Height = ImageMatrix.GetLength(0);
            int Width = ImageMatrix.GetLength(1);

            Bitmap ImageBMP = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

            unsafe
            {
                BitmapData bmd = ImageBMP.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadWrite, ImageBMP.PixelFormat);
                int nWidth = 0;
                nWidth = Width * 3;
                int nOffset = bmd.Stride - nWidth;
                byte* p = (byte*)bmd.Scan0;
                for (int i = 0; i < Height; i++)
                {
                    for (int j = 0; j < Width; j++)
                    {
                        p[2] = ImageMatrix[i, j].red;
                        p[1] = ImageMatrix[i, j].green;
                        p[0] = ImageMatrix[i, j].blue;
                        p += 3;
                    }

                    p += nOffset;
                }
                ImageBMP.UnlockBits(bmd);
            }
            PicBox.Image = ImageBMP;
        }

        public static string getPass(int TapPosition, ref string InitialSeed)
        {
            string result = "";
            string XOR;
            for (int i = 0; i < 8; i++)
            {
                XOR = InitialSeed[0] == InitialSeed[InitialSeed.Length - TapPosition - 1] ? "0" : "1";
                result += XOR;
                InitialSeed = InitialSeed.Substring(1) + XOR; // O(K)
            }
            return result;
        }

        public static RGBPixel[,] Encryption_Decryption(RGBPixel[,] ImageMatrix, int TapPosition, string InitialSeed)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);
            string result;
            RGBPixel[,] Filtered = new RGBPixel[Height, Width];

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    result = getPass(TapPosition, ref InitialSeed);
                    Filtered[i, j].red = (Byte)(ImageMatrix[i, j].red ^ Convert.ToByte(result, 2));

                    result = getPass(TapPosition, ref InitialSeed);
                    Filtered[i, j].green = (Byte)(ImageMatrix[i, j].green ^ Convert.ToByte(result, 2));

                    result = getPass(TapPosition, ref InitialSeed);
                    Filtered[i, j].blue = (Byte)(ImageMatrix[i, j].blue ^ Convert.ToByte(result, 2));
                }
            }
            return Filtered;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="node"></param>
        /// <param name="code"></param>
        /// <param name="codes"></param>
        public static void GenerateCode(ref double sum, Node node, string code, Dictionary<string,string> codes)
        {
            
            if(node == null)
            {
                return;
            }
            if(node.Data != "?")
            {
                codes[node.Data] = code;
                sum += code.Length * node.frequency;
            }
            GenerateCode(ref sum,node.left,code + "0", codes);
            GenerateCode(ref sum,node.right,code + "1", codes);
        }
        public class Node
        {
            public int frequency;
            public string Data;
            public Node left;
            public Node right;

            public Node(int freq, string data)
            {
                frequency = freq;
                Data = data;
                left = right = null;
            }
            public Node(int freq, string data, Node Left, Node Right)
            {
                frequency = freq;
                Data = data;
                left = Left;
                right = Right;
            }
        }

        public class PriorityQueue{
            List<Node> list;
            public int Size;

            public PriorityQueue()
            {
                list = new List<Node> { null };
                Size = 0;
            }

            public void Enqueue(Node node)
            {
                list.Add(node);
                Size++;
                list[Size].left = node.left;
                list[Size].right = node.right;

                int index = Size;
                int parent;
                Node tmp;
                while (true)
                {
                    parent = index / 2;
                    if(index <= 1)
                    {
                        return;
                    }
                    if(list[index].frequency < list[parent].frequency)
                    {
                        tmp = list[index];
                        list[index] = list[parent];
                        list[parent] = tmp;
                    }
                    index = parent;
                }
            }

            public Node ExtractMin()
            {
                Node extracted = list[1];
                list[1] = list[Size];
                list.RemoveAt(Size);
                Size--;
                int index = 1;
                int left, right, smallest = 0;
                Node tmp;
                while (true)
                {
                    left = index * 2;
                    right = index * 2 + 1;
                    if(left > Size)
                    {
                        return extracted;
                    }
                    else if(left == Size)
                    {
                        if(list[index].frequency > list[left].frequency)
                        {
                            tmp = list[index];
                            list[index] = list[left];
                            list[left] = tmp;
                        }
                        return extracted;
                    }
                    else
                    {
                        if(list[left].frequency > list[right].frequency)
                        {
                            smallest = right;
                        }
                        else
                        {
                            smallest = left;
                        }
                        if(list[index].frequency > list[smallest].frequency)
                        {
                            tmp = list[index];
                            list[index] = list[smallest];
                            list[smallest] = tmp;
                        }
                        index = smallest;
                    }
          
                }
                 return extracted;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <param name="InitialSeed"></param>
        /// <param name="TapPosition"></param>
        public static void Compression(RGBPixel[,] ImageMatrix, string InitialSeed, int TapPosition,string path)
        {
            int Height = GetHeight(ImageMatrix);
            int Width = GetWidth(ImageMatrix);
            Dictionary<string, int> redfreq = new Dictionary<string, int>();
            Dictionary<string, int> greenfreq = new Dictionary<string, int>();
            Dictionary<string, int> bluefreq = new Dictionary<string, int>();


            // Freq O(N^2)
            for(int i = 0; i < Height; i++)
            {
                for(int j = 0; j < Width; j++)
                {
                    if(!redfreq.ContainsKey(ImageMatrix[i, j].red.ToString()))
                    {
                        redfreq[ImageMatrix[i, j].red.ToString()] = 1;
                    }
                    else
                    {
                        redfreq[ImageMatrix[i, j].red.ToString()] += 1;
                    }

                    if (!greenfreq.ContainsKey(ImageMatrix[i, j].green.ToString()))
                    {
                        greenfreq[ImageMatrix[i, j].green.ToString()] = 1;
                    }
                    else
                    {
                        greenfreq[ImageMatrix[i, j].green.ToString()] += 1;
                    }

                    if (!bluefreq.ContainsKey(ImageMatrix[i, j].blue.ToString()))
                    {
                        bluefreq[ImageMatrix[i, j].blue.ToString()] = 1;
                    }
                    else
                    {
                        bluefreq[ImageMatrix[i, j].blue.ToString()] += 1;
                    }
                }
            }


            
            PriorityQueue redQueue = new PriorityQueue();
            PriorityQueue greenQueue = new PriorityQueue();
            PriorityQueue blueQueue = new PriorityQueue();
            // 3C
            foreach (var red in redfreq)
            {
                Node node = new Node(red.Value, red.Key);
                redQueue.Enqueue(node);
            }
            foreach (var green in greenfreq)
            {
                Node node = new Node(green.Value, green.Key);
                greenQueue.Enqueue(node);
            }
            foreach (var blue in bluefreq)
            {
                Node node = new Node(blue.Value, blue.Key);
                blueQueue.Enqueue(node);
            }

            /// 3 c log(c)
            while (redQueue.Size > 1)
            {
                Node right = redQueue.ExtractMin(); // log(c)
                Node left = redQueue.ExtractMin(); // log(c)
                Node node = new Node(left.frequency + right.frequency, "?", left, right);
                redQueue.Enqueue(node);

            }
            while (blueQueue.Size > 1)
            { 
                Node right = blueQueue.ExtractMin();
                Node left = blueQueue.ExtractMin();
                Node node = new Node(left.frequency + right.frequency, "?", left, right);
                blueQueue.Enqueue(node);

            }
            while (greenQueue.Size > 1)
            {
                
                Node right = greenQueue.ExtractMin();
                Node left = greenQueue.ExtractMin();
                Node node = new Node(left.frequency + right.frequency, "?", left, right);
                greenQueue.Enqueue(node);

            }

            Dictionary<string, string> redCodes = new Dictionary<string, string>();
            Dictionary<string, string> greenCodes = new Dictionary<string, string>();
            Dictionary<string, string> blueCodes = new Dictionary<string, string>();
            Node redRoot = redQueue.ExtractMin();
            Node blueRoot = blueQueue.ExtractMin();
            Node greenRoot = greenQueue.ExtractMin();

            double CompressionOutput = 0;
            GenerateCode(ref CompressionOutput,redRoot, "", redCodes);
            GenerateCode(ref CompressionOutput, greenRoot, "", greenCodes);
            GenerateCode(ref CompressionOutput, blueRoot, "", blueCodes);
            double ratio = ((double)(CompressionOutput / 8.0 * 100.0) / ((double)Height * (double)Width * 3.0));
            Console.WriteLine("The compression output: {0}  in bytes: {1} Ratio: {2}%", CompressionOutput, CompressionOutput / 8, Math.Round(ratio, 1));

            Save(ImageMatrix, InitialSeed, TapPosition, redCodes, greenCodes, blueCodes, Height, Width, path);
            


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ImageMatrix"></param>
        /// <param name="InitialSeed"></param>
        /// <param name="TapPosition"></param>
        /// <param name="redCodes"></param>
        /// <param name="greenCodes"></param>
        /// <param name="blueCodes"></param>
        /// <param name="Height"></param>
        /// <param name="Width"></param>
        public static void Save(RGBPixel[,] ImageMatrix, string InitialSeed, int TapPosition,
            Dictionary<string,string> redCodes, Dictionary<string, string> greenCodes,
            Dictionary<string, string> blueCodes,int Height, int Width, string path)
        {
            //string Path = @"G:\Algo Projects\MATERIALS\MATERIALS\[1] Image Encryption and Compression\Sample Test\Compressed\output.bin";
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(InitialSeed);
            bw.Write(TapPosition);

            bw.Write(redCodes.Count);
            foreach (var red in redCodes)
            {
                bw.Write(red.Value);
                bw.Write(red.Key);
            }

            bw.Write(greenCodes.Count);
            foreach (var green in greenCodes)
            {
                bw.Write(green.Value);
                bw.Write(green.Key);
            }

            bw.Write(blueCodes.Count);
            foreach (var blue in blueCodes)
            {
                bw.Write(blue.Value);
                bw.Write(blue.Key);
            }

            bw.Write(Height);
            bw.Write(Width);
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    bw.Write(redCodes[ImageMatrix[i, j].red.ToString()]);
                    bw.Write(greenCodes[ImageMatrix[i, j].green.ToString()]);
                    bw.Write(blueCodes[ImageMatrix[i, j].blue.ToString()]);
                }
            }

            bw.Close();
            fs.Close();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="redCodes"></param>
        /// <param name="greenCodes"></param>
        /// <param name="blueCodes"></param>
        /// <param name="InitialSeed"></param>
        /// <param name="TapPosition"></param>
        /// <param name="br"></param>
        public static void Load(ref Dictionary<string, string> redCodes, ref Dictionary<string, string> greenCodes, ref Dictionary<string, string> blueCodes, ref string InitialSeed, ref int TapPosition, BinaryReader br)
        {
            

            InitialSeed = br.ReadString();
            TapPosition = br.ReadInt32();

            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                redCodes[br.ReadString()] = br.ReadString();
            }
            count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                greenCodes[br.ReadString()] = br.ReadString();
            }
            count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                blueCodes[br.ReadString()] = br.ReadString();
            }

            
        }
        /// <summary>
        /// Load The Huffman Trees from the binary file, then Load The compressed image into image
        /// </summary>
        /// <param name="path"></param>
        /// <param name="InitialSeed"></param>
        /// <param name="TapPosition"></param>
        /// <returns></returns>
        public static RGBPixel[,] Load_Decompression(string path, ref string InitialSeed, ref int TapPosition)
        {
            Dictionary<string, string> redCodes = new Dictionary<string, string>();
            Dictionary<string, string> greenCodes = new Dictionary<string, string>();
            Dictionary<string, string> blueCodes = new Dictionary<string, string>();
            
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            Load(ref redCodes, ref greenCodes, ref blueCodes, ref InitialSeed, ref TapPosition, br);
            return Decompression(redCodes, greenCodes, blueCodes, fs, br);
        }
        /// <summary>
        /// Read the Compressed Image from the binary file and store it into Image
        /// </summary>
        /// <param name="redCodes">Dictionary to store the red Tree</param>
        /// <param name="greenCodes">Dictionary to store the green Tree</param>
        /// <param name="blueCodes">Dictionary to store the blue Tree</param>
        /// <param name="fs">FileStream with the path of the file that you read</param>
        /// <param name="br">BinaryReader</param>
        /// <returns></returns>
        public static RGBPixel[,] Decompression(Dictionary<string, string> redCodes, Dictionary<string, string> greenCodes, Dictionary<string, string> blueCodes, FileStream fs, BinaryReader br)
        {
         
            int Height = br.ReadInt32();
            int Width = br.ReadInt32();

            RGBPixel[,] ImageMatrix = new RGBPixel[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    ImageMatrix[i, j].red = byte.Parse(redCodes[br.ReadString()]);
                    ImageMatrix[i, j].green = byte.Parse(greenCodes[br.ReadString()]);
                    ImageMatrix[i, j].blue = byte.Parse(blueCodes[br.ReadString()]);
                }
            }
            br.Close();
            fs.Close();
            return ImageMatrix;
        }
        /// <summary>
        /// Apply Gaussian smoothing filter to enhance the edge detection 
        /// </summary>
        /// <param name="ImageMatrix">Colored image matrix</param>
        /// <param name="filterSize">Gaussian mask size</param>
        /// <param name="sigma">Gaussian sigma</param>
        /// <returns>smoothed color image</returns>
        /*public static RGBPixel[,] Encryption_Compression(RGBPixel[,] ImageMatrix, int TapPosition, string InitialSeed, string path)
        {
            RGBPixel[,] encrypted = Encryption_Decryption(ImageMatrix, TapPosition, InitialSeed);
            Compression(encrypted, InitialSeed, TapPosition, path);
            
            return encrypted;
        }*/


    }
}
