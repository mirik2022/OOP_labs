using Lab2;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.Serialization;

namespace HexagonPlugin
{
    [DataContract]
    public class Hexagon : Shape
    {
        [DataMember] public Point[] Points { get; private set; }
        [DataMember] public Point Center { get; private set; }
        [DataMember] public int Radius { get; private set; }

        public Hexagon()
        {
            Name = "Hexagon";
            Points = new Point[0];
        }

        public Hexagon(Point center, int radius)
        {
            Name = "Hexagon";
            Center = center;
            Radius = radius;
            Points = CalculateHexagonPoints();
        }

        /// <summary>
        /// Calculate the 6 vertices of a regular hexagon
        /// </summary>
        private Point[] CalculateHexagonPoints()
        {
            Point[] points = new Point[6];

            // Start from top (12 o'clock) and go clockwise
            for (int i = 0; i < 6; i++)
            {
                // 60 degrees between vertices, start at -90 degrees (top)
                double angle = i * 60 * Math.PI / 180 - Math.PI / 2;
                int x = Center.X + (int)(Math.Cos(angle) * Radius);
                int y = Center.Y + (int)(Math.Sin(angle) * Radius);
                points[i] = new Point(x, y);
            }

            return points;
        }
    }

    /// <summary>
    /// Creator for Hexagon shape
    /// </summary>
    public class HexagonCreator : IShapeCreator
    {
        public string GetName()
        {
            return "Hexagon";
        }

        public Shape Create(Point start, Point end)
        {
            // Calculate radius from drag distance
            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);
            int diameter = Math.Min(width, height);
            int radius = diameter / 2;

            // Minimum size to be visible
            if (radius < 10) radius = 10;

            // Center of the hexagon
            Point center = new Point(
                (start.X + end.X) / 2,
                (start.Y + end.Y) / 2
            );

            return new Hexagon(center, radius);
        }
    }

    /// <summary>
    /// Drawer for Hexagon shape
    /// </summary>
    public class HexagonDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            Hexagon hexagon = (Hexagon)shape;

            // Enable anti-aliasing for smooth lines
            g.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(hexagon.Color, 3))
            {
                g.DrawPolygon(pen, hexagon.Points);
            }
        }
    }

    /// <summary>
    /// Main plugin class that exposes the hexagon to the main application
    /// Implements both IShapePlugin and ISignedShapePlugin for full functionality
    /// </summary>
    public class HexagonPlugin : ISignedShapePlugin
    {
        // Regular IShapePlugin methods
        public string GetShapeName()
        {
            return "Hexagon";
        }

        public IShapeCreator GetCreator()
        {
            return new HexagonCreator();
        }

        public IShapeDrawer GetDrawer()
        {
            return new HexagonDrawer();
        }

        // ISignedShapePlugin methods for digital signature support
        /// <summary>
        /// Returns the digital signature (loaded from .sig file)
        /// The actual signature is stored separately and injected by the main program
        /// </summary>
        public string GetSignatureBase64()
        {
            // This will be populated by the PluginManager when loading
            // For unsigned plugins, return empty string
            return "";
        }

        /// <summary>
        /// Plugin expires after 6 months from compilation
        /// Different from Star's 1 year to demonstrate flexibility
        /// </summary>
        public DateTime? GetExpirationDate()
        {
            // Returns expiration date 180 days from now
            return DateTime.Now.AddMonths(6);
        }

        /// <summary>
        /// Plugin author information for verification
        /// </summary>
        public string GetAuthor()
        {
            return "Student Name";
        }

        /// <summary>
        /// Plugin version for tracking
        /// </summary>
        public string GetVersion()
        {
            return "1.0.0";
        }
    }
}