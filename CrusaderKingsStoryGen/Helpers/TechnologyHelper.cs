// <copyright file="TechnologyGroup.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using System.Collections.Generic;
    using CrusaderKingsStoryGen.ScriptHelpers;

    class TechnologyHelper
    {
        public ScriptScope Scope { get; set; }

        public void Init()
        {
            this.Scope = new ScriptScope("technology");
            this.Titles = new ScriptScope("titles");

            this.Scope.Add(this.Titles);
        }

        public ScriptScope Titles { get; set; }

        public void AddTitle(string title)
        {
            this.Titles.Add(title);
        }

        public void AddDated(int date, ScriptCommand command)
        {
            ScriptScope dates = null;
            if (this.Date.ContainsKey(date))
            {
                dates = this.Date[date];
            }
            else
            {
                dates = new ScriptScope(date.ToString());
                this.Date[date] = dates;
                this.Scope.Add(dates);
            }

            dates.Add(command);
        }


        public Dictionary<int, ScriptScope> Date = new Dictionary<int, ScriptScope>();
    }
}
