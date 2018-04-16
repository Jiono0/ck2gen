// <copyright file="StoryManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System.Collections.Generic;
    using CrusaderKingsStoryGen.Forms;

    class StoryManager
    {
        public static StoryManager instance = new StoryManager();
        public List<string> events = new List<string>();
        public int lastYear = -1;
        private object lastSubject;

        public void CreateEvent(int year, string text, object primarySubject)
        {
            string yearStr = "";
            if (this.lastYear != -1 && this.lastYear < year && primarySubject == this.lastSubject)
            {
                if (year - this.lastYear == 1)
                {
                    switch (RandomIntHelper.Next(3))
                    {
                        case 0:
                            yearStr = "A year later, ";
                            break;
                        case 1:
                            yearStr = "One year later, ";
                            break;
                        case 2:
                            yearStr = "It was a year later when ";
                            break;
                    }
                }
                else
                {
                    int n = year - this.lastYear;
                    switch (RandomIntHelper.Next(2))
                    {
                        case 0:
                            yearStr = n + " years later, ";
                            break;
                        case 1:
                            yearStr = "After " + n + " long years, ";
                            break;
                    }
                }
            }
            else
            {
                switch (RandomIntHelper.Next(3))
                {
                    case 0:
                        yearStr = "In " + year + ", ";
                        break;
                    case 1:
                        yearStr = "In the year of " + year + ", ";
                        break;
                    case 2:
                        yearStr = "It was in " + year + " when ";
                        break;
                }
            }

            this.lastYear = year;

            this.events.Add(yearStr + text);
            MainForm.instance.Log(yearStr + text);
            this.lastSubject = primarySubject;
        }
    }
}
