// <copyright file="BookmarkManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class BookmarkManager
    {
        public static BookmarkManager instance = new BookmarkManager();

        public List<int> ImportantYears = new List<int>();

        public void Save()
        {
            if (!Directory.Exists(Globals.ModDir + "common\\bookmarks\\"))
            {
                Directory.CreateDirectory(Globals.ModDir + "common\\bookmarks\\");
            }

            this.ImportantYears = this.ImportantYears.Where(i => i < SimulationManager.instance.Year).ToList();

            this.ImportantYears.Add(SimulationManager.instance.Year);

            TechnologyManager.instance.SaveOutTech(SimulationManager.instance.Year + 1);
            string filename = Globals.ModDir + "common\\bookmarks\\00_bookmarks.txt";
            int earliest = this.ImportantYears[0];
            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
            {
                for (var index = 0; index < this.ImportantYears.Count; index++)
                {
                    var importantYear = this.ImportantYears[index];

                    if (this.titles.ContainsKey(importantYear))
                    {
                        string title = this.titles[importantYear];
                        string desc = this.descs[importantYear];

                        string keyTitle = "BM_" + importantYear;
                        string descTitle = "BM_" + importantYear + "_DESC";

                        LanguageManager.instance.Add(keyTitle, title);
                        LanguageManager.instance.Add(descTitle, desc);
                        file.WriteLine(keyTitle + @" = {
	                name = """ + title + @"""

                    desc = """ + desc + @"""

                    date = " + Math.Min(importantYear, SimulationManager.instance.MaxYear) + @".9.15

                                era = yes						## Will be shown on Era screen.
	                            picture = GFX_pick_era_image_1
                            }
                ");
                    }
                    else
                    {
                        LanguageManager.instance.Add("BM_CHARLEMAGNE" + importantYear, importantYear.ToString());
                        LanguageManager.instance.Add("BM_BM_CHARLEMAGNE_DESC" + importantYear, importantYear.ToString());
                        file.WriteLine(@"bm_charlemagne" + importantYear + @" = {
	                    name = ""BM_CHARLEMAGNE" + importantYear + @"""

                        desc = ""BM_BM_CHARLEMAGNE_DESC" + importantYear + @"""

                        date = " + Math.Min(importantYear, SimulationManager.instance.MaxYear) + @".9.15

                                    era = yes						## Will be shown on Era screen.
	                                picture = GFX_pick_era_image_1
                                }
                    ");
                    }
                }

                file.Close();
            }

            {
                filename = Globals.ModDir + "common\\defines.txt";
                using (System.IO.StreamWriter file2 =
                    new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
                {
                    file2.WriteLine(@"
start_date = " + earliest + @".1.1
last_start_date = " + this.ImportantYears[this.ImportantYears.Count - 1] + @".9.15
end_date = 9999.1.1

character = {
}

diplomacy = {
}

economy = {
}

military = {
}
");
                }
            }
        }

        private Dictionary<int, string> titles = new Dictionary<int, string>();
        private Dictionary<int, string> descs = new Dictionary<int, string>();

        public void AddImportantYear(int i)
        {
            if (!SimulationManager.instance.AllowSimBookmarks)
            {
                return;
            }

            if (!this.ImportantYears.Contains(i))
            {
                this.ImportantYears.Add(i);
                TechnologyManager.instance.SaveOutTech();
            }
        }

        public void AddImportantYear(int i, string tit, string desc)
        {
            if (!this.ImportantYears.Contains(i))
            {
                this.ImportantYears.Add(i);
                TechnologyManager.instance.SaveOutTech();
                this.titles[i] = (tit);
                this.descs[i] = (desc);
            }
        }
    }
}