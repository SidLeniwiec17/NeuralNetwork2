using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MSI2.Helpers;

namespace MSI2.Content
{
    public class FaceHelper
    {
        //flag - true ; jezeli wgrywamy zbior uczacy
        //flag - false ; jezeli wgrywamy zbior testowy
        public static List<Face> ConvertFaces(List<List<string>> pictures, bool flag)
        {
            List<Face> faces = new List<Face>();

            for (int c = 0; c < pictures.Count(); c++)
            {
                Console.WriteLine("Folder nr " + c);
                //for (int i = 0; i < pictures[c].Count(); i++)
                Parallel.For(0, pictures[c].Count(), i =>
                {
                    List<int> gradients = new List<int>();
                    string _class = "UNKNOWN";
                    int index = -1;
                    Bitmap picture = GrayScale(new Bitmap(pictures[c][i]));
                    gradients = GetGradient(picture);
                    if (flag == true)
                    {
                        switch (c)
                        {
                            case 0:
                                _class = "BK";
                                index = 0;
                                break;
                            case 1:
                                _class = "BM";
                                index = 1;
                                break;
                            case 2:
                                _class = "LK";
                                index = 2;
                                break;
                            case 3:
                                _class = "LM";
                                index = 3;
                                break;
                        }
                    }
                    if (flag == true)
                    {
                        Face tempFace = new Face(_class, gradients, index);
                        if (tempFace.Validate() == true)
                            faces.Add(tempFace);
                        else
                        {
                            Console.WriteLine("ERROR !?");
                        }
                    }
                    else
                    {
                        Face tempFace = new Face(_class, gradients, pictures[c][i], index);
                        if (tempFace.Validate() == true)
                            faces.Add(tempFace);
                        else
                        {
                            Console.WriteLine("ERROR !?");
                        }
                    }
                });
            }
            return faces;
        }

        public static List<int> GetGradient(Bitmap picture)
        {
            List<int> grad = new List<int>();
            int[,] smallGradients = new int[picture.Width - 2, picture.Height - 2];

            for (int y = 1; y < picture.Height - 1; y++)
            {
                for (int x = 1; x < picture.Width - 1; x++)
                {
                    Color[,] neighbours = new Color[3, 3];
                    neighbours[0, 0] = picture.GetPixel(x - 1, y - 1);
                    neighbours[1, 0] = picture.GetPixel(x, y - 1);
                    neighbours[2, 0] = picture.GetPixel(x + 1, y - 1);
                    neighbours[0, 1] = picture.GetPixel(x - 1, y);
                    neighbours[1, 1] = picture.GetPixel(x, y);
                    neighbours[2, 1] = picture.GetPixel(x + 1, y);
                    neighbours[0, 2] = picture.GetPixel(x - 1, y + 1);
                    neighbours[1, 2] = picture.GetPixel(x, y + 1);
                    neighbours[2, 2] = picture.GetPixel(x + 1, y + 1);
                    //GRADIENT
                    smallGradients[x - 1, y - 1] = GetDirection(neighbours);
                }
            }

            grad = SmallGradientToBigGradient(smallGradients, picture.Width - 2, picture.Height - 2);

            return grad;
        }

        public static List<int> SmallGradientToBigGradient(int[,] smallGradient, int width, int height)
        {
            //int gX = 2;
            //int gY = 3;
            int gX = 20;
            int gY = 25;
            int dX = (width / gX);
            int dY = (height / gY);
            List<int> gradient = new List<int>();
            double[,] values = new double[gX, gY];
            for (int i = 0; i < gX; i++)
                for (int j = 0; j < gY; j++)
                    values[i, j] = 0;

            int[,] numbers = new int[gX, gY];

            for (int y = 0; y < gY * dY; y++)
            {
                for (int x = 0; x < gX * dX; x++)
                {
                    values[x / dX, y / dY] = values[x / dX, y / dY] + (double)smallGradient[x, y];
                    numbers[x / dX, y / dY] = numbers[x / dX, y / dY] + 1;
                }
            }

            for (int y = 0; y < gY; y++)
            {
                for (int x = 0; x < gX; x++)
                {
                    gradient.Add((int)Math.Round((values[x, y] / (double)numbers[x, y])));
                }
            }

            return gradient;
        }

        /*
         * directions
         * 8 1 2
         * 7   3
         * 6 5 4
         * 
         * a moze ??
         * 
         * 4 1 2
         * 3   3
         * 2 1 4
         * */
        public static int GetDirection(Color[,] pixels)
        {
            int direction = -1;
            int maxValue = -255;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    if (!(x == 1 && y == 1))
                    {
                        if (pixels[x, y].R >= maxValue)
                        {
                            maxValue = pixels[x, y].R;
                            maxX = x;
                            maxY = y;
                        }
                    }
                }
            }

            int tempDir = (maxY * 3) + maxX;
            if (pixels[1, 1].R == 255 && maxValue == 255)
            {
                direction = 0;
            }

            else
            {
                direction = DirectionTranslator.TranslateDirection(tempDir);
            }
            return direction;
        }

        public static Bitmap GrayScale(Bitmap color)
        {
            Bitmap gray = new Bitmap(color.Width, color.Height);

            for (int i = 0; i < color.Width; i++)
            {
                for (int x = 0; x < color.Height; x++)
                {
                    Color oc = color.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    gray.SetPixel(i, x, nc);
                }
            }
            return gray;
        }
    }
}

