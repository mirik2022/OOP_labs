using System;
using System.Drawing;

namespace Lab2
{
    public class CircleCreator : IShapeCreator
    {
        public string GetName()
        {
            return "Circle";
        }

        public Shape Create(Point start, Point end)
        {
            int x = Math.Min(start.X, end.X);
            int y = Math.Min(start.Y, end.Y);
            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);
            int diameter = Math.Min(width, height);

            if (diameter < 5) diameter = 5;

            return new Circle(x, y, diameter);
        }
    }
}