using System;
using System.Windows.Forms;

namespace Lab2
{
    static class Program
    {
        // Main entry point for the application
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}