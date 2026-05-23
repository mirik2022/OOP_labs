using System.Drawing;

namespace Lab2
{
    public class Rectangle : Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        // Parameterless constructor (required for XML serialization)
        public Rectangle()
        {
            Name = "Rectangle";
            X = 0; Y = 0; Width = 0; Height = 0;
        }

        public Rectangle(int x, int y, int width, int height)
        {
            Name = "Rectangle";
            X = x; Y = y; Width = width; Height = height;
        }
    }
}