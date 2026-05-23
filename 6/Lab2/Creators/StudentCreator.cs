using System;
using System.Drawing;

namespace Lab2.Creators
{
    /// <summary>
    /// Creator for StudentShape - part of Adapter pattern
    /// </summary>
    public class StudentCreator : IShapeCreator
    {
        private string _shapeName;
        private object _studentPlugin;

        public StudentCreator(string shapeName, object studentPlugin = null)
        {
            _shapeName = shapeName;
            _studentPlugin = studentPlugin;
        }

        public string GetName() => _shapeName;

        public Shape Create(Point start, Point end)
        {
            int x = Math.Min(start.X, end.X);
            int y = Math.Min(start.Y, end.Y);
            int width = Math.Abs(end.X - start.X);
            int height = Math.Abs(end.Y - start.Y);

            if (width < 50) width = 50;
            if (height < 50) height = 50;

            var shape = new StudentShape(_shapeName, x, y, width, height);

            // If we have student plugin, try to create sample data
            if (_studentPlugin != null)
            {
                try
                {
                    var method = _studentPlugin.GetType().GetMethod("CreatePerson");
                    if (method != null)
                    {
                        object[] parameters = GenerateSampleData();
                        var student = method.Invoke(_studentPlugin, new object[] { parameters });
                        shape.SetStudentData(student);
                    }
                }
                catch { }
            }

            return shape;
        }

        private object[] GenerateSampleData()
        {
            Random rand = new Random();
            return new object[]
            {
                $"Student_{rand.Next(1000)}",
                20 + rand.Next(10),
                "University Campus",
                $"ID_{rand.Next(10000)}",
                3.0 + rand.NextDouble(),
                $"Thesis_{rand.Next(100)}",
                "Prof. Advisor",
                "PhD"
            };
        }
    }
}