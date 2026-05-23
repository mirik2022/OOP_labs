using System.Drawing;

namespace Lab2
{
    public class EllipseDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            Ellipse ellipse = (Ellipse)shape;
            Pen pen = new Pen(ellipse.Color, 5);
            g.DrawEllipse(pen, ellipse.X, ellipse.Y, ellipse.Width, ellipse.Height);
            pen.Dispose();
        }
    }
}