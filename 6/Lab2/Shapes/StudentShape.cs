using System;
using System.Drawing;

namespace Lab2
{
    /// <summary>
    /// Custom shape class that holds student data from comrade's plugin
    /// Used by Adapter pattern
    /// </summary>
    public class StudentShape : Shape
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public object StudentData { get; private set; }
        public string StudentInfo { get; set; }

        public StudentShape()
        {
            Name = "StudentShape";
            X = 0;
            Y = 0;
            Width = 50;
            Height = 50;
        }

        public StudentShape(string name, int x, int y, int width, int height)
        {
            Name = name;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public void SetStudentData(object data)
        {
            StudentData = data;
            if (data != null)
            {
                StudentInfo = data.ToString();
                if (StudentInfo.Length > 30)
                    StudentInfo = StudentInfo.Substring(0, 27) + "...";
            }
        }
    }
}