using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace CrusaderKingsStoryGen
{
    class ReligionGroupParser : Parser
    {
        public List<ReligionParser> Religions = new List<ReligionParser>();

        public void RemoveProvince(ProvinceParser provinceParser)
        {
            Provinces.Remove(provinceParser);
        }

        public List<ProvinceParser> Provinces = new List<ProvinceParser>();

        public void AddProvince(ProvinceParser provinceParser)
        {
            if (!Provinces.Contains(provinceParser))
                Provinces.Add(provinceParser);
        }
        public ReligionGroupParser(ScriptScope scope)
            : base(scope)
        {
            foreach (var scriptScope in scope.Scopes)
            {
                if (ReligionManager.instance.ReligionMap.ContainsKey(scriptScope.Name))
                    Religions.Add(ReligionManager.instance.ReligionMap[scriptScope.Name]);
            }
        }

        public override ScriptScope CreateScope()
        {
            return null;
        }

        public void RemoveReligion(String name)
        {
            var r = ReligionManager.instance.ReligionMap[name];
            Religions.Remove(r);
            Scope.Remove(r);
        }

        public void AddReligion(ReligionParser r)
        {
            if (r.Group != null)
            {
                r.Group.Scope.Remove(r.Scope);
            }
            Scope.Add(r.Scope);
            Religions.Add(r);
        }

        public ReligionParser AddReligion(String name, String orig = null)
        {
            if (name != "urtru")
            {
                String oname = name;
                name = StarNames.SafeName(name);

                LanguageManager.instance.Add(name, oname);
                orig = oname;
            }

            ScriptScope scope = new ScriptScope
            {
                Name = name
            };

            Scope.Add(scope);

            ReligionParser r = new ReligionParser(scope);
            ReligionManager.instance.AllReligions.Add(r);

            if (orig != null)
            {
                r.LanguageName = orig;
            }

            r.Name = r.Scope.Name;
            r.Group = this;

            var col= Col();

            r.r = col.R;
            r.g = col.G;
            r.b = col.B;

            Religions.Add(r);
            ReligionManager.instance.ReligionMap[name] = r;

            Console.Out.WriteLine(r.Name + " added");

            return r;
        }

        private Color Col()
        {
            var r = color.R + Rand.Next(-40, 40);
            if (r > 255)
                r = 255;
            if (r < 0)
                r = 0;
            var g = color.G + Rand.Next(-40, 40);
            if (g > 255)
                g = 255;
            if (g < 0)
                g = 0;

            var b = color.B + Rand.Next(-40, 40);
            if (b > 255)
                b = 255;
            if (b < 0)
                b = 0;
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
            color = Color.FromArgb(255, Rand.Next(255), Rand.Next(255), Rand.Next(255));

            hostile_within_group = Rand.Next(2) == 0;
            String g = gfx[Rand.Next(gfx.Count())];
            Scope.Clear();
            Scope.Do(@"

	            has_coa_on_barony_only = yes
	            graphical_culture = " + g + @"
	            playable = yes
	            hostile_within_group = " + (hostile_within_group ? "yes" : "no") + @"
");

        }

        public HashSet<ProvinceParser> holySites = new HashSet<ProvinceParser>();

        public void TryFillHolySites()
        {
            if (holySites.Count >= 5)
            {
                while (holySites.Count > 5)
                {
                    holySites.ToArray()[0].Title.Scope.Remove(holySites.ToArray()[0].Title.Scope.Find("holy_site"));
                    holySites.Remove(holySites.ToArray()[0]);
                }

                return;
            }

            while (holySites.Count < 5)
            {
                if (holySites.Count == Provinces.Count)
                    break;

                var chosen = Provinces[Rand.Next(Provinces.Count)];

                if (holySites.Contains(chosen))
                    continue;

                chosen.templeRequirement++;
     
                holySites.Add(chosen);
            }
        }

        public void DoEquivelent(string name)
        {
            EquivelentReligion = name;
        }

        public string EquivelentReligion { get; set; }

        public void AddTreeNode(TreeView inspectTree)
        {
            var res = inspectTree.Nodes.Add(Name.Lang());
            res.Tag = this;
            res.ImageIndex = 0;
            foreach (var religionParser in Religions)
            {
                var res2 = res.Nodes.Add(religionParser.LanguageName);
                res2.Tag = religionParser;
                res2.ImageIndex = 0;
            }
        }

        public void SaveXml(XmlWriter writer)
        {
            writer.WriteElementString("name", Name);
            foreach (var religionParser in Religions)
            {
                writer.WriteStartElement("religion");
                religionParser.SaveXml(writer);
                writer.WriteEndElement();
            }
        }
    }
}