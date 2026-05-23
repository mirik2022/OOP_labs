using System.Drawing;

namespace Lab2
{
    public class SquareDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            Square square = (Square)shape;
            Pen pen = new Pen(square.Color, 2);
            g.DrawRectangle(pen, square.X, square.Y, square.Side, square.Side);
            pen.Dispose();
        }
    }
}