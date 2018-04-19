// <copyright file="TechnologyManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Parsers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    class TechnologyManager
    {
        public static TechnologyManager instance = new TechnologyManager();
        private Script script;
        public List<ProvinceParser> TechOrderedProvinceList = new List<ProvinceParser>();
        public List<TitleParser> TechOrderedTitleList = new List<TitleParser>();
        public List<TitleParser> TechOrderedDukeTitleList = new List<TitleParser>();
        public Dictionary<float, TechnologyHelper> Groups = new Dictionary<float, TechnologyHelper>();
        public List<ProvinceParser> CentresOfTech = new List<ProvinceParser>();
        private float minMilitary = 0f;
        private float minEconomy = 0f;
        private float minCulture = 0f;

        private float minDate = 200;
        private float maxDate = 1070;

        private float maxMilitary = 4f;
        private float maxEconomy = 4f;
        private float maxCulture = 4f;

        public Script Script
        {
            get { return this.script; }
            set { this.script = value; }
        }

        public int numMerchantRepublics = 0;

        public void Init()
        {
            this.Script = new Script();

            this.Script.Name = Globals.ModDir + "history\\technology\\techs.txt";
            this.Script.Root = new ScriptScope();
        }

        List<ReligionParser> religionsTheocracy = new List<ReligionParser>();



        private int useYear = 0;
        private int lastYear = -1;

        public void SaveOutTech(int year = -1)
        {
            // CJS
            return;
            if (year == -1)
            {
                year = SimulationManager.instance.Year-3;
                this.useYear = year;
            }
            else
            {
                this.useYear = year;
                if (this.lastYear == -1)
                {
                    this.SaveOutTech(this.useYear-2);
                }
            }

            this.lastYear = this.useYear;


            if (this.TechOrderedDukeTitleList.Count == 0)
            {
                foreach (var titleParser in TitleManager.instance.Titles)
                {
                    if (titleParser.Rank == 2)
                    {
                        this.TechOrderedDukeTitleList.Add(titleParser);
                    }

                    if (titleParser.Liege == null)
                    {
                        this.TechOrderedTitleList.Add(titleParser);
                    }
                }

                this.TechOrderedProvinceList.AddRange(MapManager.instance.Provinces);
                this.TechOrderedTitleList.Sort(this.SortDuchyByTotalTech);
                this.TechOrderedDukeTitleList.Sort(this.SortDuchyByTotalTech);
            }

            float dateDelta = (year - this.minDate) / (float)(this.maxDate - this.minDate);

            if (dateDelta < 0)
            {
                dateDelta = 0;
            }

            if (dateDelta > 1)
            {
                dateDelta = 1;
            }

            float lastDelta = 0;
            float orderDelta = 1.0f;
            int ToLower = this.TechOrderedDukeTitleList.Count;
            ToLower = (int)(ToLower * 0.005f);
            float rate = 1.0f - (MutationHelper.TechSpreadRate / 5.0f);
            ToLower = (int)(ToLower * rate);
            int cccc = 0;
            float lowerAmount = 0;

            foreach (var titleParser in this.TechOrderedDukeTitleList)
            {
                float delSpeed = (5 - MutationHelper.TechAdvanceRate) / 5.0f;
                delSpeed *= 2.0f;
                float mil = this.lerp(this.minMilitary, this.maxCulture * delSpeed, dateDelta - lowerAmount);
                float cul = this.lerp(this.minCulture, this.maxCulture * delSpeed, dateDelta - lowerAmount);
                float eco = this.lerp(this.minEconomy, this.maxCulture * delSpeed, dateDelta - lowerAmount);

                this.Add(titleParser, mil, cul, eco);

                cccc++;
                if (cccc >= ToLower)
                {
                    lowerAmount += 1.0f / 400.0f;
                    cccc = 0;
                }
            }
        }

        private void Add(TitleParser title, float mil, float cul, float eco)
        {
            string name = title.Name;
            int y = this.useYear;
            if (y > SimulationManager.instance.MaxYear)
            {
                y = SimulationManager.instance.MaxYear;
            }

            var group = title.TechGroup;
            if (title.TechGroup != null)
            {
                if (title.TechGroup.HasNamed(y.ToString()))
                {
                    return;
                }

                var date = new ScriptScope((y).ToString());
                date.Add(new ScriptCommand("military", mil, date));
                date.Add(new ScriptCommand("economy", eco, date));
                date.Add(new ScriptCommand("culture", cul, date));
                title.TechGroup.Add(date);
                return;
            }

            if (!this.Groups.ContainsKey(mil))
            {
                this.Groups[mil] = new TechnologyHelper();
                this.Groups[mil].Date = new Dictionary<int, ScriptScope>();
                this.Groups[mil].Date[y] = new ScriptScope((y).ToString());
                this.Groups[mil].Date[y].Add(new ScriptCommand("military", mil, this.Groups[mil].Date[y]));
                this.Groups[mil].Date[y].Add(new ScriptCommand("economy", eco, this.Groups[mil].Date[y]));
                this.Groups[mil].Date[y].Add(new ScriptCommand("culture", cul, this.Groups[mil].Date[y]));
                this.Groups[mil].Titles = new ScriptScope("titles");
                this.Groups[mil].Titles.Add("\t\t"+name);
                this.Script.Root.AllowDuplicates = true;
                var tech = new ScriptScope("technology");
                this.Script.Root.Add(tech);
                tech.Add(this.Groups[mil].Titles);
                tech.Add(this.Groups[mil].Date[y]);
                title.TechGroup = this.Groups[mil].Titles.Parent;
            }
            else
            {
                if (!this.Groups[mil].Titles.Children.Contains("\t\t" + name))
                {
                    this.Groups[mil].Titles.Add("\t\t" + name);
                    title.TechGroup = this.Groups[mil].Titles.Parent;
                }
            }
        }

        private float lerp(float min, float max, float delta)
        {
            if (delta < 0)
            {
                delta = 0;
            }

            float d = (max - min)*delta;
            d += min;

            d *= 10.0f;
            d = (int) d;
            d /= 10.0f;

            return d;
        }

        private int SortDuchyByTotalTech(TitleParser x, TitleParser y)
        {
            if (x.AverageTech > y.AverageTech)
            {
                return -1;
            }

            if (x.AverageTech < y.AverageTech)
            {
                return 1;
            }

            return 0;
        }


        public void Save()
        {
            if (!Directory.Exists(Globals.ModDir + "history\\technology\\"))
            {
                Directory.CreateDirectory(Globals.ModDir + "history\\technology\\");
            }

            if (File.Exists(Globals.ModDir + "history\\technology\\techs.txt"))
            {
                File.Delete(Globals.ModDir + "history\\technology\\techs.txt");
            }

            this.Script.Save();
        }

        private int holdingCost = 400;
        private int maxMerchantRepublics = 7;

        public void HandleTech(CharacterParser chr)
        {
            return;
            var provinces = chr.GetAllProvinces();

            foreach (var provinceParser in provinces)
            {
                if (provinceParser.economicTechPoints > this.holdingCost)
                {
                    bool bTribal = false;
                    if (provinceParser.government=="tribal")
                    {
                        bTribal = true;
                    }

                    if (bTribal)
                    {
                        // work toward feudalism...
                        provinceParser.economicTechPoints -= this.holdingCost;

                        provinceParser.baronies[0].level++;
                        if (provinceParser.baronies[0].level >= 5)
                        {
                            ScriptScope thing = new ScriptScope();
                            string barName = provinceParser.baronies[0].title;
                            thing.Name = SimulationManager.instance.Year + ".4.1";
                            bool done = false;
                            if (this.numMerchantRepublics < this.maxMerchantRepublics)
                            {
                                if (provinceParser.Adjacent.Where(p => !p.land && p.Range.Y - p.Range.X > 10).Any() && provinceParser.Title.Liege != null && provinceParser.Title.Liege.Rank==2 && provinceParser.Title.Liege.CapitalProvince == provinceParser )
                                {
                                    if (RandomIntHelper.Next(4) == 0)
                                    {
                                        provinceParser.republic = true;
                                        thing.Add(new ScriptCommand() { Name = barName, Value = "city" });
                                        done = true;
                                        provinceParser.government = "republic";
                                        this.numMerchantRepublics++;

                                        {
                                            var chosen = provinceParser.Title.Liege;
                                            chosen.Holder.GiveTitleSoft(provinceParser.Title);
                                            chosen.CapitalProvince = provinceParser;
                                            //chosen.Holder.GiveTitleSoft(provinceParser.Title);
                                            //if (provinceParser.Title.Liege != chosen)
                                            //  chosen.Holder.GiveTitleSoft(provinceParser.Title.Liege);
                                            chosen.DoSetLiegeEvent(null);
                                            chosen.government = "republic";
                                            provinceParser.Title.government = "republic";
                                            chosen.Scope.Do(@"
		                                dignity = 200 # Never want the Republic of Venice to change primary title

                                        allow = {
			                                is_republic = yes
		                                }
                                ");

                                            {
                                                ScriptScope thingTit = new ScriptScope();

                                                thingTit.Name = SimulationManager.instance.Year + ".4.1";
                                                ScriptScope thingTit3 = new ScriptScope();

                                                thingTit3.Name = SimulationManager.instance.Year + ".4.1";

                                                chosen.titleScripts.Add(thingTit);
                                                provinceParser.Title.titleScripts.Add(thingTit3);

                                                thingTit.Add(new ScriptCommand() { Name = "law", Value = "succ_patrician_elective" });
                                                thingTit3.Add(new ScriptCommand() { Name = "law", Value = "succ_patrician_elective" });
                                                // if (title.Liege != null && title.Rank == 0 && title.TopmostTitle.government == "republic")
                                                {
                                                    // if (title.Liege.Owns[0].baronies[0].title == title.Name)
                                                    {
                                                        var name = chosen.Culture.dna.GetPlaceName();
                                                        var sname = StarHelper.SafeName(name);
                                                        while (LanguageManager.instance.Get(sname) != null)
                                                        {
                                                            name = chosen.Culture.dna.GetPlaceName();
                                                            sname = StarHelper.SafeName(name);
                                                        }

                                                        chosen.republicdynasties.Add(chosen.Holder.Dynasty.ID);

                                                        for (int x = 0; x < 4; x++)
                                                        {
                                                            Dynasty d = DynastyManager.instance.GetDynasty(chosen.Culture);
                                                            if (!TitleManager.instance.dynastiesWithPalaces.Contains(d.ID))
                                                            {
                                                                var barony = TitleManager.instance.CreateBaronyScriptScope(null, chosen.Culture);
                                                                TitleManager.instance.Titles.Add(barony);
                                                                barony.government = "republicpalace";
                                                                barony.republicdynasty = d.ID;
                                                                barony.culture = chosen.culture;
                                                                barony.PalaceLocation = provinceParser;
                                                                barony.republicreligion = chosen.Holder.religion;
                                                                barony.DoSetLiegeEvent(chosen);
                                                                var cr = SimulationManager.instance.AddCharacterForTitle(barony,
                                                                    true, false, d);

                                                                //chosen.Holder.GiveTitleSoft(barony);
                                                                chosen.Holder.Dynasty.palace = barony;
                                                                TitleManager.instance.dynastiesWithPalaces.Add(d.ID);
                                                                chosen.palaces.Add(barony);
                                                                if (barony.Rank == 0 && barony.government == "republicpalace")
                                                                {
                                                                    ScriptScope thingTit2 = new ScriptScope();

                                                                    thingTit2.Name = SimulationManager.instance.Year + ".4.1";
                                                                    thingTit2.Add(new ScriptCommand() { Name = "holding_dynasty", Value = barony.republicdynasty });
                                                                    //thing.Add(new ScriptCommand() { Name = "liege", Value = title.republicdynasty });
                                                                    if (barony.Scope.Find("culture") == null)
                                                                    {
                                                                        barony.Scope.Add(new ScriptCommand() { Name = "culture", Value = chosen.culture });
                                                                        barony.Scope.Add(new ScriptCommand() { Name = "religion", Value = chosen.Holder.religion });
                                                                    }

                                                                    barony.titleScripts.Add(thingTit2);
                                                                }
                                                            }
                                                        }

                                                        if (!TitleManager.instance.dynastiesWithPalaces.Contains(chosen.Holder.Dynasty.ID))
                                                        {
                                                            var barony = TitleManager.instance.CreateBaronyScriptScope(null, chosen.Culture);
                                                            TitleManager.instance.Titles.Add(barony);
                                                            barony.government = "republicpalace";
                                                            barony.republicdynasty = chosen.Holder.Dynasty.ID;
                                                            barony.culture = chosen.culture;
                                                            barony.republicreligion = chosen.Holder.religion;
                                                            barony.DoSetLiegeEvent(chosen);
                                                            barony.PalaceLocation = provinceParser;
                                                            chosen.Holder.GiveTitleSoft(barony);
                                                            chosen.Holder.Dynasty.palace = barony;
                                                            chosen.palaces.Add(barony);
                                                            TitleManager.instance.dynastiesWithPalaces.Add(chosen.Holder.Dynasty.ID);
                                                            if (barony.Rank == 0 && barony.government == "republicpalace")
                                                            {
                                                                ScriptScope thingTit2 = new ScriptScope();

                                                                thingTit2.Name = SimulationManager.instance.Year + ".4.1";
                                                                thingTit2.Add(new ScriptCommand() { Name = "holding_dynasty", Value = barony.republicdynasty });
                                                                //thing.Add(new ScriptCommand() { Name = "liege", Value = title.republicdynasty });
                                                                if (barony.Scope.Find("culture") == null)
                                                                {
                                                                    barony.Scope.Add(new ScriptCommand() { Name = "culture", Value = chosen.culture });
                                                                    barony.Scope.Add(new ScriptCommand() { Name = "religion", Value = chosen.Holder.religion });
                                                                }

                                                                barony.titleScripts.Add(thingTit2);
                                                            }
                                                        }
                                                    }
                                                }

                                                if (chosen.Liege != null && chosen.Liege.Rank > chosen.Rank)
                                                {
                                                    thingTit.Add(new ScriptCommand() { Name = "liege", Value = chosen.Liege.Name });
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!done)
                            {
                                thing.Add(new ScriptCommand() {Name = barName, Value = "castle"});
                                provinceParser.government = "feudalism";
                            }

                            provinceParser.militaryTechPoints = 0;
                            provinceParser.dateScripts.Add(thing);
                            if (provinceParser.Title.Holder == chr)
                            {
                                chr.PrimaryTitle.government = "feudalism";
                            }
                        }
                    }
                    else if(provinceParser.militaryTechPoints > this.holdingCost)
                    {
                        if (provinceParser.ActiveBaronies < provinceParser.Max_settlements)
                        {
                            provinceParser.militaryTechPoints -= this.holdingCost;
                            ProvinceParser.Barony b = provinceParser.GetLastEnabledBarony();
                            if (b.level >= 2)
                            {
                                List<string> choices = new List<string>();

                                b = provinceParser.GetNextBarony();
                                if (provinceParser.CastleCount == 0)
                                {
                                    choices.Add("castle");
                                }

                                if (provinceParser.TownCount == 0)
                                {
                                    choices.Add("city");
                                }

                                if (provinceParser.TempleCount == 0)
                                {
                                    choices.Add("temple");
                                }

                                if (choices.Count == 0)
                                {
                                    choices.Add("castle");
                                    choices.Add("city");
                                    choices.Add("temple");
                                }

                                {
                                    b.enabled = true;
                                    b.type = choices[RandomIntHelper.Next(choices.Count)];
                                    ScriptScope thing = new ScriptScope();
                                    string barName = b.title;
                                    thing.Name = SimulationManager.instance.Year + ".4.1";
                                    thing.Add(new ScriptCommand() {Name = barName, Value = b.type});

                                    provinceParser.dateScripts.Add(thing);
                                }
                            }
                            else
                            {
                                b.level++;
                            }
                        }
                    }
                }
            }
        }
    }
}
