using System.Drawing;

namespace Lab2
{
    public class Rectangle : Shape
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rectangle(int x, int y, int width, int height)
        {
            Name = "Rectangle";
            X = x; Y = y; Width = width; Height = height;
        }
    }
}