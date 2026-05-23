using System;
using System.Drawing;

namespace Lab2
{
    public class RectangleCreator : IShapeCreator
    {
        public string GetName()
        {
            return "Rectangle";
        }

        public Shape Create(Point start, Point end)
        {
            // Calculate position and size from two points
            int x = Math.Min(start.X, end.X);
            int y = Math.Min(start.Y, end.Y);
            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);

            // Ensure minimum visible size
            if (width < 5) width = 5;
            if (height < 5) height = 5;

            return new Rectangle(x, y, width, height);
        }
    }
}