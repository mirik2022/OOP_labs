using System.Drawing;

namespace Lab2
{
    public class Ellipse : Shape
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Ellipse(int x, int y, int width, int height)
        {
            Name = "Ellipse";
            X = x; Y = y; Width = width; Height = height;
        }
    }
}