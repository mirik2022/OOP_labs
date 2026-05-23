using System.Drawing;

namespace Lab2
{
    public class Square : Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Side { get; set; }

        // Parameterless constructor (required for XML serialization)
        public Square()
        {
            Name = "Square";
            X = 0; Y = 0; Side = 0;
        }

        public Square(int x, int y, int side)
        {
            Name = "Square";
            X = x; Y = y; Side = side;
        }
    }
}