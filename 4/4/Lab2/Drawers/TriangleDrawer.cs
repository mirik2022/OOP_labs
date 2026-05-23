using System.Drawing;

namespace Lab2
{
    public class TriangleDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            Triangle triangle = (Triangle)shape;
            Pen pen = new Pen(triangle.Color, 2);
            g.DrawPolygon(pen, triangle.Points);
            pen.Dispose();
        }
    }
}