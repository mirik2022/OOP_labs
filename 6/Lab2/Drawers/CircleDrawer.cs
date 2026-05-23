using System.Drawing;

namespace Lab2
{
    public class CircleDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            Circle circle = (Circle)shape;
            Pen pen = new Pen(circle.Color, 5);
            g.DrawEllipse(pen, circle.X, circle.Y, circle.Diameter, circle.Diameter);
            pen.Dispose();
        }
    }
}