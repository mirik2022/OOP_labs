using System.Drawing;

namespace Lab2
{
    public class Ellipse : Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        // Parameterless constructor (required for XML serialization)
        public Ellipse()
        {
            Name = "Ellipse";
            X = 0; Y = 0; Width = 0; Height = 0;
        }

        public Ellipse(int x, int y, int width, int height)
        {
            Name = "Ellipse";
            X = x; Y = y; Width = width; Height = height;
        }
    }
}