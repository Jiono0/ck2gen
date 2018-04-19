// <copyright file="ReligionGroupParser.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Parsers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using System.Xml;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class ReligionGroupParser : Parser
    {
        public List<ReligionParser> Religions = new List<ReligionParser>();

        public void RemoveProvince(ProvinceParser provinceParser)
        {
            this.Provinces.Remove(provinceParser);
        }

        public List<ProvinceParser> Provinces = new List<ProvinceParser>();

        public void AddProvince(ProvinceParser provinceParser)
        {
            if (!this.Provinces.Contains(provinceParser))
            {
                this.Provinces.Add(provinceParser);
            }
        }

        public ReligionGroupParser(ScriptScope scope)
            : base(scope)
        {
            foreach (var scriptScope in scope.Scopes)
            {
                if (ReligionManager.instance.ReligionMap.ContainsKey(scriptScope.Name))
                {
                    this.Religions.Add(ReligionManager.instance.ReligionMap[scriptScope.Name]);
                }
            }
        }

        public void RemoveReligion(string name)
        {
            var r = ReligionManager.instance.ReligionMap[name];
            this.Religions.Remove(r);
            this.Scope.Remove(r);
        }

        public void AddReligion(ReligionParser r)
        {
            if (r.Group != null)
            {
                r.Group.Scope.Remove(r.Scope);
            }

            this.Scope.Add(r.Scope);
            this.Religions.Add(r);
        }

        public ReligionParser AddReligion(string name, string orig = null)
        {
            if (name != "pagan")
            {
                string oname = name;
                name = StarHelper.SafeName(name);


                LanguageManager.instance.Add(name, oname);
                orig = oname;
            }

            ScriptScope scope = new ScriptScope();
            scope.Name = name;
            this.Scope.Add(scope);

            ReligionParser r = new ReligionParser(scope);
            ReligionManager.instance.AllReligions.Add(r);
            if (orig != null)
            {
                r.LanguageName = orig;
            }

            r.Name = r.Scope.Name;
            r.Group = this;
            var col= this.Col();
            r.r = col.R;
            r.g = col.G;
            r.b = col.B;
            this.Religions.Add(r);
            ReligionManager.instance.ReligionMap[name] = r;
            System.Console.Out.WriteLine(r.Name + " added");

            //  TraitManager.instance.AddReligiousTraits(r);
            return r;
        }

        private Color Col()
        {
            var r = this.color.R + RandomIntHelper.Next(-40, 40);
            if (r > 255)
            {
                r = 255;
            }

            if (r < 0)
            {
                r = 0;
            }

            var g = this.color.G + RandomIntHelper.Next(-40, 40);
            if (g > 255)
            {
                g = 255;
            }

            if (g < 0)
            {
                g = 0;
            }

            var b = this.color.B + RandomIntHelper.Next(-40, 40);
            if (b > 255)
            {
                b = 255;
            }

            if (b < 0)
            {
                b = 0;
            }

            return Color.FromArgb(255, r, g, b);
        }

        public Color color;

        static string[] gfx = new string[]
            {
                "muslimgfx",
                "westerngfx",
                "norsegfx",
                "mongolgfx",
                "mesoamericangfx",
                "africangfx",
                "persiangfx",
                "jewishgfx",
                "indiangfx",
                "hindugfx",
                "buddhistgfx",
                "jaingfx",
            };

        public bool hostile_within_group = false;

        public void Init()
        {
            this.color = Color.FromArgb(255, RandomIntHelper.Next(255), RandomIntHelper.Next(255), RandomIntHelper.Next(255));

            this.hostile_within_group = RandomIntHelper.Next(2) == 0;
            string g = gfx[RandomIntHelper.Next(gfx.Count())];
            this.Scope.Clear();
            this.Scope.Do(@"

	            has_coa_on_barony_only = yes
	            graphical_culture = " + g + @"
	            playable = yes
	            hostile_within_group = " + (this.hostile_within_group ? "yes" : "no") + @"
	
	            # Names given only to Pagan characters (base names)
	            male_names = {
		            Anund Asbjörn Aslak Audun Bagge Balder Brage Egil Emund Frej Gnupa Gorm Gudmund Gudröd Hardeknud Helge Odd Orm 
		            Orvar Ottar Rikulfr Rurik Sigbjörn Styrbjörn Starkad Styrkar Sämund Sölve Sörkver Thorolf Tjudmund Toke Tolir 
		            Torbjörn Torbrand Torfinn Torgeir Toste Tyke
	            }
	            female_names = {
		            Aslaug Bothild Björg Freja Grima Gytha Kráka Malmfrid Thora Thordis Thyra Ragnfrid Ragnhild Svanhild Ulvhilde
	            }

");
        }

        public HashSet<ProvinceParser> holySites = new HashSet<ProvinceParser>();

        public void TryFillHolySites()
        {
            if (this.Name == "aranahap")
            {
            }

            if (this.holySites.Count >= 5)
            {
                if (this.Name == "aranahap")
                {
                }

                while (this.holySites.Count > 5)
                {
                    this.holySites.ToArray()[0].Title.Scope.Remove(this.holySites.ToArray()[0].Title.Scope.Find("holy_site"));
                    this.holySites.Remove(this.holySites.ToArray()[0]);
                }

                return;
            }

            while (this.holySites.Count < 5)
            {
                if (this.holySites.Count == this.Provinces.Count)
                {
                    break;
                }

                var chosen = this.Provinces[RandomIntHelper.Next(this.Provinces.Count)];
                if (this.holySites.Contains(chosen))
                {
                    continue;
                }

                chosen.templeRequirement++;

                this.holySites.Add(chosen);
            }
        }

        public void DoEquivelent(string name)
        {
            this.EquivelentReligion = name;
        }

        public string EquivelentReligion { get; set; }

        public void AddTreeNode(TreeView inspectTree)
        {
            var res = inspectTree.Nodes.Add(this.Name.Lang());
            res.Tag = this;
            res.ImageIndex = 0;
            foreach (var religionParser in this.Religions)
            {
                var res2 = res.Nodes.Add(religionParser.LanguageName);
                res2.Tag = religionParser;
                res2.ImageIndex = 0;
            }
        }

        public void SaveXml(XmlWriter writer)
        {
            writer.WriteElementString("name", this.Name);
            foreach (var religionParser in this.Religions)
            {
                writer.WriteStartElement("religion");
                religionParser.SaveXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}