using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    static void Main(string[] args)
    {
        Application.EnableVisualStyles();
        Console.WriteLine("---CS4306 Bonus Project | Zachary Jackson---");
        BMP bmp = new();
        Console.ReadLine();
    }

    public class BMP
    {
        public BMP()
        {
            if(Rect.defaultsEnabled)
            {
                cX = .1f;
                cY = .2f;
                Size_X = 1024;
                Size_Y = 1024;
                LoopCount = 12;
            }
            else
            {
                cX = GetFloat("CX");
                cY = GetFloat("CY");
                Size_X = GetInt("Size_X", 0);
                Size_Y = GetInt("Size_Y", 0);
                LoopCount = GetInt("LoopCount", 9);
            }
            dX = (Rect.X1 - Rect.X0) / Size_X;
            dY = (Rect.Y1 - Rect.Y0) / Size_Y;
            coords = GetCoords(Rect);
            Task.Run(() =>
            {
                f1 = new Form1(this);
                Application.Run(f1);
            });
        }
        public Rectangle Rect = new();
        public Form1 f1;
        public float cX { get; private set; }
        public float cY { get; private set; }
        public int Size_X { get; private set; }
        public int Size_Y { get; private set; }
        public int LoopCount { get; private set; }
        public float dX { get; private set; }
        public float dY { get; private set; }

        public Coord[] coords;

        public class Rectangle
        {
            public Rectangle()
            {
                float x0, y0, x1, y1;
                if(useDefaults())
                {
                    defaultsEnabled = true;
                    x0 = -1.0f;
                    y0 = -1.0f;
                    x1 = 1.0f;
                    y1 = 1.0f;
                }
                else
                {
                    defaultsEnabled = false;
                    x0 = GetFloat("X0");
                    y0 = GetFloat("Y0");
                    x1 = GetFloat("X1");
                    y1 = GetFloat("Y1");
                    float temp;

                    if (x0 > x1)
                    {
                        temp = x0;
                        x0 = x1;
                        x1 = temp;
                    }
                    if (y0 > y1)
                    {
                        temp = y0;
                        y0 = y1;
                        y1 = temp;
                    }

                }
                 X0 = x0;
                 Y0 = y0;
                 X1 = x1;
                 Y1 = y1;

            }

            public float X0 { get; private set; }
            public float Y0 { get; private set; }
            public float X1 { get; private set; }
            public float Y1 { get; private set; }
            public bool defaultsEnabled { get; private set; }


        }

        public class Coord
        {
            public int X, Y;
            public float X_Prime, Y_Prime;
            
            public Coord(int x, int y, float x_p, float y_p)
            {
                X = x;
                Y = y;
                X_Prime= x_p;
                Y_Prime= y_p;
            }
        }

        public partial class Form1 : Form
        {
            public Form1(BMP bmp)
            {
                Width = 1000;
                Height = 1000;
                img.Height = bmp.Size_X;
                img.Width = bmp.Size_Y;
                img.BackColor = Color.White;
                Controls.Add(img);
                Bmp = bmp;
            }
            private PictureBox img = new PictureBox();
            private BMP Bmp;
            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                img.Paint += (s, ev) =>
                {
                    Bmp.GenerateBitmap(ev.Graphics);
                };
            }

        }

        private Coord[] GetCoords(Rectangle rect)
        {
            int index = 0;
            int len = Convert.ToInt32(Size_X * Size_Y);
            Coord[] coords = new Coord[len];
            float x0 = rect.X0;
            float y0 = rect.Y0;
            for (int m = 0; m < Size_X; m++)
            {
                for (int n = 0; n < Size_Y; n++)
                {
                    float c1 = x0 + m * dX;
                    float c2 = y0 + n * dY;
                    float x_prime = (c1 * c1) - (c2 * c2) + cX;
                    float y_prime = (2 * (c1 * c2)) + cY;
                    for (int i = 0; i < LoopCount; i++)
                    {
                        x_prime = (x_prime + m * dX) * (x_prime + m * dX) - (y_prime + n * dY) * (y_prime + n * dY) + cX;
                        y_prime = 2 * (y_prime + m * dX) * c2 + cY;
                    }
                    coords[index++] = new Coord(m, n, x_prime, y_prime);
                }
            }
            return coords;
        }

        public void GenerateBitmap(Graphics gr)
        {
            Bitmap bmp = new Bitmap(Size_X, Size_Y);
            
            for(int i = 0; i < coords.Length; i++)
            {
                float xP = coords[i].X_Prime;
                float yP = coords[i].Y_Prime;
                int r = 0; int g = 0; int b = 0;
                switch (xP)
                {
                    case float.PositiveInfinity:
                        r = 255; break;
                    case var exp when xP > LoopCount:
                        r = 128; break;
                    case var exp when (xP <= LoopCount && xP >= 0.001):
                        r = 64; break;
                    case var exp when (xP < 0.001 && xP >= 0):
                        b = 0; r = 0; break;
                    case float.NegativeInfinity:
                        b = 255; break;
                    case var exp when xP < (LoopCount * -1):
                        b = 128; break;
                    case var exp when (xP >= (LoopCount * -1) && xP <= -0.001):
                        b = 64; break;
                    case var exp when xP > -0.001:
                        r = 0; b = 0; break;
                }
                switch (yP)
                {
                    case float.PositiveInfinity:
                        g = 255; break;
                    case var exp when yP > LoopCount:
                        g = 200; break;
                    case var exp when (yP <= LoopCount && yP >= 0.001):
                        g = 160; break;
                    case var exp when (yP < 0.001 && yP >= 0):
                        g = 0; break;
                    case float.NegativeInfinity:
                        g = 128; break;
                    case var exp when yP < (LoopCount * -1):
                        g = 90; break;
                    case var exp when (yP >= (LoopCount * -1) && yP <= -0.001):
                        g = 64; break;
                    case var exp when yP > -0.001:
                        g = 0; break;

                }
                Color color = Color.FromArgb(r, g, b);
                SolidBrush brush = new SolidBrush(color);
                bmp.SetPixel(coords[i].X, coords[i].Y, color);             
            }
            gr.DrawImage(bmp, 0, 0);
            bmp.Save(Directory.GetCurrentDirectory() + "\\img01.png", ImageFormat.Png);
        }

        private int GetInt(string name, int constraint)
        {
            Console.WriteLine("Please enter a value for " + name);
            int parsed;
            int.TryParse(Console.ReadLine(), out parsed);
            if (parsed > constraint)
            {
                return parsed;
            }
            Console.WriteLine("Invalid entry, must be a non-negative integer greater than " + constraint.ToString());
            return GetInt(name, constraint);
        }
        private static float GetFloat(string name)
        {
            Console.WriteLine("Please enter a value for " + name);
            if (float.TryParse(Console.ReadLine(), out float value))
            {
                return value;
            }
            Console.WriteLine("Invalid entry for " + name + ", must be a floating point number");
            return GetFloat(name);
        }
        private static bool useDefaults()
        {
            Console.WriteLine("Use default values for visualization (Y/N)?");
            if (Console.ReadLine().ToLower() == "y")
            {
                return true;
            }
            return false;
        }
    }

}
