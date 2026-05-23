using System.Drawing;

namespace Lab2
{
    public class Triangle : Shape
    {
        public Point[] Points;

        public Triangle(Point p1, Point p2, Point p3)
        {
            Name = "Triangle";
            Points = new Point[] { p1, p2, p3 };
        }
    }
}