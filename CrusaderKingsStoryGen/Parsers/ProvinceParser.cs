// <copyright file="ProvinceParser.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using CrusaderKingsStoryGen.Simulation;

    class ProvinceParser :  Parser
    {
        public string ProvinceName { get; set; }

        public int id;
        public int provinceRCode;
        public int provinceGCode;
        public int provinceBCode;
        public int militaryTechPoints = 0;
        public int cultureTechPoints = 0;
        public int economicTechPoints = 0;

        public string ProvinceTitle { get; set; }

        public int Max_settlements { get; set; } = 7;

        public TitleParser ProvinceOwner
        {
            get { return this.Title; }

            set
            {
                if (value != null)
                {
                    this.ProvinceTitle = value.Name;
                }
                else
                {
                    this.ProvinceTitle = null;
                }
            }
        }

        public List<Barony> baronies = new List<Barony>();
        public string initialReligion = "";
        public string initialCulture =  "";

        public void Save()
        {
            if (this.ProvinceTitle == null)
            {
                return;
            }

            if (this.Title == null)
            {
                return;
            }

            if (!TitleManager.instance.Titles.Contains(this.Title))
            {
                int gfdgfd = 0;
            }

            string dir = Globals.ModDir + "history\\provinces\\";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string provincesDir = Globals.MapDir + "history\\provinces\\" + this.id.ToString() + " - " + this.Title.Name.Replace("c_", "") + ".txt";

            Script s = new Script();
            s.Root = new ScriptScope();
            s.Name = provincesDir;
            s.Root.Add(new ScriptCommand("title", this.Title.Name, s.Root));
            s.Root.Add(new ScriptCommand("max_settlements", this.Max_settlements, s.Root));



            if (this.Culture != null)
            {
                s.Root.Add(new ScriptCommand("culture", this.initialCulture, s.Root));
                s.Root.Add(new ScriptCommand("religion", this.initialReligion, s.Root));
            }

            if (MapManager.instance.LoadedTerrain.ContainsKey(this.id))
            {
                s.Root.Add(new ScriptCommand("terrain", MapManager.instance.LoadedTerrain[this.id], s.Root));
            }

            int cc = 0;
            foreach (var barony in this.baronies)
            {
                if (this.baronies[0].type == "tribal" && barony.type != "temple" && cc > 0)
                {
                    continue;
                }

                if (barony.enabled && cc==0)
                {
                    s.Root.Add(new ScriptCommand(barony.title, barony.type, s.Root));
                }

                cc++;
            }

            var cities = this.baronies.Where(c => c.type == "city" && c.enabled);

            if(cities.Any())
            if (this.Adjacent.Where(o => !o.land).Count() > 0)
                s.Root.Do(@"1.1.1 = { 
                   " + cities.First().title + @" = ct_port_1
                  }");

            if (this.terrain != null)
            {
                s.Root.Add(new ScriptCommand("terrain", this.terrain, s.Root));
            }

            foreach (var scriptScope in this.dateScripts)
            {
                s.Root.SetChild(scriptScope);
            }

            s.Save();
        }

        public void Rename(string name)
        {
            if (StarHelper.SafeName(name) == "selj")
            {
            }

            string oldName = this.ProvinceName;
            this.ProvinceName = StarHelper.SafeName(name);
            LanguageManager.instance.Remove(this.ProvinceName);
            LanguageManager.instance.Add(StarHelper.SafeName(name), name);
            LanguageManager.instance.Add("c_" + StarHelper.SafeName(name), name);
            LanguageManager.instance.Add("PROV" + this.id, name);
            MapManager.instance.ProvinceMap.Remove(oldName);
            MapManager.instance.ProvinceMap[this.ProvinceName] = this;

            if (this.Title != null)
            {
                this.Title.Rename(name, true);
            }
        }

        public void RenameSafe(string name)
        {
            string oldName = this.ProvinceName;
            this.ProvinceName = name;
            LanguageManager.instance.Remove(this.ProvinceName);
            MapManager.instance.ProvinceMap.Remove(oldName);
            MapManager.instance.ProvinceMap[this.ProvinceName] = this;
            this.ProvinceTitle = this.ProvinceName;
            if (this.Title != null)
            {
                this.Title.RenameSafe(name, true);
            }
        }

        public void RenameForCulture(CultureParser culture)
        {
           LanguageManager.instance.Remove(this.ProvinceName);

            string namem = null;

            do
            {
                var name = culture.dna.GetPlaceName();
                namem = name;
            } while (MapManager.instance.ProvinceMap.ContainsKey(StarHelper.SafeName(namem)));

            this.Rename(namem);
        }

        public void AddTemple(CultureParser culture)
        {
            this.AddBarony("temple", culture);
        }

        public void CreateProvinceDetails(CultureParser culture)
        {
            {
                this.AddBarony("tribal", culture);
            }

            this.AddAdditionalBaronies(culture);
        }

        private void AddAdditionalBaronies(CultureParser culture)
        {
            for (int x = 0; x < 7; x++)
            {
                this.AddBarony("tribal", culture, false);
            }
        }

        public void AddBarony(CultureParser culture)
        {
            this.AddBarony("tribal", culture);
        }

        public void AddBarony2(CultureParser culture)
        {
            this.AddBarony("tribal", culture);
        }

        public void AddBarony(string type, CultureParser culture, bool enabled = true)
        {
            TitleParser title = TitleManager.instance.CreateBaronyScriptScope(this, culture);
            this.baronies.Add(new Barony() { province = this, title = title.Name, titleParser = title, type = type, enabled = enabled });
            this.Title.AddSub(title);
        }

        public void AddBarony(string name, TitleParser title)
        {
             this.baronies.Add(new Barony() { province = this, title = name, titleParser = title, enabled =false});
         }

        public CharacterParser TotalLeader
        {
            get
            {
                var title = this.ProvinceOwner;
                if (title == null)
                {
                    return null;
                }

                while (title.Liege != null && title.Rank < title.Liege.Rank)
                {
                    title = title.Liege;
                }

                return title.Holder;
            }
        }

        public Color Color { get; set; }


        public TitleParser Title
        {
            get
            {
                if (this.ProvinceTitle == null)
                {
                    return null;
                }

                if (!TitleManager.instance.TitleMap.ContainsKey(this.ProvinceTitle))
                {
                    if (this.ProvinceName != null)
                    {
                        if (TitleManager.instance.TitleMap.ContainsKey(this.ProvinceName))
                        {
                            this.ProvinceTitle = this.ProvinceName;
                                return TitleManager.instance.TitleMap[this.ProvinceName];
                        }
                    }

                    return null;
                }

                return TitleManager.instance.TitleMap[this.ProvinceTitle];
            }
        }

        public bool loadingFromHistoryFiles { get; set; }

        public CultureParser Culture
        {
            get { return this._culture; }

            set
            {
                if (this._culture != null)
                {
                    this._culture.RemoveProvince(this);
                }

                if (this._culture != value && value != null && this.initialCulture!="" && !this.loadingFromHistoryFiles)
                {
                    ScriptScope thing = new ScriptScope();
                    thing.Name = SimulationManager.instance.Year + ".4.1";
                    thing.Add(new ScriptCommand() { Name = "culture", Value = value.Name });

                    this.dateScripts.Add(thing);
                }

                this._culture = value;
                this._culture.AddProvince(this);
            }
        }

        public List<ScriptScope> dateScripts = new List<ScriptScope>();

        public ReligionParser Religion
        {
            get { return this._religion; }

            set
            {
                if (this._religion != null)
                {
                    this._religion.RemoveProvince(this);
                }

                if (this._religion != value && value != null && this.initialReligion != "")
                {
                    ScriptScope thing = new ScriptScope();
                    thing.Name = SimulationManager.instance.Year + ".3.1";
                    thing.Add(new ScriptCommand() { Name = "religion", Value = value.Name });

                    this.dateScripts.Add(thing);
                }

                this._religion = value;
                this._religion.AddProvince(this);
            }
        }

        public int ActiveBaronies
        {
            get
            {
                int c = 0;
                foreach (var barony in this.baronies)
                {
                    if (barony.enabled)
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public string government { get; set; } = "tribal";

        public int CastleCount
        {
            get
            {
                int c = 0;
                foreach (var barony in this.baronies)
                {
                    if (barony.enabled && (barony.type == "castle" || barony.type == "tribal"))
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public int TempleCount
        {
            get
            {
                int c = 0;
                foreach (var barony in this.baronies)
                {
                    if (barony.enabled && barony.type == "temple")
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public int TownCount
        {
            get
            {
                int c = 0;
                foreach (var barony in this.baronies)
                {
                    if (barony.enabled && barony.type == "city")
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public Point Range { get; set; }

        public Rectangle Bounds { get; set; }

        public string EditorName { get; set; }

        public List<Point> Points = new List<Point>();

        public override string ToString()
        {
            return this.id + " - " + this.ProvinceTitle;
        }

        public ProvinceParser(ScriptScope scope) : base(scope)
        {
            int line = 0;
            foreach (var child in scope.Children)
            {
                if (child is ScriptCommand)
                {
                    this.RegisterProperty(line, ((child as ScriptCommand).Name), child);
                }

                line++;
                if (child is ScriptScope)
                {
                    var subscope = (child as ScriptScope);
                }
            }
        }

        public void DoTitleOwnership()
        {
            if (this.ProvinceTitle != null && TitleManager.instance.TitleMap.ContainsKey(this.ProvinceTitle))
            {
                TitleManager.instance.TitleMap[this.ProvinceTitle].Owns.Add(this);
                this.ProvinceOwner = TitleManager.instance.TitleMap[this.ProvinceTitle];
            }
        }

        public override ScriptScope CreateScope()
        {
            return null;
        }

        public List<ProvinceParser> Adjacent = new List<ProvinceParser>();

        public void AddAdjacent(ProvinceParser prov)
        {
            if (!this.Adjacent.Contains(prov))
            {
                this.Adjacent.Add(prov);
            }

            if (!prov.Adjacent.Contains(this))
            {
                prov.Adjacent.Add(this);
            }
        }

        public bool IsAdjacentToUnclaimed()
        {
            foreach (var provinceParser in this.Adjacent)
            {
                if (!provinceParser.Title.Claimed)
                {
                    return true;
                }
            }

            return false;
        }

        public class Barony
        {
            public string type;
            public string title;
            public ProvinceParser province;
            public TitleParser titleParser;
            public int level;

            public bool enabled { get; set; }
        }

        public List<Barony> Temples = new List<Barony>();
        public bool land = false;
        public int templeRequirement;
        private CultureParser _culture;
        private ReligionParser _religion;
        public bool republic;
        public string terrain;
        public bool river;

        public void GatherBaronies()
        {
            foreach (var child in this.Scope.Children)
            {
                if (child is ScriptCommand)
                {
                    ScriptCommand c = (ScriptCommand) child;

                    if (c.Name.StartsWith("b_"))
                    {
                        string str = c.Value.ToString();

                        if (c.Value.ToString() == "temple")
                        {
                            var t = new Barony() {type = c.Value.ToString(), title = c.Name, province = this};
                            this.Temples.Add(t);
                            MapManager.instance.Temples[c.Name] = t;
                        }
                    }
                }
            }
        }

        public TitleParser CreateTitle()
        {
            this.ProvinceTitle = "c_" + this.ProvinceName;


            var scope = new ScriptScope();
            scope.Name = this.ProvinceTitle;
            var c = new TitleParser(scope);
            c.capital = this.id;
            c.CapitalProvince = this;
            c.Owns.Add(this);
            TitleManager.instance.AddTitle(c);
            return c;
        }

        public CharacterParser GetCurrentHolder()
        {
            if (this.ProvinceTitle == null)
            {
                return null;
            }

            if (this.Title.CurrentHolder != null)
            {
                return this.Title.CurrentHolder;
            }

            return this.Title.Holder;
        }

        public float DistanceTo(ProvinceParser other)
        {
            var x = other;
            var p = this.Points[0];
            float a = p.X - x.Points[0].X;
            float b = p.Y - x.Points[0].Y;

            return (Math.Abs(a) + Math.Abs(b));
        }

        public bool IsAdjacentToSea()
        {
            foreach (var provinceParser in this.Adjacent)
            {
                if (!provinceParser.land)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddTown()
        {
            for (int index = 1; index < this.baronies.Count; index++)
            {
                var barony = this.baronies[index];
                if (!barony.enabled)
                {
                    barony.enabled = true;
                    barony.type = "town";
                    return;
                }
            }
        }

        public Barony GetLastEnabledBarony()
        {
            for (int index = 0; index < this.baronies.Count; index++)
            {
                var barony = this.baronies[index];
                if (!barony.enabled)
                {
                    return this.baronies[index - 1];
                }
            }

            return null;
        }

        public Barony GetNextBarony()
        {
            for (int index = 0; index < this.baronies.Count; index++)
            {
                var barony = this.baronies[index];
                if (!barony.enabled)
                {
                    return this.baronies[index];
                }
            }

            return null;
        }

        public void ActivateBarony(string eName, string type)
        {
            List<Barony> newList = new List<Barony>();
            List<Barony> newListUnenabled = new List<Barony>();

            for (int index = 0; index < this.baronies.Count; index++)
            {
                var barony = this.baronies[index];
                if (barony.titleParser.Name == eName)
                {
                    if (type == "castle" || type == "temple" || type == "city" || type == "tribal")
                    {
                        barony.enabled = true;
                        barony.type = type;

                        newList.Add(barony);
                    }
                }
                else if (barony.enabled)
                {
                    newList.Add(barony);
                }
                else
                {
                    newListUnenabled.Add(barony);
                }
            }

            this.baronies = new List<Barony>();
            this.baronies.AddRange(newList);
            this.baronies.AddRange(newListUnenabled);
        }

        public List<ProvinceParser> GetAdjacentLand(int i, List<ProvinceParser> list)
        {
            if (i <= 0)
            {
                return list;
            }

            foreach (var provinceParser in this.Adjacent)
            {
                if (provinceParser.land)
                {
                    list.Add(provinceParser);
                }
                else
                {
                    provinceParser.GetAdjacentLand(i - 1, list);
                }
            }

            return list;;
        }
    }
}