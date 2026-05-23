using System;
using System.Drawing;

namespace Lab2
{
    public class SquareCreator : IShapeCreator
    {
        public string GetName()
        {
            return "Square";
        }

        public Shape Create(Point start, Point end)
        {
            // Square uses the smaller dimension to maintain equal sides
            int x = Math.Min(start.X, end.X);
            int y = Math.Min(start.Y, end.Y);
            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);
            int side = Math.Min(width, height);

            if (side < 5) side = 5;

            return new Square(x, y, side);
        }
    }
}