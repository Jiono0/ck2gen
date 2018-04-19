namespace CrusaderKingsStoryGen.Parsers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class CultureGroupParser : Parser
    {
        public List<CultureParser> Cultures = new List<CultureParser>();
        internal string chosenGfx;
        public int r;
        public int b;
        public int g;
        public List<GovermentPolicyHelper> Governments = new List<GovermentPolicyHelper>();
        private string preferedSucc;
        private string preferedGender;

        public string PreferedSuccession
        {
            get
            {
                if (this.preferedSucc == null)
                {
                    switch (RandomIntHelper.Next(10))
                    {
                        case 0:
                        case 1:
                            this.preferedSucc = "succ_gavelkind";
                            break;

                        case 2:
                        case 3:
                            this.preferedSucc = "succ_primogeniture";
                            break;

                        case 4:
                            this.preferedSucc = "succ_feudal_elective";
                            break;

                        case 5:
                            this.preferedSucc = "succ_tanistry";
                            break;

                        case 6:
                            this.preferedSucc = "succ_ultimogeniture";
                            break;

                        case 7:
                            this.preferedSucc = "succ_seniority";
                            break;

                        case 8:
                            this.preferedSucc = "succ_elective_gavelkind";
                            break;

                        case 9:
                            this.preferedSucc = "succ_open_elective";
                            break;
                    }
                }

                return this.preferedSucc;
            }
        }

        public string PreferedGenderLaw
        {
            get
            {
                if (this.preferedGender == null)
                {
                    switch (RandomIntHelper.Next(6))
                    {
                        case 0:
                        case 1:
                        case 2:
                            this.preferedGender = "agnatic_succession";
                            break;

                        case 3:
                        case 4:
                        case 5:
                            this.preferedGender = "cognatic_succession";
                            break;
                    }
                }

                return this.preferedGender;
            }
        }

        public CultureGroupParser(ScriptScope scope)
            : base(scope)
        {
            if (scope.UnsavedData.ContainsKey("color"))
            {
                var col = (Color)this.Scope.UnsavedData["color"];

                this.r = col.R;
                this.g = col.G;
                this.b = col.B;
            }

            foreach (var scriptScope in scope.Scopes)
            {
                if (CultureManager.instance.CultureMap.ContainsKey(scriptScope.Name))
                {
                    this.Cultures.Add(CultureManager.instance.CultureMap[scriptScope.Name]);
                }
            }
        }

        public ReligionGroupParser ReligionGroup { get; set; }

        public void RemoveCulture(string name)
        {
            var r = CultureManager.instance.CultureMap[name];
            this.Cultures.Remove(r);
            this.Scope.Remove(r);
        }

        public void AddCulture(CultureParser r)
        {
            r.group = this;
            if (r.Group != null)
            {
                r.Group.Scope.Remove(r.Scope);
            }

            this.Scope.Add(r.Scope);
            this.Cultures.Add(r);
            this.Cultures = this.Cultures.Distinct().ToList();
        }

        public void AddTreeNode(TreeView inspectTree)
        {
            var res = inspectTree.Nodes.Add(this.Name.Lang());
            res.Tag = this;
            res.ImageIndex = 0;
            foreach (var religionParser in this.Cultures)
            {
                if (religionParser.LanguageName == "")
                {
                    continue;
                }

                var res2 = res.Nodes.Add(religionParser.LanguageName);
                res2.Tag = religionParser;
                res2.ImageIndex = 0;
            }
        }

        public CultureParser AddCulture(string name)
        {
            string langName = "";
            if (name != "norse")
            {
                string oname = name;
                name = StarHelper.SafeName(name);

                LanguageManager.instance.Add(name, oname);
                langName = oname;
            }

            ScriptScope scope = new ScriptScope();
            scope.Name = name;

            this.Scope.Add(scope);
            CultureParser r = new CultureParser(scope, this);
            CultureManager.instance.AllCultures.Add(r);
            r.LanguageName = langName;
            this.Cultures.Add(r);
            CultureManager.instance.CultureMap[name] = r;
            r.Name = name;
            r.Init();
            r.group = this;
            var col = this.Col();
            r.r = col.R;
            r.g = col.G;
            r.b = col.B;
            this.Scope.SetChild(r.Scope);
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

        public void Init()
        {
            this.Scope.Clear();
            this.chosenGfx = CultureParser.GetRandomCultureGraphics();
            this.Scope.Do(@"

	            graphical_cultures = {
                  " + this.chosenGfx + @"
                }
");

            this.r = RandomIntHelper.Next(255);
            this.g = RandomIntHelper.Next(255);
            this.b = RandomIntHelper.Next(255);

            this.Scope.UnsavedData["color"] = Color.FromArgb(255, this.r, this.g, this.b);
        }

        public void RemoveProvince(ProvinceParser provinceParser)
        {
            this.Provinces.Remove(provinceParser);
        }

        public void AddProvince(ProvinceParser provinceParser)
        {
            if (!this.Provinces.Contains(provinceParser))
            {
                this.Provinces.Add(provinceParser);
            }
        }

        public List<ProvinceParser> Provinces = new List<ProvinceParser>();
    }

}
