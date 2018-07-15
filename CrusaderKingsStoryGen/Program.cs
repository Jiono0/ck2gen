using System;
using System.IO;
using System.Windows.Forms;

namespace CrusaderKingsStoryGen
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Directory.Exists("logs"))
            {
                Directory.Delete("logs", true);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
