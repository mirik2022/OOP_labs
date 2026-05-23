using System.Drawing;

namespace Lab2
{
    public class Circle : Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Diameter { get; set; }

        // Parameterless constructor (required for XML serialization)
        public Circle()
        {
            Name = "Circle";
            X = 0; Y = 0; Diameter = 0;
        }

        public Circle(int x, int y, int diameter)
        {
            Name = "Circle";
            X = x; Y = y; Diameter = diameter;
        }
    }
}