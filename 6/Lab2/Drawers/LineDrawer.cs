using System.Drawing;

namespace Lab2
{
    public class LineDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            Line line = (Line)shape;
            Pen pen = new Pen(line.Color, 5);
            g.DrawLine(pen, line.Start, line.End);
            pen.Dispose();
        }
    }
}