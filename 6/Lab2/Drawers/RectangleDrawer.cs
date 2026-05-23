using System.Drawing;

namespace Lab2
{
    public class RectangleDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            Rectangle rect = (Rectangle)shape;
            Pen pen = new Pen(rect.Color, 2);
            g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
            pen.Dispose();
        }
    }
}