using System.Drawing;

namespace Lab2
{
    public class LineCreator : IShapeCreator
    {
        public string GetName()
        {
            return "Line";
        }

        public Shape Create(Point start, Point end)
        {
            return new Line(start, end);
        }
    }
}