using System.Drawing;

namespace Lab2
{
    public class Square : Shape
    {
        public int X;
        public int Y;
        public int Side;

        public Square(int x, int y, int side)
        {
            Name = "Square";
            X = x; Y = y; Side = side;
        }
    }
}