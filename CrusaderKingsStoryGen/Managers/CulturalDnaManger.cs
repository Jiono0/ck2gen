// <copyright file="CulturalDnaManger.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System;
    using System.Collections.Generic;

    internal class CulturalDnaManager
    {
        public static CulturalDnaManager instance = new CulturalDnaManager();
        public Dictionary<string, StringManipulationHelper> dna = new Dictionary<string, StringManipulationHelper>();
        public List<string> dnaTypes = new List<string>();

        public StringManipulationHelper GetVanillaCulture(string culture)
        {
            if (culture == null)
            {
                culture = this.dnaTypes[RandomIntHelper.Next(this.dnaTypes.Count)];
            }

            return this.dna[culture]; ;
        }

        public StringManipulationHelper GetVanillaCulture(StringManipulationHelper not)
        {
            string culture = this.dnaTypes[RandomIntHelper.Next(this.dnaTypes.Count)];

            while (this.dna[culture] == not)
            {
                culture = this.dnaTypes[RandomIntHelper.Next(this.dnaTypes.Count)];
            }

            return this.dna[culture]; ;
        }

        public StringManipulationHelper GetNewFromVanillaCulture(string culture = null)
        {
            if (culture == null)
            {
                culture = this.dnaTypes[RandomIntHelper.Next(this.dnaTypes.Count)];
            }

            StringManipulationHelper dna = this.dna[culture];
            dna.culture = null;
            StringManipulationHelper dna2 = dna.Mutate(32, null);
            dna2.DoRandom();
            return dna2;
        }

        public void Init()
        {
            Script s = ScriptLoader.instance.Load(Globals.GameDir + "common\\cultures\\00_cultures.txt");
            foreach (var child in s.Root.Children)
            {
                if (child is ScriptScope)
                {
                    ScriptScope cc = (ScriptScope)child;

                    foreach (var scriptScope in cc.Scopes)
                    {
                        if (scriptScope.Name == "graphical_cultures")
                        {
                            continue;
                        }

                        StringManipulationHelper dna = new StringManipulationHelper();
                        foreach (var scope in scriptScope.Scopes)
                        {
                            if (scope.Name == "male_names" || scope.Name == "female_names")
                            {
                                string[] male_names = scope.Data.Split(new[] { ' ', '_', '\t' });
                                foreach (var maleName in male_names)
                                {
                                    var mName = maleName.Trim();
                                    if (mName.Length > 0)
                                    {
                                        dna.Cannibalize(mName);
                                    }
                                }
                            }
                        }

                        this.dna[scriptScope.Name] = dna;
                        this.dnaTypes.Add(scriptScope.Name);

                        dna.Name = scriptScope.Name;
                        dna.ShortenWordCounts();
                    }
                }
            }
        }
    }
}