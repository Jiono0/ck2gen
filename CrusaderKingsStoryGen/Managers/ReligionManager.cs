// <copyright file="ReligionManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    class ReligionManager : ISerializeXml
    {
        public static ReligionManager instance = new ReligionManager();
        private Script script;
        public List<ReligionParser> AllReligions = new List<ReligionParser>();
        public Dictionary<string, ReligionParser> ReligionMap = new Dictionary<string, ReligionParser>();
        public List<ReligionGroupParser> AllReligionGroups = new List<ReligionGroupParser>();

        public ReligionManager()
        {
        }

        public Script Script
        {
            get { return this.script; }
            set { this.script = value; }
        }

        public Dictionary<string, ReligionGroupParser> GroupMap = new Dictionary<string, ReligionGroupParser>();

        public ReligionGroupParser AddReligionGroup(string name)
        {
            ScriptScope scope = new ScriptScope();
            scope.Name = name;

            this.script.Root.Add(scope);

            ReligionGroupParser r = new ReligionGroupParser(scope);

            r.Init();
            this.GroupMap[name] = r;
            this.AllReligionGroups.Add(r);

            ScripterTriggerManager.instance.AddTrigger(r);
            return r;
        }


        public void DoReligiousEquivelents()
        {
            if (!this.SaveReligions)
            {
                return;
            }

            if (this.AllReligionGroups.Count == 1)
            {
                return;
            }

            ReligionGroupParser biggestGroup = null;
            ReligionGroupParser secondGroup = null;
            ReligionGroupParser thirdGroup = null;
            ReligionGroupParser fourthGroup = null;
            ReligionGroupParser fifthGroup = null;
            ReligionGroupParser sixthGroup = null;
            this.AllReligionGroups.Sort(this.SortByBelievers);

            if (this.AllReligionGroups.Count >= 6)
            {
                this.ChristianGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count) / 2];
                this.ChristianGroupSub.Name = this.ChristianGroupSub.Scope.Name;
                this.AllReligionGroups.Remove(this.ChristianGroupSub);
                this.MuslimGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)/2];
                this.MuslimGroupSub.Name = this.MuslimGroupSub.Scope.Name;
                this.AllReligionGroups.Remove(this.MuslimGroupSub);
                this.IndianGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)/2];
                this.IndianGroupSub.Name = this.IndianGroupSub.Scope.Name;
                this.AllReligionGroups.Remove(this.IndianGroupSub);
                this.ZoroGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)/2];
                this.ZoroGroupSub.Name = this.ZoroGroupSub.Scope.Name;
                this.AllReligionGroups.Remove(this.ZoroGroupSub);
                this.PaganGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)/2];
                this.PaganGroupSub.Name = this.PaganGroupSub.Scope.Name;
                this.AllReligionGroups.Remove(this.PaganGroupSub);
                this.JewGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)/2];
                this.JewGroupSub.Name = this.JewGroupSub.Scope.Name;
                this.AllReligionGroups.Remove(this.JewGroupSub);

                while (this.ZoroGroupSub == this.MuslimGroupSub)
                {
                    this.ZoroGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                }

                while (this.ChristianGroupSub == this.MuslimGroupSub)
                {
                    this.ChristianGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                }

                while (this.JewGroupSub == this.MuslimGroupSub)
                {
                    this.JewGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                }
            }
            else
            {
                if (this.AllReligionGroups.Count > 3)
                {
                    this.ChristianGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count/2)];
                    this.ChristianGroupSub.Name = this.ChristianGroupSub.Scope.Name;
                    this.AllReligionGroups.Remove(this.ChristianGroupSub);
                    this.MuslimGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count / 2)];
                    this.MuslimGroupSub.Name = this.MuslimGroupSub.Scope.Name;
                    this.AllReligionGroups.Remove(this.MuslimGroupSub);
                }
                else
                {
                    this.ChristianGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                    this.ChristianGroupSub.Name = this.ChristianGroupSub.Scope.Name;
                    this.MuslimGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                    this.MuslimGroupSub.Name = this.MuslimGroupSub.Scope.Name;
                }

                this.IndianGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                this.IndianGroupSub.Name = this.IndianGroupSub.Scope.Name;
                this.ZoroGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                this.ZoroGroupSub.Name = this.ZoroGroupSub.Scope.Name;
                this.PaganGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                this.PaganGroupSub.Name = this.PaganGroupSub.Scope.Name;
                this.JewGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                this.JewGroupSub.Name = this.JewGroupSub.Scope.Name;

                while (this.ZoroGroupSub == this.MuslimGroupSub)
                {
                    this.ZoroGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                }

                while (this.ChristianGroupSub == this.MuslimGroupSub)
                {
                    this.ChristianGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                }

                while (this.JewGroupSub == this.MuslimGroupSub)
                {
                    this.JewGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
                }
            }

            this.ChristianGroupSub.Religions.Sort(this.SortByBelieversReligion);

            this.CatholicSub = this.ChristianGroupSub.Religions[0];
        //    this.CatholicSub.Name = this.CatholicSub.Scope.Name;
            this.OrthodoxSub = this.ChristianGroupSub.Religions[1];
         //   this.OrthodoxSub.Name = this.OrthodoxSub.Scope.Name;
            this.MuslimGroupSub.Religions.Sort(this.SortByBelieversReligion);

            this.SunniEquiv = this.MuslimGroupSub.Religions[0];
        //    this.SunniEquiv.Name = this.SunniEquiv.Scope.Name;
            this.ShiiteEquiv = this.MuslimGroupSub.Religions[1];
         //   this.ShiiteEquiv.Name = this.ShiiteEquiv.Scope.Name;

            while (this.IndianGroupSub.Religions.Count < 3)
            {
                this.IndianGroupSub = this.AllReligionGroups[RandomIntHelper.Next(this.AllReligionGroups.Count)];
            //    this.IndianGroupSub.Name = this.IndianGroupSub.Scope.Name;
            }

            {
                this.HinduEquiv = this.IndianGroupSub.Religions[0];
            //    this.HinduEquiv.Name = this.HinduEquiv.Scope.Name;
                this.BuddhistEquiv = this.IndianGroupSub.Religions[1];
            //    this.BuddhistEquiv.Name = this.BuddhistEquiv.Scope.Name;
                this.JainEquiv = this.IndianGroupSub.Religions[2];
           //     this.JainEquiv.Name = this.JainEquiv.Scope.Name;
            }

            this.PaganGroupSub.Religions.Sort(this.SortByBelieversReligion);
            this.NorseSub = this.PaganGroupSub.Religions[0];
          //  this.NorseSub.Name = this.NorseSub.Scope.Name;
            this.NorseReformSub = this.PaganGroupSub.Religions[1];
         //   this.NorseReformSub.Name = this.NorseSub.Scope.Name;

            foreach (var religionParser in this.PaganGroupSub.Religions)
            {
                religionParser.polytheism = true;
                religionParser.hasLeader = false;
            }

            foreach (var religionParser in this.ChristianGroupSub.Religions)
            {
                religionParser.hasLeader = true;
            }

            this.NorseSub.allow_viking_invasion = true;
            this.NorseSub.allow_looting = true;
            this.JainEquiv.pacifist = false;
            this.JainEquiv.can_call_crusade = true;
            this.HinduEquiv.pacifist = true;
            this.HinduEquiv.can_call_crusade = false;
            this.BuddhistEquiv.pacifist = true;
            this.BuddhistEquiv.can_call_crusade = false;

            foreach (var religionParser in this.AllReligions)
            {
                religionParser.RedoReligionScope();
            }
        }

        public ReligionParser JainEquiv { get; set; }

        public ReligionParser BuddhistEquiv { get; set; }

        public ReligionParser HinduEquiv { get; set; }

        public ReligionParser OrthodoxSub { get; set; }

        public ReligionParser ShiiteEquiv { get; set; }

        public ReligionParser SunniEquiv { get; set; }

        public ReligionParser NorseSub { get; set; }

        public ReligionParser CatholicSub { get; set; }

        public ReligionParser NorseReformSub { get; set; }


        public void Save()
        {
            if (!this.SaveReligions)
            {
                return;
            }

            int biggest = -1;

            LanguageManager.instance.SetupReligionEventSubsitutions();

            var list = new List<ScriptScope>();
            foreach (var child in this.script.Root.Children)
            {
                list.AddRange((child as ScriptScope)?.Children.Where((o => o is ScriptScope)).Where(o => !((ScriptScope)o).Name.Contains("_names")).Cast<ScriptScope>());
            }

            foreach (var religionParser in this.AllReligions)
            {
                if(religionParser.Provinces.Count > 0)
                {
                    religionParser.CreateSocietyDetails(religionParser.Provinces[0].Culture.Name);
                }
            }

            foreach (var scriptScope in list)
            {
                string name = (scriptScope as ScriptScope)?.Name;
                if (this.AllReligions.All(c => c.Name != name))
                {
                    System.Console.Out.WriteLine("Cannot find " + name);
                }
            }

            this.script.Save();
        }

        public int SortByBelievers(ReligionGroupParser x, ReligionGroupParser y)
        {
            if (x.Provinces.Count > y.Provinces.Count)
            {
                return -1;
            }

            if (x.Provinces.Count < y.Provinces.Count)
            {
                return 1;
            }

            return 0;
        }

        private int SortByBelieversReligion(ReligionParser x, ReligionParser y)
        {
            if (x.Provinces.Count > y.Provinces.Count)
            {
                return -1;
            }

            if (x.Provinces.Count < y.Provinces.Count)
            {
                return 1;
            }

            return 0;
        }

        public ReligionGroupParser PaganGroupSub { get; set; }

        public ReligionGroupParser JewGroupSub { get; set; }

        public ReligionGroupParser MuslimGroupSub { get; set; }

        public ReligionGroupParser ChristianGroupSub { get; set; }

        public ReligionGroupParser IndianGroupSub { get; set; }

        public ReligionGroupParser ZoroGroupSub { get; set; }

        public void Init()
        {
            LanguageManager.instance.Add("norse", StarHelper.Generate(RandomIntHelper.Next(1000000)));
            LanguageManager.instance.Add("pagan", StarHelper.Generate(RandomIntHelper.Next(1000000)));
            LanguageManager.instance.Add("christian", StarHelper.Generate(RandomIntHelper.Next(1000000)));

            Script s = new Script();
            this.script = s;
            s.Name = Globals.ModDir + "common\\religions\\00_religions.txt";
            s.Root = new ScriptScope();
            ReligionGroupParser r = this.AddReligionGroup("pagan");
            r.Init();
            var pagan = r.AddReligion("pagan");

            pagan.CreateRandomReligion(null);

            this.AllReligionGroups.Add(r);
        }

        public ReligionParser BranchReligion(string religion,  string culture)
        {
            var rel = this.ReligionMap[religion];
            var group = rel.Group;
            int totCount = 0;
            foreach (var religionParser in rel.Group.Religions)
            {
                totCount += religionParser.Provinces.Count;
            }

            if (!CultureManager.instance.allowMultiCultureGroups)
            {
                string name = StarHelper.Generate(culture);
                string safe = StarHelper.SafeName(name);
                while (ReligionManager.instance.GroupMap.ContainsKey(safe))
                {
                    name = StarHelper.Generate(culture);
                    safe = StarHelper.Generate(culture);
                }

                LanguageManager.instance.Add(safe, name);
                group = this.AddReligionGroup(safe);
                name = StarHelper.Generate(culture);
                while (ReligionManager.instance.ReligionMap.ContainsKey(safe))
                {
                    name = StarHelper.Generate(culture);
                    safe = StarHelper.SafeName(name);
                }

                var rel2 = group.AddReligion(name);
                 rel2.RandomReligionProperties();
                rel2.CreateRandomReligion(group);
                return rel2;
            }
            else
            {
                var rell = StarHelper.Generate(culture);

                while (ReligionManager.instance.ReligionMap.ContainsKey(StarHelper.SafeName(rell)))
                {
                    rell = StarHelper.Generate(culture);
                }

                var rel2 = group.AddReligion(rell);

                rel2.RandomReligionProperties();
                rel2.CreateRandomReligion(group);

                rel2.Mutate(rel, CultureManager.instance.CultureMap[culture], 6);
                return rel2;
            }
        }

        public void SaveProject(XmlWriter writer)
        {
            writer.WriteStartElement("religions");

            foreach (var religionGroupParser in this.AllReligionGroups)
            {
                writer.WriteStartElement("group");

                religionGroupParser.SaveXml(writer);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        public void LoadProject(XmlReader reader)
        {
        }

        public void LoadVanilla()
        {
            this.SaveReligions = false;
            var files = ModManager.instance.GetFileKeys("common\\religions");
            foreach (var file in files)
            {
                Script s = ScriptLoader.instance.LoadKey(file);
                foreach (var rootChild in s.Root.Children)
                {
                    if ((rootChild as ScriptScope).Name == "secret_religion_visibility_trigger")
                    {
                        continue;
                    }

                    ReligionGroupParser p = new ReligionGroupParser(rootChild as ScriptScope);

                    this.AllReligionGroups.Add(p);
                    foreach (var scopeChild in p.Scope.Children)
                    {
                        if (scopeChild is ScriptScope)
                        {
                            var sc = scopeChild as ScriptScope;

                            if (sc.Name == "male_names" ||
                                sc.Name == "female_names")
                            {
                                continue;
                            }

                            ReligionParser r = new ReligionParser(sc);
                            this.AllReligions.Add(r);
                                this.ReligionMap[r.Name] = r;
                            p.Religions.Add(r);
                            r.Group = p;
                            r.LanguageName = LanguageManager.instance.Get(r.Name);
                        }
                    }
                }
            }
        }

        public bool SaveReligions { get; set; } = true;
    }
}
