// <copyright file="EventLogger.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using System.Collections.Generic;
    using System.IO;

    public class EventLogHelper
    {
        public static EventLogHelper instance = new EventLogHelper();

        public List<string> titleLogList = new List<string>();

        public EventLogHelper()
        {
         //   AddTitle("c_blois");
         //   AddTitle("k_lotharingia");
        }

        Dictionary<string, System.IO.StreamWriter> files = new Dictionary<string, StreamWriter>();

        internal void AddTitle(string title)
        {
            if (this.files.ContainsKey(title))
            {
                return;
            }

            string filename = Directory.GetCurrentDirectory() + "\\logs\\" + title + ".txt";

            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\logs\\"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\logs\\");
            }

            string f = filename;
            int n = 2;
            while (File.Exists(f))
            {
                f = filename.Replace(".txt", n + ".txt");
                n++;
            }

            System.IO.StreamWriter file =
                new System.IO.StreamWriter(f);


            this.files[title] = file;
        }

        public void Log(string title, string text)
        {
            if (!this.files.ContainsKey(title))
            {
                return;
            }

            this.files[title].WriteLine(Simulation.SimulationManager.instance.Year + " - " + text);
        }

        public void Save()
        {
            foreach (var filesValue in this.files.Values)
            {
                filesValue.Close();
            }
        }
    }
}
