using System;
using System.ComponentModel;
using System.Drawing;

class Program
{
    static void Main()
    {
        string welcome = "---CS4306 Bonus Project | Zachary Jackson---";
        Console.WriteLine(welcome);
        BMP bmp = new();
    }

    public class BMP
    {
        public BMP()
        {
            cX = GetFloat("CX");
            cY = GetFloat("CY");
            Size_X = GetInt("Size_X", 0);
            Size_Y = GetInt("Size_Y", 0);
            LoopCount = GetInt("LoopCount", 9);
            Dx = (Rect.X1 - Rect.X0) / Size_X;
            Dy = (Rect.Y1 - Rect.Y0) / Size_Y;
            coords = GetCoords(Rect);
            //f1 = new(this);
            //f1.Show();
        }
        public Rectangle Rect = new();
        //public Form1 f1;
        public float cX { get; private set; }
        public float cY { get; private set; }
        public int Size_X { get; private set; }
        public int Size_Y { get; private set; }
        public int LoopCount { get; private set; }
        public float Dx { get; private set; }
        public float Dy { get; private set; }

        public Coord[] coords;

        public class Rectangle
        {
            public Rectangle()
            {
                float x0 = GetFloat("X0");
                float y0 = GetFloat("Y0");
                float x1 = GetFloat("X1");
                float y1 = GetFloat("Y1");

                if (x0 > x1)
                {
                    float temp = x0;
                    x0 = x1;
                    x1 = temp;
                }
                if (y0 > y1)
                {
                    float temp = y0;
                    y0 = y1;
                    y1 = temp;
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


        }

        public class Coord
        {
            public float X, Y;
            public Coord(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

        /*public partial class Form1 : Form
        {
            //public Form1(BMP bmp)
            //{
            //    pb.Image = bmp.GenerateBitmap();
              //  this.Controls.Add(pb);
            //}
            private PictureBox pb = new PictureBox();
            protected override void OnLoad(EventArgs e)
            {
                base.OnLoad(e);
                pb.Show();
            }
        }*/

        private Coord[] GetCoords(Rectangle rect)
        {
            int len = Convert.ToInt32(((rect.X1 - rect.X0) / Dx) * ((rect.Y1 - rect.Y0) / Dy));
            Coord[] coords = new Coord[len];
            float x = rect.X0;
            float y = rect.Y0;
            int index = 0;
            while (x < rect.X1 || y < rect.Y1)
            {
                if (x <= rect.X1)
                {
                    x += Dx;
                }
                if (y <= rect.Y1)
                {
                    y += Dy;
                }
                float x_prime = (x + Size_X * Dx) * (x + Size_X * Dx) - (y + Size_Y * Dy) * (y + Size_Y * Dy) + cX;
                float y_prime = 2 * (x + Size_X * Dx) * (y + Size_Y * Dy) + cY;
                for (int i = 0; i < LoopCount; i++)
                {
                    x_prime = (x_prime + Size_X * Dx) * (x_prime + Size_X * Dx) - (y_prime + Size_Y * Dy) * (y_prime + Size_Y * Dy) + cX;
                    y_prime = 2 * ((y_prime + Size_X * Dx) * (y + Size_Y * Dy)) + cY;
                    coords[index++] = new Coord(x_prime, y_prime);
                }
            }

            return coords;
        }

        public System.Drawing.Bitmap GenerateBitmap()
        {

            Bitmap bmp = new Bitmap((int)Rect.X1, (int)Rect.Y1);
            System.Drawing.Rectangle rect = new(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpdata =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = bmpdata.Scan0;
            int bytes = Math.Abs(bmpdata.Stride) * bmp.Height;
            byte[] rgbVals = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbVals, 0, bytes);
            int index = 0;
            for (int i = 0; i < coords.Length; i++)
            {
                float xP = coords[i].X;
                float yP = coords[i].Y;
                rgbVals[index] = 255;
                switch (xP)
                {
                    case float.PositiveInfinity:
                        rgbVals[index + 1] = 255; break;
                    case var exp when xP > LoopCount:
                        rgbVals[index + 1] = 128; break;
                    case var exp when (xP < LoopCount && xP >= 1):
                        rgbVals[index + 1] = 64; break;
                    case var exp when (xP < 1 && xP >= 0):
                        rgbVals[index + 1] = 0; rgbVals[i + 3] = 0; break;
                    case float.NegativeInfinity:
                        rgbVals[index + 3] = 255; break;
                    case var exp when xP < (LoopCount * -1):
                        rgbVals[index + 3] = 128; break;
                    case var exp when (xP >= (LoopCount * -1) && xP <= -1):
                        rgbVals[index + 3] = 64; break;
                    case var exp when xP > -1:
                        rgbVals[index + 1] = 0; rgbVals[i + 2] = 0; break;
                }
                switch (yP)
                {
                    case float.PositiveInfinity:
                        rgbVals[index + 2] = 255; break;
                    case var exp when yP > LoopCount:
                        rgbVals[index + 2] = 200; break;
                    case var exp when (yP <= LoopCount && yP >= 1):
                        rgbVals[index + 2] = 160; break;
                    case var exp when (yP < 1 && yP >= 0):
                        rgbVals[index + 2] = 0; break;
                    case float.NegativeInfinity:
                        rgbVals[index + 2] = 128; break;
                    case var exp when yP < (LoopCount * -1):
                        rgbVals[index + 2] = 90; break;
                    case var exp when (yP > (LoopCount * -1) && yP <= -1):
                        rgbVals[index + 2] = 64; break;
                    case var exp when yP > -1:
                        rgbVals[index + 2] = 0; break;

                }
                index += 4;
            }
            System.Runtime.InteropServices.Marshal.Copy(rgbVals, 0, ptr, bytes);
            bmp.UnlockBits(bmpdata);
            return bmp;
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
    }

}
