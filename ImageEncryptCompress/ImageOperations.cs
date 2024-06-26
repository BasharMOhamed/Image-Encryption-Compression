using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
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
            string XOR;
            string result = "";
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
            // 3 C log(C)
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
            double comp = CompressionOutput / 8;
            Console.WriteLine("The compression output: {0}  in bytes: {1} Ratio: {2}%", CompressionOutput,comp, Math.Round(ratio, 1));
            byte bits = Convert.ToByte((comp - Math.Floor(comp)) * 8);

            
            
            Save(ImageMatrix, InitialSeed, TapPosition, redfreq, greenfreq, bluefreq, Height, Width, path, redCodes, greenCodes, blueCodes, bits) ;


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
            Dictionary<string, int> redfreq, Dictionary<string, int> greenfrq,
            Dictionary<string, int> bluefreq, int Height, int Width, string path, Dictionary<string, string> redCodes,
            Dictionary<string, string> greenCodes, Dictionary<string, string> blueCodes, byte bits)
        {
            //string Path = @"G:\Algo Projects\MATERIALS\MATERIALS\[1] Image Encryption and Compression\Sample Test\Compressed\output.bin";
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(InitialSeed);
            bw.Write(TapPosition);
            
            bw.Write(Convert.ToByte(redfreq.Count - 1));
            
            // 3 C
            foreach (var red in redfreq)
            {
                bw.Write(red.Value);
                bw.Write(Convert.ToByte(int.Parse(red.Key)));
            }

            bw.Write(Convert.ToByte(greenfrq.Count - 1));
            foreach (var green in greenfrq)
            {
                //bw.Write(Convert.ToByte(green.Value ,2));
                bw.Write(green.Value);
                bw.Write(Convert.ToByte(int.Parse(green.Key)));
            }

            bw.Write(Convert.ToByte(bluefreq.Count -1));
            foreach (var blue in bluefreq)
            {
                bw.Write(blue.Value);
                bw.Write(Convert.ToByte(int.Parse(blue.Key)));
            }



            bw.Write(Height);
            bw.Write(Width);
            bw.Write(bits);
            string value, buffer = "";
            
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    value = redCodes[ImageMatrix[i, j].red.ToString()];
                    for (int k=0;k< value.Length; k++)
                    {
                        buffer += value[k];
                        if(buffer.Length == 8)
                        {
                            bw.Write(Convert.ToByte(buffer , 2));
                            buffer = "";
                        }
                    }
                    value = greenCodes[ImageMatrix[i, j].green.ToString()];
                    for (int k = 0; k < value.Length; k++)
                    {
                        buffer += value[k];
                        if (buffer.Length == 8)
                        {
                            bw.Write(Convert.ToByte(buffer, 2));
                            buffer = "";
                        }
                    }
                    value = blueCodes[ImageMatrix[i, j].blue.ToString()];
                    for (int k = 0; k < value.Length; k++)
                    {
                        buffer += value[k];
                        if (buffer.Length == 8)
                        {
                            bw.Write(Convert.ToByte(buffer, 2));
                            buffer = "";
                        }
                    }
                }
            }
            if(buffer.Length > 0)
            {
                bw.Write(Convert.ToByte(buffer, 2));
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
        public static List<Node> Load(ref string InitialSeed, ref int TapPosition, string path,ref string compressedImage,List<string> byteBlock, ref int Height, ref int Width)
        {

            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            PriorityQueue redQueue = new PriorityQueue();
            PriorityQueue greenQueue = new PriorityQueue();
            PriorityQueue blueQueue = new PriorityQueue();

            InitialSeed = br.ReadString();
            TapPosition = br.ReadInt32();


            // 3 C log(C)
            short count = Convert.ToInt16(br.ReadByte());
            for (short i = 0; i <= count; i++)
            {
                Node node = new Node(br.ReadInt32(), br.ReadByte().ToString());
                redQueue.Enqueue(node);
            }


            count = Convert.ToInt16(br.ReadByte());
            for (short i = 0; i <= count; i++)
            {
                Node node = new Node(br.ReadInt32(), br.ReadByte().ToString());
                greenQueue.Enqueue(node);
            }


            count = Convert.ToInt16(br.ReadByte());
            for (short i = 0; i <= count; i++)
            {
                Node node = new Node(br.ReadInt32(), br.ReadByte().ToString());
                blueQueue.Enqueue(node);
            }


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

            Height = br.ReadInt32();
            Width = br.ReadInt32();
            byte bits = br.ReadByte();

            StringBuilder sb = new StringBuilder();
            long remainingLength = br.BaseStream.Length - br.BaseStream.Position;

            for (long i = remainingLength -1; i >= remainingLength / 2; i--)
            {
                sb.Append(Convert.ToString(br.ReadByte(), 2).PadLeft(8, '0'));
            }
            byteBlock.Add(sb.ToString());
            sb.Clear();


            for(long i = (remainingLength / 2) - 1; i >= 0; i--)
            {
                sb.Append(Convert.ToString(br.ReadByte(), 2).PadLeft(8, '0'));
            }
            
            int start = sb.Length - 8;
            int length = 8 - bits;
            if (bits != 0)
            {
                sb.Remove(start, length);
            }
            byteBlock.Add(sb.ToString());
            br.Close();
            fs.Close();


            List<Node> list = new List<Node>();
            list.Add(redQueue.ExtractMin());
            list.Add(greenQueue.ExtractMin());
            list.Add(blueQueue.ExtractMin());
            return list;
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
            string compressedImage = "";
            int Height = 0;
            int Width = 0;
            byte[] bytes = new byte[1];
            List<string> byteBlock = new List<string>();
            List<Node> list = Load(ref InitialSeed, ref TapPosition, path, ref compressedImage, byteBlock, ref Height, ref Width);
            

            return Decompression(list[0], list[1], list[2], compressedImage, Height, Width, byteBlock);
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
        public static RGBPixel[,] Decompression(Node redRoot, Node greenRoot, Node blueRoot, string compressedImage, int Height, int Width, List<string> byteBlock)
        {
            int index = 0;
            RGBPixel[,] ImageMatrix = new RGBPixel[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    
                    ImageMatrix[i, j].red = getFromTree(redRoot, compressedImage, ref index, byteBlock);
                    ImageMatrix[i, j].green = getFromTree(greenRoot, compressedImage, ref index, byteBlock);
                    ImageMatrix[i, j].blue = getFromTree(blueRoot, compressedImage, ref index, byteBlock);

                }
            }


            return ImageMatrix;
        }

        public static byte getFromTree(Node ptr, string compressedImage,ref int index, List<string> byteBlock)
        {

            while (index < byteBlock[0].Length + byteBlock[1].Length)
            {
                
                if (byteBlock[index / byteBlock[0].Length][index % byteBlock[0].Length] == '1')
                {
                    if (ptr.right == null)
                    {
                        break;
                    }
                    ptr = ptr.right;
                }
                else
                {
                    if (ptr.left == null)
                    {
                        break;
                    }
                    ptr = ptr.left;
                }
                index++;
            }

            return byte.Parse(ptr.Data);
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
