using Lab2;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace StarPlugin
{
    [DataContract]
    public class Star : Shape
    {
        [DataMember] public Point[] Points { get; private set; }
        [DataMember] public Point Center { get; private set; }
        [DataMember] public int Radius { get; private set; }

        // Parameterless constructor for serialization
        public Star()
        {
            Name = "Star";
            Points = new Point[0];
        }

        public Star(Point center, int radius)
        {
            Name = "Star";
            Center = center;
            Radius = radius;
            Points = CalculateStarPoints();
        }

        private Point[] CalculateStarPoints()
        {
            Point[] points = new Point[10];
            double angle = -Math.PI / 2;
            int innerRadius = Radius / 2;

            for (int i = 0; i < 10; i++)
            {
                double r = (i % 2 == 0) ? Radius : innerRadius;
                int x = Center.X + (int)(Math.Cos(angle) * r);
                int y = Center.Y + (int)(Math.Sin(angle) * r);
                points[i] = new Point(x, y);
                angle += Math.PI / 5;
            }

            return points;
        }
    }

    public class StarCreator : IShapeCreator
    {
        public string GetName() => "Star";

        public Shape Create(Point start, Point end)
        {
            int radius = Math.Abs(end.X - start.X) / 2;
            if (radius < 10) radius = 10;

            Point center = new Point(
                (start.X + end.X) / 2,
                (start.Y + end.Y) / 2
            );

            return new Star(center, radius);
        }
    }

    public class StarDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            Star star = (Star)shape;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(star.Color, 3))
            {
                g.DrawPolygon(pen, star.Points);
            }
        }
    }

    /// <summary>
    /// Signed plugin class with expiration and signature support
    /// </summary>
    public class StarPlugin : ISignedShapePlugin
    {
        // This signature will be replaced by the signing tool
        // The actual signature is stored in the .sig file
        private static string _signature = "";

        public string GetShapeName() => "Star";

        public IShapeCreator GetCreator() => new StarCreator();

        public IShapeDrawer GetDrawer() => new StarDrawer();

        /// <summary>
        /// Returns the digital signature as Base64 string
        /// </summary>
        public string GetSignatureBase64()
        {
            // In production, this would be embedded as a resource
            // For now, it's loaded from the .sig file
            return _signature;
        }

        /// <summary>
        /// Set signature (called by the main program)
        /// </summary>
        public static void SetSignature(string signature)
        {
            _signature = signature;
        }

        /// <summary>
        /// Plugin expires after 1 year from compilation
        /// </summary>
        public DateTime? GetExpirationDate()
        {
            // Expires 1 year from now
            return DateTime.Now.AddYears(1);
        }

        /// <summary>
        /// Plugin author information
        /// </summary>
        public string GetAuthor()
        {
            return "Student Name";
        }

        /// <summary>
        /// Plugin version
        /// </summary>
        public string GetVersion()
        {
            return "1.0.0";
        }
    }
}