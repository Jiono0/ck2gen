// <copyright file="RegionManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System.Collections.Generic;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Parsers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    internal class RegionManager
    {
        public static RegionManager instance = new RegionManager();

        internal class Region
        {
            public string name;
            public List<TitleParser> duchies = new List<TitleParser>();
        }

        public List<TitleParser> duchiesAssigned = new List<TitleParser>();

        public List<Region> regions = new List<Region>();

        public void AddRegion(string name, List<TitleParser> duchies)
        {
            string safeName = StarHelper.SafeName(name);

            LanguageManager.instance.Add(safeName, name);
            Region r = new Region();
            for (int index = 0; index < duchies.Count; index++)
            {
                var titleParser = duchies[index];
                if (this.duchiesAssigned.Contains(titleParser))
                {
                    duchies.Remove(titleParser);
                    index--;
                }
            }

            r.name = name;
            r.duchies.AddRange(duchies);
            this.duchiesAssigned.AddRange(duchies);
            this.regions.Add(r);
        }

        public void Save()
        {
            Script s = new Script();
            s.Name = Globals.GameDir + "map\\geographical_region.txt";
            s.Root = new ScriptScope();
            foreach (var region in this.regions)
            {
                ScriptScope ss = new ScriptScope();

                string duchieList = "";

                foreach (var titleParser in region.duchies)
                {
                    duchieList = duchieList + " " + titleParser.Name;
                }

                ss.Name = StarHelper.SafeName(region.name);
                ss.Do(@"
                    duchies = {
                        " + duchieList + @"
                    }
");
                s.Root.Add(ss);
           }

            s.Save();
        }
    }
}
