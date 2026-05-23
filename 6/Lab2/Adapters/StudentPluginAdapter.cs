using System;
using System.Drawing;
using System.Reflection;

namespace Lab2.Adapters
{
    /// <summary>
    /// Adapter pattern: Adapts comrade's StudentPlugin to work as IShapePlugin
    /// </summary>
    public class StudentPluginAdapter : IShapePlugin
    {
        private object _comradePlugin;
        private string _shapeName;

        public StudentPluginAdapter(object comradePlugin, string shapeName)
        {
            _comradePlugin = comradePlugin;
            _shapeName = shapeName;
        }

        public string GetShapeName() => _shapeName;

        public IShapeCreator GetCreator()
        {
            return new StudentPluginCreator(_comradePlugin, _shapeName);
        }

        public IShapeDrawer GetDrawer()
        {
            return new StudentPluginDrawer(_shapeName);
        }
    }

    /// <summary>
    /// Adapter creator - converts comrade's Person data to Shape
    /// </summary>
    public class StudentPluginCreator : IShapeCreator
    {
        private object _comradePlugin;
        private string _shapeName;

        public StudentPluginCreator(object plugin, string shapeName)
        {
            _comradePlugin = plugin;
            _shapeName = shapeName;
        }

        public string GetName() => _shapeName;

        public Shape Create(Point start, Point end)
        {
            int x = Math.Min(start.X, end.X);
            int y = Math.Min(start.Y, end.Y);
            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);

            if (width < 60) width = 60;
            if (height < 50) height = 50;

            var shape = new StudentShape(_shapeName, x, y, width, height);

            // Try to create a person from comrade's plugin
            try
            {
                var createMethod = _comradePlugin.GetType().GetMethod("CreatePerson");
                if (createMethod != null)
                {
                    object[] parameters = GenerateStudentData();
                    var person = createMethod.Invoke(_comradePlugin, new object[] { parameters });
                    shape.SetStudentData(person);
                }
            }
            catch (Exception ex)
            {
                shape.SetStudentData($"Adapted: {_shapeName}");
            }

            return shape;
        }

        private object[] GenerateStudentData()
        {
            Random rand = new Random();
            return new object[]
            {
                $"Student_{rand.Next(1000)}",      // name
                20 + rand.Next(10),                // age
                "University Campus",                // address
                $"ID_{rand.Next(10000)}",           // studentId
                3.0 + rand.NextDouble(),            // GPA
                $"Thesis_Topic_{rand.Next(100)}",   // thesisTitle
                "Prof. Advisor",                    // advisor
                "PhD"                               // degree
            };
        }
    }

    /// <summary>
    /// Adapter drawer - visualizes adapted student shape
    /// </summary>
    public class StudentPluginDrawer : IShapeDrawer
    {
        private string _shapeName;

        public StudentPluginDrawer(string shapeName)
        {
            _shapeName = shapeName;
        }

        public void Draw(Shape shape, Graphics g)
        {
            if (shape is StudentShape studentShape)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Draw rectangle border
                using (Pen pen = new Pen(studentShape.Color, 3))
                {
                    g.DrawRectangle(pen, studentShape.X, studentShape.Y, studentShape.Width, studentShape.Height);
                }

                // Draw "ADAPTER" badge to show pattern in use
                using (Font font = new Font("Arial", 7, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.Blue))
                {
                    g.DrawString("[ADAPTER]", font, brush, studentShape.X + 5, studentShape.Y + 5);
                }

                // Draw shape name
                using (Font font = new Font("Arial", 8, FontStyle.Bold))
                using (Brush brush = new SolidBrush(studentShape.Color))
                {
                    g.DrawString(_shapeName, font, brush, studentShape.X + 10, studentShape.Y + 18);
                }

                // Draw student info if available
                if (!string.IsNullOrEmpty(studentShape.StudentInfo) && studentShape.Height > 40)
                {
                    using (Font smallFont = new Font("Arial", 6))
                    using (Brush brush = new SolidBrush(studentShape.Color))
                    {
                        string info = studentShape.StudentInfo;
                        if (info.Length > 25) info = info.Substring(0, 22) + "...";
                        g.DrawString(info, smallFont, brush, studentShape.X + 10, studentShape.Y + 32);
                    }
                }
            }
        }
    }
}