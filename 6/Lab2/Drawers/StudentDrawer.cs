using System;
using System.Drawing;

namespace Lab2.Drawers
{
    /// <summary>
    /// Drawer for StudentShape - visualizes student data
    /// </summary>
    public class StudentDrawer : IShapeDrawer
    {
        public void Draw(Shape shape, Graphics g)
        {
            if (shape is StudentShape studentShape)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw rectangle
                using (Pen pen = new Pen(studentShape.Color, 3))
                {
                    g.DrawRectangle(pen, studentShape.X, studentShape.Y, studentShape.Width, studentShape.Height);
                }

                // Draw graduation cap icon
                int iconX = studentShape.X + 8;
                int iconY = studentShape.Y + 8;
                using (Pen pen = new Pen(studentShape.Color, 2))
                {
                    Point[] cap = {
                        new Point(iconX, iconY + 6),
                        new Point(iconX + 8, iconY),
                        new Point(iconX + 16, iconY + 6),
                        new Point(iconX + 8, iconY + 3)
                    };
                    g.DrawPolygon(pen, cap);
                    g.DrawLine(pen, iconX + 8, iconY + 3, iconX + 8, iconY + 10);
                }

                // Draw text if enough space
                if (studentShape.Width > 70 && studentShape.Height > 40)
                {
                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
                    using (Brush brush = new SolidBrush(studentShape.Color))
                    {
                        g.DrawString(studentShape.Name, font, brush, studentShape.X + 30, studentShape.Y + 8);

                        if (!string.IsNullOrEmpty(studentShape.StudentInfo))
                        {
                            using (Font smallFont = new Font("Arial", 6))
                            {
                                g.DrawString(studentShape.StudentInfo, smallFont, brush,
                                    studentShape.X + 30, studentShape.Y + 22);
                            }
                        }
                    }
                }
            }
        }
    }
}