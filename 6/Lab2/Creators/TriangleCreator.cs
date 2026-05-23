using System;
using System.Drawing;

namespace Lab2
{
    public class TriangleCreator : IShapeCreator
    {
        public string GetName()
        {
            return "Triangle";
        }

        public Shape Create(Point start, Point end)
        {
            int x = Math.Min(start.X, end.X);
            int y = Math.Min(start.Y, end.Y);
            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);

            if (width < 5) width = 5;
            if (height < 5) height = 5;

            // Create an isosceles triangle pointing up
            Point p1 = new Point(x + width / 2, y);      // Top point
            Point p2 = new Point(x, y + height);          // Bottom left
            Point p3 = new Point(x + width, y + height);  // Bottom right

            return new Triangle(p1, p2, p3);
        }
    }
}