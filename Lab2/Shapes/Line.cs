using System.Drawing;

namespace Lab2
{
    public class Line : Shape
    {
        public Point Start { get; set; }
        public Point End { get; set; }

        // Parameterless constructor (required for XML serialization)
        public Line()
        {
            Name = "Line";
            Start = new Point(0, 0);
            End = new Point(0, 0);
        }

        public Line(Point start, Point end)
        {
            Name = "Line";
            Start = start;
            End = end;
        }
    }
}