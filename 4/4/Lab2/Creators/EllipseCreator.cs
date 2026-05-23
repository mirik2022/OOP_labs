using System;
using System.Drawing;

namespace Lab2
{
    public class EllipseCreator : IShapeCreator
    {
        public string GetName()
        {
            return "Ellipse";
        }

        public Shape Create(Point start, Point end)
        {
            int x = Math.Min(start.X, end.X);
            int y = Math.Min(start.Y, end.Y);
            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);

            if (width < 5) width = 5;
            if (height < 5) height = 5;

            return new Ellipse(x, y, width, height);
        }
    }
}