using System.Drawing;

namespace Lab2
{
    public class Line : Shape
    {
        public Point Start;
        public Point End;

        public Line(Point start, Point end)
        {
            Name = "Line";
            Start = start;
            End = end;
        }
    }
}