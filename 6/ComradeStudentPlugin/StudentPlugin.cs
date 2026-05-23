using System;
using System.Collections.Generic;

namespace ComradeStudentPlugin
{
    public interface IPlugin
    {
        string PluginName { get; }
        string Version { get; }
        Type PersonType { get; }
        void Initialize();
        object CreatePerson(object[] parameters);
        Dictionary<string, Type> GetPropertyDefinitions();
    }

    public abstract class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }

        protected Person(string name, int age, string address)
        {
            Name = name;
            Age = age;
            Address = address;
        }

        public abstract string GetRole();
        public virtual string Serialize() => $"{Name}|{Age}|{Address}";
    }

    public class GraduateStudent : Person
    {
        public string StudentId { get; set; }
        public double GPA { get; set; }
        public string ThesisTitle { get; set; }
        public string Advisor { get; set; }
        public string Degree { get; set; }

        public GraduateStudent(string name, int age, string address, string studentId, double gpa,
            string thesisTitle, string advisor, string degree)
            : base(name, age, address)
        {
            StudentId = studentId;
            GPA = gpa;
            ThesisTitle = thesisTitle;
            Advisor = advisor;
            Degree = degree;
        }

        public override string GetRole() => $"Graduate Student ({Degree})";
        public override string Serialize() => base.Serialize() + $"|{StudentId}|{GPA}|{ThesisTitle}|{Advisor}|{Degree}";
        public override string ToString() => $"{Name}, {Degree} student, Thesis: {ThesisTitle}";
    }

    // ONLY GraduateStudentPlugin, NO InternPlugin
    public class GraduateStudentPlugin : IPlugin
    {
        public string PluginName => "Graduate Student Plugin";
        public string Version => "1.0.0";
        public Type PersonType => typeof(GraduateStudent);

        public void Initialize() { }

        public object CreatePerson(object[] parameters)
        {
            return new GraduateStudent(
                (string)parameters[0],
                (int)parameters[1],
                (string)parameters[2],
                (string)parameters[3],
                (double)parameters[4],
                (string)parameters[5],
                (string)parameters[6],
                (string)parameters[7]
            );
        }

        public Dictionary<string, Type> GetPropertyDefinitions()
        {
            return new Dictionary<string, Type>
            {
                { "Name", typeof(string) },
                { "Age", typeof(int) },
                { "Address", typeof(string) },
                { "StudentId", typeof(string) },
                { "GPA", typeof(double) },
                { "ThesisTitle", typeof(string) },
                { "Advisor", typeof(string) },
                { "Degree", typeof(string) }
            };
        }
    }
}