using System.Drawing;

namespace Lab2
{
    public class Circle : Shape
    {
        public int X;
        public int Y;
        public int Diameter;

        public Circle(int x, int y, int diameter)
        {
            Name = "Circle";
            X = x; Y = y; Diameter = diameter;
        }
    }
}