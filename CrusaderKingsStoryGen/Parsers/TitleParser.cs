// <copyright file="TitleParser.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using CrusaderKingsStoryGen.ScriptHelpers;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.Helpers;

    public class TitleParser : Parser
    {
        public int Rank = 1;
        public Dictionary<string, TitleParser> SubTitles = new Dictionary<string, TitleParser>();
        public List<ScriptScope> titleScripts = new List<ScriptScope>();

        public void SetCapital(ProvinceParser cap)
        {
            if(this.Scope.Find("capital")!=null)
            {
                this.Scope.Remove(this.Scope.Find("capital"));
            }

            this.Scope.Add(new ScriptCommand() { Name = "capital", Value = cap.id });
        }

        public List<TitleParser> CurrentSubTitles = new List<TitleParser>();
        public float FinalTechLevel = 0.0f;

        public Rectangle Bounds
        {
            get
            {
                if (this.dirty)
                {
                    this._bounds = this.GetBounds();
                }

                if (!this._bounds.IsEmpty)
                {
                    this.dirty = false;
                }

                return this._bounds;
            }
        }

        public Point TextPos
        {
            get
            {
                if (this.dirty)
                {
                    this._bounds = this.GetBounds();
                }

                this.dirty = false;

                return this._textPos;
            }

            set { this._textPos = value; }
        }

        public Rectangle GetBounds()
        {
            var prov = this.GetAllProvinces();

            float avx = 0;
            float avy = 0;
            if (this.Rank == 1 && this.Holder != null && this.Holder.PrimaryTitle.Rank == 1)
            {
            }

            Rectangle tot = Rectangle.Empty;
            foreach (var provinceParser in prov)
            {
                int cx = provinceParser.Bounds.X + (provinceParser.Bounds.Width/2);
                int cy = provinceParser.Bounds.Y + (provinceParser.Bounds.Height/2);

                avx += cx;
                avy += cy;


                if (tot.Width == 0)
                    tot = provinceParser.Bounds;
                else
                {
                    if (tot.Left > provinceParser.Bounds.Left)
                    {
                        int right = tot.Right;
                        tot.X = provinceParser.Bounds.Left;
                        tot.Width = (right - tot.X);
                    }

                    if (tot.Top > provinceParser.Bounds.Top)
                    {
                        int right = tot.Top;
                        tot.Y = provinceParser.Bounds.Top;
                        tot.Height = (right - tot.Y);
                    }

                    if (tot.Right < provinceParser.Bounds.Right)
                    {
                        tot.Width = provinceParser.Bounds.Right - tot.X;
                    }

                    if (tot.Bottom < provinceParser.Bounds.Bottom)
                    {
                        tot.Height = provinceParser.Bounds.Bottom - tot.Y;
                    }
                }
            }

            avx /= prov.Count;
            avy /= prov.Count;
            this.TextPos = new Point((int) avx, (int) avy);
            return tot;
        }

        public TitleParser Liege
        {
            get
            {
                if (this._liege == this)
                {
                    return null;
                }

                return this._liege;
            }

            private set
            {
                if (this.Name == "k_armenia")
                {
                }

                if (value != null && value.Holder != null && value.Holder.PrimaryTitle.Rank <= this.Rank)
                {
                    return;
                }

                if (value != null && value.Rank <= this.Rank)
                {
                    return;
                }

                if (value != null)
                {
                    if (value.Active == false)
                    {
                        return;
                    }
                }

                if (this._liege != value)
                {
                    if (this._liege != null)
                    {
                        this._liege.dirty = true;

                        this._liege.SubTitles.Remove(this.Name);
                    }
                }

                this._liege = value;
                if (this._liege != null)
                {
                    if (!this._liege.SubTitles.ContainsKey(this.Name))
                    {
                        this._liege.dirty = true;
                        this._liege.SubTitles[this.Name] = this;
                    }
                }

                if (this.Liege != null && SimulationManager.instance.AutoValidateRealm && !bSkipValidate)
                {
                    this.Liege.ValidateRealm(new List<CharacterParser>(), this.TopmostTitle);
                }
            }
        }

        private static bool bSkipValidate = false;

        public TitleParser LiegeDirect
        {
            get
            {
                if (this._liege == this)
                {
                    return null;
                }

                return this._liege;
            }

             set
            {
                if (value != null && value.Holder != null && value.Holder.PrimaryTitle.Rank <= this.Rank)
                {
                    return;
                }

                if (value != null && value.Rank <= this.Rank)
                {
                    return;
                }

                if (this._liege != value)
                {
                    if (this._liege != null)
                    {
                        this._liege.dirty = true;

                        this._liege.SubTitles.Remove(this.Name);
                    }
                }

                this._liege = value;
                if (this._liege != null)
                {
                    if (!this._liege.SubTitles.ContainsKey(this.Name))
                    {
                        this._liege.dirty = true;
                        this._liege.SubTitles[this.Name] = this;
                    }
                }

                if (this.Liege != null && SimulationManager.instance.AutoValidateRealm)
                {
                    this.Liege.ValidateRealm(new List<CharacterParser>(), this.TopmostTitle);
                }
            }
        }

        public void ValidateRealm(List<CharacterParser> listOfCharacters, TitleParser expectedHead )
        {
            if (this.Holder != null)
            {
                listOfCharacters.Add(this.Holder);


                var list = this.Holder.Titles.ToList();
                foreach (var titleParser in list)
                {
                    if (titleParser.Rank < this.Holder.PrimaryTitle.Rank && titleParser != this.Holder.PrimaryTitle )
                    {
                        if (titleParser.Liege != null && titleParser.Liege.Holder != this.Holder)
                        {
                            if (titleParser.Liege != null && titleParser.Liege.Holder != this.Holder)
                            {
                                titleParser.Log("Setting to vassal of " + this.Holder.PrimaryTitle + " in Validate Realm");
                                titleParser.DoSetLiegeEvent(this.Holder.PrimaryTitle);
                                continue;
                            }


                            if (titleParser.HasLiegeInChain(this.Holder.PrimaryTitle.Liege) || this.Holder.PrimaryTitle.HasLiegeInChain(titleParser.Liege))
                            {
                                continue;
                            }

                            titleParser.DoSetLiegeEvent(this.Holder.PrimaryTitle);
                        }
                    }
                    else if (titleParser.Rank == this.Holder.PrimaryTitle.Rank && titleParser != this.Holder.PrimaryTitle && titleParser.Liege != this.Holder.PrimaryTitle.Liege)
                    {
                        if (titleParser.Liege != null && titleParser.Liege.Holder != this.Holder)
                        {
                            if (titleParser.Liege != null)
                            {
                                if (titleParser.Name == "d_holland")
                                {
                                }

                                titleParser.Log("Setting independent in Validate Realm");
                                titleParser.DoSetLiegeEvent(null);
                            }
                        }
                    }
                }
            }

            foreach (var titleParser in this.SubTitles.ToArray())
            {
                titleParser.Value.ValidateRealm(new List<CharacterParser>(), this.TopmostTitle);
            }
        }

        internal void Log(string text)
        {
            if (this.Rank >= 3 || this.Liege == null)
            {
                EventLogHelper.instance.AddTitle(this.Name);
            }

            EventLogHelper.instance.Log(this.Name, text);

            if (this.Liege != null)
            {
                EventLogHelper.instance.Log(this.Liege.Name, "vassal: " + this.Name + " - " + text);
                EventLogHelper.instance.Log(this.TopmostTitle.Name, "sub vassal: " + this.Name + " - " + text);
            }
        }

        public TitleParser ConquerLiege { get; set; }

        public GovermentPolicyHelper Government { get; set; }

        public bool rebel { get; set; }

        public bool landless { get; set; }

        public bool primary { get; set; }

        public string culture { get; set; }

        public bool tribe { get; set; }

        public Color color { get; set; }

        public Color color2 { get; set; }

        public int capital { get; set; }

        public int dignity { get; set; }

        public bool Active { get; set; }

        public List<ProvinceParser> Owns = new List<ProvinceParser>();
        private CharacterParser _holder;
        public List<TitleParser> AdjacentToTitle = new List<TitleParser>();
        public HashSet<TitleParser> AdjacentToTitleSet = new HashSet<TitleParser>();
        private TitleParser _liege;

        public void RenameForCulture(CultureParser culture)
        {
            string namem = null;
            do
            {
                var name = culture.dna.GetPlaceName();
                namem = name;
            } while (this.Rename(namem));

            if (this.Rank == 1 && this.Owns.Count > 0)
            {
                this.Owns[0].Rename(namem);
                this.Owns[0].ProvinceTitle = this.Name;
            }

            //Rename(namem);
        }

        public bool RenameSafe(string name, bool fromProvince = false)
        {
            string oldName = this.Name;
            LanguageManager.instance.Remove(this.Name);
            TitleManager.instance.TitleMap.Remove(oldName);
            this.Name = StarHelper.SafeName(name);


            bool was = TitleManager.instance.TitleMap.ContainsKey(this.Name);
            TitleManager.instance.TitleMap[this.Name] = this;

            if (TitleManager.instance.TieredTitles.ContainsKey(oldName))
            {
                TitleManager.instance.TieredTitles.Remove(oldName);
                TitleManager.instance.TieredTitles[this.Name] = this;

                if (this.Liege != null)
                {
                    this.Liege.SubTitles.Remove(oldName);
                    this.Liege.SubTitles[this.Name] = this;
                }
            }

            this.Scope.Parent.ChildrenMap.Remove(oldName);
            this.Scope.Parent.ChildrenMap[this.Name] = this;


            return was;
        }

        public bool Rename(string name, bool fromProvince = false)
        {
            string oldName = this.Name;
            LanguageManager.instance.Remove(this.Name);
            TitleManager.instance.TitleMap.Remove(oldName);
            this.Name = StarHelper.SafeName(name);
            if (this.Rank == 0)
            {
                this.Name = "b_" + this.Name;
            }

            if (this.Rank == 1)
            {
                this.Name = "c_" + this.Name;
            }

            if (this.Rank == 2)
            {
                this.Name = "d_" + this.Name;
            }

            if (this.Rank == 3)
            {
                this.Name = "k_" + this.Name;
            }

            if (this.Rank == 4)
            {
                this.Name = "e_" + this.Name;
            }

            LanguageManager.instance.Add(this.Name, name);
            bool was = TitleManager.instance.TitleMap.ContainsKey(this.Name);
            TitleManager.instance.TitleMap[this.Name] = this;

            if (TitleManager.instance.TieredTitles.ContainsKey(oldName))
            {
                TitleManager.instance.TieredTitles.Remove(oldName);
                TitleManager.instance.TieredTitles[this.Name] = this;

                if (this.Liege != null)
                {
                    this.Liege.SubTitles.Remove(oldName);
                    this.Liege.SubTitles[this.Name] = this;
                }
            }

            this.Scope.Parent.ChildrenMap.Remove(oldName);
            this.Scope.Parent.ChildrenMap[this.Name] = this;


            return was;
        }

        public void RenameSoft(string name, bool fromProvince = false)
        {
            string oldName = this.Name;
            LanguageManager.instance.Remove(this.Name);

            LanguageManager.instance.Add(this.Name, name);
        }


        public void AddSub(TitleParser sub)
        {
            if (sub.Rank >= this.Rank)
            {
                return;
            }
            //  if (SubTitles.ContainsKey(sub.Name))
            //     return;
            if (this == sub)
            {
                return;
            }

            var liege = sub;
            this.dirty = true;
            while (liege.Liege != null && liege.Liege.Rank > liege.Rank)
            {
                if (liege == this)
                {
                    return;
                }

                liege = liege.Liege;
            }

            if (sub.Liege != null)
            {
                sub.Liege.dirty = true;

                sub.Liege.SubTitles.Remove(sub.Name);
            }

            this.SubTitles[sub.Name] = sub;
            if(sub.Scope.Parent != null)
            {
                sub.Scope.Parent.Remove(sub.Scope);
            }
            else
            {
                TitleManager.instance.LandedTitlesScript.Root.Remove(sub.Scope);
            }

            this.Scope.SetChild(sub.Scope);
            sub.Liege = this;


            {
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";

                thing.Add(new ScriptCommand() { Name = "liege", Value = this.Name });

                sub.titleScripts.Add(thing);
            }

            sub.Dejure = this;
        }

        public TitleParser(ScriptScope scope, bool addToList = true)
            : base(scope)
        {
            string newName = "";
            this.Name = scope.Name;
            if (this.Name == "k_armenia")
            {
            }

            if (this.Name == "d_vastergotland")
            {
            }

            if (this.Name.StartsWith("b_"))
            {
          //      newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture));
                this.Rank = 0;
            }

            if (this.Name.StartsWith("c_"))
            {
           //     newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture));
                this.Rank = 1;
            }

            if (this.Name.StartsWith("d_"))
            {
           //     newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture));
                this.Rank = 2;
            }

            if (this.Name.StartsWith("k_"))
            {
             //   newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture));
                this.Rank = 3;
            }

            if (this.Name.StartsWith("e_"))
            {
               // newName = LanguageManager.instance.Add(Name, StarNames.Generate(culture) + " Empire");
                this.Rank = 4;
            }

            this.color = scope.GetColor("color");

            if (addToList)
            {
                TitleManager.instance.TitleMap[this.Name] = this;
                TitleManager.instance.Titles.Add(this);
            }

            int line = 0;

            foreach (var child in scope.Children)
            {
                if (child is ScriptCommand)
                {
                    this.RegisterProperty(line, ((child as ScriptCommand).Name), child);
                    var com = (child as ScriptCommand);

                    if (com.Name == "capital" && MapManager.instance.IsVanilla)
                    {
                        this.capital = Convert.ToInt32(com.Value);
                        this.DejureCapitalProvince = MapManager.instance.ProvinceIDMap[this.capital];
                    }
                }

                line++;
                if (child is ScriptScope)
                {
                    var subscope = (child as ScriptScope);
                    if (subscope.Name == "c_narke")
                    {
                    }

                    if (subscope.Name == "OR" || subscope.Name == "NOT" || subscope.Name == "AND" ||
                        subscope.Name == "allow" ||
                        subscope.Name == "male_names" ||
                        subscope.Name == "coat_of_arms" ||
                        subscope.Name  == "pagan_coa" ||
                        subscope.Name == "layer")
                    {
                        continue;
                    }

                    this.SubTitles[subscope.Name] = new TitleParser(subscope, addToList);
                    this.SubTitles[subscope.Name].Dejure = this;
                    this.SubTitles[subscope.Name].LiegeDirect = this;
                    if (subscope.Name.StartsWith("b_"))
                    {
                        MapManager.instance.RegisterBarony(subscope.Name, this.SubTitles[subscope.Name]);
                    }
                }
            }

            if (this.capital != 0)
            {
                if (MapManager.instance.ProvinceIDMap.ContainsKey(this.capital))
                {
                    ProvinceParser provinceParser = MapManager.instance.ProvinceIDMap[this.capital];
                    this.CapitalProvince = provinceParser;
                    if (this.Name.StartsWith("c_"))
                    {
                        this.Owns.Add(this.CapitalProvince);
                        this.CapitalProvince.ProvinceTitle = this.Name;
                    }
                }
            }
            else if (MapManager.instance.ProvinceMap.ContainsKey(this.Name) && this.Rank == 1)
            {
                ProvinceParser provinceParser = MapManager.instance.ProvinceMap[this.Name];
                this.CapitalProvince = provinceParser;
                if (!this.Name.StartsWith("d_"))
                {
                    this.Owns.Add(this.CapitalProvince);
                    this.CapitalProvince.ProvinceTitle = this.Name;
                }
            }
        }

        public ProvinceParser DejureCapitalProvince { get; set; }

        public void DoCapital()
        {
        }

        public override string ToString()
        {
            return this.Name;
        }

        public ProvinceParser CapitalProvince
        {
            get
            {
                if (this._capitalProvince == null && !SimulationManager.instance.AllowCapitalPicking)
                {
                    return null;
                }

                if (this._capitalProvince == null && this.GetAllProvincesTitleOnly().Count > 0)
                {
                    if (!this.PickNewCapital())
                    {
                        this.FlagProblem = true;
                        return this._capitalProvince;
                    }
                }

                if (this._capitalProvince != null && this._capitalProvince.ProvinceTitle != null && (this._capitalProvince.Title == this || this._capitalProvince.Title.HasLiegeInChain(this)))
                {
                    return this._capitalProvince;
                }

                if (!this.PickNewCapital())
                {
                    this.FlagProblem = true;
                    this.PickNewCapital();
                }

                return this._capitalProvince;
            }

            set { this._capitalProvince = value; }
        }

        public bool FlagProblem { get; set; }

        public bool PickNewCapital()
        {
            var c = this.GetAllDirectProvinces().OrderBy(o=>o.TempleCount + o.TownCount + o.TownCount).Reverse().ToList();

            if (this.government == "republic")
            {
                c = c.Where(p => p.IsAdjacentToSea() && p.government == "republic").ToList();
            }

            if (c.Count == 0)
            {
                c = this.GetAllProvincesTitleOnly().Where(p=>p.republic == (this.government == "republic")).OrderBy(o => o.TempleCount + o.TownCount + o.CastleCount).Reverse().ToList();
                if (c.Count == 0)
                {
                    return false;
                }

                if (this.Holder != null)
                {
                    this.Holder.GiveTitleSoft(c[0].Title);
                }
            }

            if (c.Count == 0)
            {
                this._capitalProvince = null;
                return false;
            }

            this.CapitalProvince = c[0];

            return true;
        }

        public class HeldDate
        {
            public int date;
            public int chrid;
        }

        public List<string> prevLeaderNames = new List<string>();

        public List<HeldDate> heldDates = new List<HeldDate>();
        public string government = "tribal";


        public CharacterParser Holder
        {
            get { return this._holder; }

            set
            {
                if (this._holder != null && this._holder != value)
                {
                    this._holder.Titles.Remove(this);
                }

                this._holder = value;

                if (value != null)
                {
                    this.culture = this._holder.culture;
                    foreach (var provinceParser in this.Owns)
                    {
                        Color col = value.Color;
                        this.SetProperty("color", col);
                        this.SetProperty("color2", col);
                    }
                }
            }
        }

        public bool Claimed
        {
            get
            {
                var t = this;
                return !(!t.Active || t.Holder == null);
            }
        }

        public int LandedTitlesCount
        {
            get
            {
                int c = 0;
                if (this.Rank == 1)
                {
                    return 1;
                }

                foreach (var titleParser in this.SubTitles)
                {
                    c += titleParser.Value.LandedTitlesCount;
                }

                return c;
            }
        }

        public int DirectLandedTitlesCount
        {
            get
            {
                int c = 0;
                if (this.Rank == 1)
                {
                    return 1;
                }

                return c;
            }
        }

        public int DirectVassalLandedTitlesCount
        {
            get
            {
                int c = 0;
                if (this.Rank == 1)
                {
                    return 1;
                }

                foreach (var titleParser in this.SubTitles)
                {
                    c += titleParser.Value.DirectLandedTitlesCount;
                }

                return c;
            }
        }

        public TitleParser TopmostTitle
        {
            get
            {
                if (this.Holder != null)
                {
                    return this.Holder.TopLiegeCharacter.PrimaryTitle;
                }

                var liege = this.Liege;

                if (liege == null || liege.Holder == null)
                {
                    return this;
                }

                while (liege.Liege != null && liege.Liege.Holder != null && liege.Liege.Rank > liege.Rank)
                {
                    liege = liege.Liege;
                }

                return liege;
            }
        }

        public TitleParser TopmostTitleDirect
        {
            get
            {
                var liege = this.Liege;

                if (liege == null || liege.Holder == null)
                {
                    return this;
                }

                while (liege.Liege != null && liege.Liege.Holder != null && liege.Liege.Rank > liege.Rank)
                {
                    liege = liege.Liege;
                }

                return liege;
            }
        }

        public bool Religious { get; set; }

        public CharacterParser CurrentHolder { get; set; }

        public CultureParser Culture
        {
            get
            {
                if(CultureManager.instance.CultureMap.ContainsKey(this.culture))
                    return CultureManager.instance.CultureMap[this.culture];
                else
                {
                    return CultureManager.instance.CultureMap.OrderBy(o=> RandomIntHelper.Next(1000000)).First().Value;
                }
            }
        }

        public TitleParser Dejure { get; set; }

        public int TotalTech
        {
            get
            {
                int tot = 0;
                if (this.Rank == 1)
                {
                    return this.Owns[0].cultureTechPoints;
                }

                foreach (var titleParser in this.SubTitles)
                {
                    if (titleParser.Value.Rank < this.Rank)
                    if (titleParser.Value.Rank > 1)
                        tot += titleParser.Value.TotalTech;
                        else if (titleParser.Value.Rank == 1)
                        {
                            tot += titleParser.Value.Owns[0].cultureTechPoints;
                        }
                }

                return tot;
            }
        }

        public int AverageTech
        {
            get
            {
                var p = this.GetAllProvinces();

                if (p.Count == 0)
                {
                    return 0;
                }

                int tot = 0;
                if (this.Rank == 1)
                {
                    if (this.Owns.Count == 0)
                    {
                    }

                    return this.Owns[0].cultureTechPoints;
                }

                foreach (var titleParser in this.SubTitles)
                {
                    if (titleParser.Value.Rank > 1)
                        tot += titleParser.Value.TotalTech;
                    else if(titleParser.Value.Rank==1)
                    {
                        if (titleParser.Value.Owns.Count == 0)
                        {
                            continue;
                        }

                        tot += titleParser.Value.Owns[0].cultureTechPoints;
                    }
                }


                return tot / p.Count;
            }
        }

        public string GenderLaw { get; set; }

        public string Succession { get; set; }

        public string LangName
        {
            get
            {
                if (this.Holder != null && this.Culture.dna.dynasty_title_names)
                {
                    if (this.Holder.Dynasty.Name == null)
                    {
                        this.Holder.Dynasty.Name = this.Culture.dna.GetDynastyName();
                    }

                    return this.Holder.Dynasty.Name;
                }
                else
                {
                    if (LanguageManager.instance.Get(this.Name) == null)
                    {
                        foreach (var scopeChild in this.Scope.Children)
                        {
                            if (scopeChild is ScriptCommand)
                            {
                                var name = (scopeChild as ScriptCommand).Name;

                                if (name == "capital" ||
                                    name == "color" ||
                                    name == "color2" ||
                                    name == "religion" ||
                                    name == "culture" ||
                                    name == "holy_site" ||
                                    !((scopeChild as ScriptCommand).Value is string))
                                {
                                }
                                else
                                {
                                    return (scopeChild as ScriptCommand).Value.ToString();
                                }
                            }
                        }
                    }

                    if (LanguageManager.instance.Get(this.Name) == null || LanguageManager.instance.Get(this.Name) == "")
                    {
                        if (this.Name.StartsWith("c_"))

                        {
                            if (this.culture != null)
                            {
                                if (LanguageManager.instance.Get(this.Name.Replace("c_", "b_")) == null ||
                                    LanguageManager.instance.Get(this.Name.Replace("c_", "b_")) == "")
                                {
                            //        RenameForCulture(Culture);
                                }
                                else
                                {
                                    return LanguageManager.instance.Get(this.Name.Replace("c_", "b_"));
                                }
                            }
                            else
                            {
                                return this.Name;
                            }
                        }
                        else
                        {
                            if (this.culture != null)
                            {
                                if (LanguageManager.instance.Get(this.Name.Replace("c_", "b_")) == null ||
                                    LanguageManager.instance.Get(this.Name.Replace("c_", "b_")) == "")
                                {
                                    return this.Name;
                                }
                                else
                                {
                                    return LanguageManager.instance.Get(this.Name.Replace("c_", "b_"));
                                }
                            }
                        }
                    }




                    return LanguageManager.instance.Get(this.Name);
                }
            }
        }

        public string LangRealmName
        {
            get
            {
                if (LanguageManager.instance.Get(this.Name) == null)
                {
                    foreach (var scopeChild in this.Scope.Children)
                    {
                        if (scopeChild is ScriptCommand)
                        {
                            var name = (scopeChild as ScriptCommand).Name;

                            if (name == "capital" ||
                                name == "color" ||
                                name == "color2" ||
                                name == "religion" ||
                                name == "culture" ||
                                name == "holy_site" ||
                                !((scopeChild as ScriptCommand).Value is string))
                            {
                            }
                            else
                            {
                                return (scopeChild as ScriptCommand).Value.ToString();
                            }
                        }
                    }
                }

                if (LanguageManager.instance.Get(this.Name) == "")
                {
                    foreach (var scopeChild in this.Scope.Children)
                    {
                        if (scopeChild is ScriptCommand)
                        {
                            var name = (scopeChild as ScriptCommand).Name;

                            if (name == "capital" ||
                                name == "color" ||
                                name == "color2" ||
                                name == "religion" ||
                                name == "culture" ||
                                name == "holy_site" ||
                                !((scopeChild as ScriptCommand).Value is string))
                            {
                            }
                            else
                            {
                                return (scopeChild as ScriptCommand).Value.ToString();
                            }
                        }
                    }
                }


                switch (this.Rank)
                {
                    case 1:
                            return this.Owns[0].EditorName;

                        break;
                    case 2:
                        return this.LangName;
                        break;
                    case 3:

                        return this.LangName;
                        break;
                    case 4:

                        return this.LangName;
                        break;
                }

                return this.LangName;
            }
        }

        public string LangTitleName
        {
            get
            {
                switch (this.Rank)
                {
                    case 1:
                     //   if (LanguageManager.instance.Get(Name) == null ||
                      //LanguageManager.instance.Get(Name) == "")
                     //       RenameForCulture(Culture);
                        if (LanguageManager.instance.Get(this.Name) == "")
                        {
                            return this.Owns[0].EditorName;
                        }

                        return LanguageManager.instance.Get(this.Culture.dna.countTitle) + " of " + LanguageManager.instance.Get(this.Name);
                        break;
                    case 2:
                    //    if (LanguageManager.instance.Get(Name) == null ||
                 //  LanguageManager.instance.Get(Name) == "")
                     //       RenameForCulture(Culture);
                        return LanguageManager.instance.Get(this.Culture.dna.dukeTitle) + " of " + LanguageManager.instance.Get(this.Name);
                        break;
                    case 3:
                    //    if (LanguageManager.instance.Get(Name) == null ||
                    // LanguageManager.instance.Get(Name) == "")
                     //       RenameForCulture(Culture);
                        return LanguageManager.instance.Get(this.Culture.dna.kingTitle) + " of " + LanguageManager.instance.Get(this.Name);
                        break;
                    case 4:
                     //   if (LanguageManager.instance.Get(Name) == null ||
                    //  LanguageManager.instance.Get(Name) == "")
                      //      RenameForCulture(Culture);
                        return LanguageManager.instance.Get(this.Culture.dna.empTitle) + " of " + LanguageManager.instance.Get(this.Name);
                        break;
                }

                return LanguageManager.instance.Get(this.Name);
            }
        }

        public ProvinceParser PalaceLocation { get; set; }

        public ScriptScope TechGroup { get; set; }

        public TitleParser StartLiege { get; set; }

        public Dictionary<string, TitleParser> DejureSub = new Dictionary<string, TitleParser>();

        public List<TitleParser> palaces = new List<TitleParser>();

        public List<int> republicdynasties = new List<int>();

        public List<TitleParser> Wars = new List<TitleParser>();

        public void Remove()
        {
            this.Scope.Parent.Remove(this.Scope);
        }

        public bool SameRealm(TitleParser title)
        {
            var liege = this;

            while (liege.Liege != null && liege.Rank < liege.Liege.Rank)
            {
                liege = liege.Liege;
            }

            var liege2 = title;

            while (liege2.Liege != null && liege2.Rank < liege2.Liege.Rank)
            {
                liege2 = liege2.Liege;
            }

            return liege == liege2;
        }

        public void AddChildProvinces(List<ProvinceParser> targets)
        {
            foreach (var subTitle in this.SubTitles)
            {
                if (subTitle.Value.Rank >= this.Rank)
                {
                    continue;
                }

                targets.AddRange(subTitle.Value.Owns);
                subTitle.Value.AddChildProvinces(targets);
            }
        }

        public bool Adjacent(TitleParser other)
        {
            return this.IsAdjacent(other);
        }

        private void AddAdj(TitleParser other)
        {
            if (this.AdjacentToTitleSet.Contains(other))
            {
                return;
            }

            this.AdjacentToTitleSet.Add(other);
            this.AdjacentToTitle.Add(other);
            other.AdjacentToTitleSet.Add(this);
            other.AdjacentToTitle.Add(this);
        }

        private void AddNotAdj(TitleParser other)
        {
            if (this.AdjacentToTitleSet.Contains(other))
            {
                return;
            }

            this.AdjacentToTitleSet.Add(other);
            other.AdjacentToTitleSet.Add(this);
        }


        public void RemoveVassal(TitleParser titleParser)
        {
            this.SubTitles.Remove(titleParser.Name);
            if (titleParser.Liege == this)
            {
                titleParser.Liege = null;
            }

            this.Scope.Remove(titleParser.Scope);
        }

        public void AddVassals(ICollection<TitleParser> vassals)
        {
            var a = vassals.ToArray();
            foreach (var titleParser in a)
            {
                if (titleParser != null && titleParser.Rank >= this.Rank)
                {
                    continue;
                }

                this.SubTitles[titleParser.Name] = titleParser;
                titleParser.Liege = this;
                this.Scope.Add(titleParser.Scope);
            }
        }

        public TitleParser GetRandomLowRankLandedTitle()
        {
            List<TitleParser> choices = new List<TitleParser>();

            this.GetRandomLowRankLandedTitle(choices);
            if (choices.Count == 0)
            {
                return null;
            }

            return choices[RandomIntHelper.Next(choices.Count)];
        }

        public void GetRandomLowRankLandedTitle(List<TitleParser> choices)
        {
            if(this.Owns.Count > 0 && this.Holder.PrimaryTitle.Rank==1)
            {
                choices.Add(this);
            }

            foreach (var titleParser in this.SubTitles)
            {
                titleParser.Value.GetRandomLowRankLandedTitle(choices);
            }

            if (choices.Count == 0)
            {
                return;
            }
        }

        public void SplitLands()
        {
            if (this.Rank == 2)
            {
                List<ProvinceParser> titles = this.GetAllProvinces();
                List<ProvinceParser> half = this.GetAdjacentGroup(titles, titles.Count / 2);
                List<TitleParser> tits = new List<TitleParser>();
                foreach (var provinceParser in half)
                {
                    tits.Add(TitleManager.instance.TitleMap[provinceParser.ProvinceTitle]);
                }

              //  TitleManager.instance.PromoteNewRuler(TitleManager.instance.CreateDuke(tits));
            }
        }

        private List<ProvinceParser> GetAdjacentGroup(List<ProvinceParser> provinces, int preferedSize)
        {
            List<ProvinceParser> split = new List<ProvinceParser>();

            var start = provinces[RandomIntHelper.Next(provinces.Count)];

            foreach (var provinceParser in provinces)
            {
                if(start.Adjacent.Contains(provinceParser))
                {
                    split.Add(provinceParser);
                }

                if (split.Count >= preferedSize)
                {
                    break;
                }
            }

            if (split.Count < preferedSize)
            {
                foreach (var provinceParser in provinces)
                {
                    var a = split.ToArray();
                    foreach (var chosen in a)
                    {
                        if (start.Adjacent.Contains(provinceParser))
                        {
                            split.Add(provinceParser);
                        }

                        if (split.Count >= preferedSize)
                        {
                            break;
                        }
                    }
                }
            }

            return split;
        }

        public List<ProvinceParser> GetAllDejureProvinces()
        {
            List<ProvinceParser> provinces = new List<ProvinceParser>();

            this.GetAllDejureProvinces(provinces);

            return provinces;
        }


        public List<ProvinceParser> GetAllProvinces()
        {
            if (this.Rank == 0)
            {
                return new List<ProvinceParser>();
            }

            if (this.Holder != null)
            {
                return this.Holder.GetAllProvinces();
            }

            List<ProvinceParser> provinces = new List<ProvinceParser>();

            this.GetAllProvinces(provinces);

            return provinces;
        }

        public List<ProvinceParser> GetAllProvincesTitleOnly()
        {
            if (this.Rank == 0)
            {
                return new List<ProvinceParser>();
            }

            List<ProvinceParser> provinces = new List<ProvinceParser>();

            this.GetAllProvinces(provinces);

            return provinces;
        }

        public List<ProvinceParser> GetAllDirectProvinces()
        {
            List<ProvinceParser> provinces = new List<ProvinceParser>();

            this.GetAllProvinces(provinces);

            provinces = provinces.Where(p => p.Title.Holder == this.Holder).ToList();

            return provinces;
        }

        public List<ProvinceParser> GetAllDejureProvinces(List<ProvinceParser> provinces)
        {
            if (this.Owns.Count > 0 && !provinces.Contains(this.Owns[0]))
            {
                provinces.Add(this.Owns[0]);
            }

            foreach (var subTitle in this.DejureSub)
            {
                if (subTitle.Value.Rank >= this.Rank)
                {
                    continue;
                }

                subTitle.Value.GetAllDejureProvinces(provinces);
            }


            return provinces;
        }

        public List<ProvinceParser> GetAllProvinces(List<ProvinceParser> provinces)
        {
            if (this.Owns.Count > 0 && !provinces.Contains(this.Owns[0]))
            {
                provinces.Add(this.Owns[0]);
            }

            foreach (var subTitle in this.SubTitles)
            {
                if (subTitle.Value.Rank >= this.Rank)
                {
                    continue;
                }

                if (subTitle.Value.Rank == 0)
                {
                    continue;
                }

                if (subTitle.Value.Liege == this)
                {
                    subTitle.Value.GetAllProvinces(provinces);
                }
            }


            provinces = provinces.Where(a => this.Holder != null && a.Title.Holder.TopLiegeCharacter == this.Holder.TopLiegeCharacter).ToList();
            return provinces;
        }

        public void Kill()
        {
            if (this.Liege != null)
            {
                this.Liege.SubTitles.Remove(this.Name);
            }

            this.Scope.Parent.Remove(this.Scope);
        }

        public bool AnyHolder()
        {
            return this.Holder != null || this.CurrentHolder != null;
        }

        public void SetRealmReligion(string religion)
        {
            if (this.Holder != null)
            {
                this.Holder.religion = religion;
                this.Holder.UpdateCultural();
            }

            foreach (var titleParser in this.SubTitles)
            {
                titleParser.Value.SetRealmReligion(religion);
            }
        }

        public bool HasLiegeInChain(CharacterParser titleParser)
        {
            var c = this;

            c = this.Liege;

            while (c != null)
            {
                if (c.Holder == titleParser)
                {
                    return true;
                }

                c = c.Liege;
            }

            return false;
        }

        public bool HasCharacterInChain(CharacterParser titleParser)
        {
            var c = this;

            while (c != null)
            {
                if (c.Holder == titleParser)
                {
                    return true;
                }

                if (c.Holder == null)
                {
                    c = c.Liege;
                }
                else
                {
                    c = c.Holder.PrimaryTitle.Liege;
                }
            }

            return false;
        }

        public bool HasLiegeInChain(TitleParser titleParser)
        {
            var c = this;

            c = this.Liege;

            while (c != null)
            {
                if (c == titleParser)
                {
                    return true;
                }

                c = c.Liege;
            }

            return false;
        }

        public bool HasTitleInChain(TitleParser titleParser)
        {
            var c = this;

            while (c != null)
            {
                if (c == titleParser)
                {
                    return true;
                }

                c = c.Liege;
            }

            return false;
        }

        /*   public void DoSetLiegeEvent(TitleParser titleParser)
           {
               if (Liege == titleParser)
                   return;
               if (government == "republic" && titleParser != null)
                   return;

               if (titleParser != null && titleParser.Rank <= Rank)
                   return;

               if (titleParser != null && Holder != null && Holder.PrimaryTitle.Rank >= titleParser.Rank)
               {
                   return;
               }

               SimulationManager.instance.DebugTest(this);

               Liege = titleParser;

               {

                   ScriptScope thing = new ScriptScope();
                   thing.Name = SimulationManager.instance.Year + ".2.1";
                   if (titleParser == null)
                       thing.Add(new ScriptCommand() { Name = "liege", Value = 0 });
                   else
                       thing.Add(new ScriptCommand() { Name = "liege", Value = titleParser.Name });

                   titleScripts.Add(thing);

               }

               SimulationManager.instance.DebugTest(this);
           }*/
        public void DoSetLiegeEventDejure(TitleParser titleParser)
        {
            if (this.Liege == titleParser)
            {
                return;
            }

            if (this.government == "republic" && titleParser != null)
            {
                return;
            }

            if (titleParser != null)
            {
                if (titleParser != null && titleParser.Rank <= this.Rank)
                {
                    return;
                }

                if (this.Holder != null && this.Holder.PrimaryTitle.Rank >= titleParser.Rank)
                {
                    return;
                }

                var parent = TitleManager.instance.LandedTitlesScript.Root;

                if (this.Scope.Parent != null)
                {
                    this.Scope.Parent.Remove(this.Scope);
                    if (this.Liege != null)
                    {
                        this.Liege.SubTitles.Remove(this.Name);
                    }
                }
                else
                {
                    parent.Remove(this.Scope);
                }

                titleParser.Scope.SetChild(this.Scope);
                titleParser.SubTitles[titleParser.Name] = this;

                this.Liege = titleParser;
            }
            else
            {
                var parent = TitleManager.instance.LandedTitlesScript.Root;

                if (this.Scope.Parent != null)
                {
                    this.Scope.Parent.Remove(this.Scope);
                    if (this.Liege != null)
                    {
                        this.Liege.SubTitles.Remove(this.Name);
                    }
                }
                else
                {
                    parent.Remove(this.Scope);
                }

                parent.SetChild(this.Scope);

                this.Liege = null;
            }

            {
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";
                if (titleParser == null)
                {
                    thing.Add(new ScriptCommand() { Name = "liege", Value = 0 });
                }
                else
                {
                    thing.Add(new ScriptCommand() { Name = "liege", Value = titleParser.Name });
                }

                this.titleScripts.Add(thing);
            }
        }

        public bool DoSetLiegeEvent(TitleParser titleParser, bool skipValidate = false)
        {
            if (titleParser != null && titleParser.Rank <= this.Rank)
            {
                return false;
            }

            if (this.Liege == titleParser)
            {
                return false;
            }

            if (this.government == "republic" && titleParser != null)
            {
                return false;
            }

            if (titleParser != null)
            {
                if (titleParser != null && titleParser.Rank <= this.Rank)
                {
                    return false;
                }

                if (this.Holder != null &&  this.Holder != titleParser.Holder && this.Holder.PrimaryTitle.Rank >= titleParser.Rank)
                {
                    return false;
                }

                /*   var parent = TitleManager.instance.LandedTitlesScript.Root;

                   if (Scope.Parent != null)
                   {
                       Scope.Parent.Remove(Scope);

                   }
                   else
                   {
                       parent.Remove(Scope);
                   }

                   titleParser.Scope.SetChild(Scope);
               */
                if (this.Liege != null)
                {
                    this.Liege.Log("Lost vassal: " + this.Name);
                }

                bSkipValidate = skipValidate;
                this.Liege = titleParser;
                bSkipValidate = false;
                if (this.Liege != null)
                {
                    this.Liege.Log("Gained vassal: " + this.Name);
                }


                SimulationManager.instance.DestroyedTitles.Remove(titleParser);
                SimulationManager.instance.DestroyedTitles.Remove(this);
            }
            else
            {
                    this.Log("Made independent");

                /*                var parent = TitleManager.instance.LandedTitlesScript.Root;

                                if (Scope.Parent != null)
                                {
                                    Scope.Parent.Remove(Scope);
                                }
                                else
                                {
                                    parent.Remove(Scope);
                                }

                                parent.SetChild(Scope);
                                */
                this.Liege = (null);
                SimulationManager.instance.DestroyedTitles.Remove(this);
            }

            {
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";
                if (titleParser == null)
                {
                    thing.Add(new ScriptCommand() { Name = "liege", Value = 0 });
                }
                else
                {
                    thing.Add(new ScriptCommand() { Name = "liege", Value = titleParser.Name });
                }

                this.titleScripts.Add(thing);
            }

            return true;
        }

        public void SoftDestroy()
        {
            var list = this.SubTitles.Values.ToList();
            TitleParser lastLiege = null;
            if (this.Liege != null)
            {
                lastLiege = this.Liege;
                this.DoSetLiegeEvent(null);
            }

            this.Log("Title destroyed");
            SimulationManager.instance.DestroyedTitles.Add(this);
            if (this.Holder != null)
            {
                this.Holder.Titles.Remove(this);
                this.Holder = null;
            }

            foreach (var titleParser in list)
            {
                if (lastLiege == null)
                {
                    titleParser.Log("Setting independent due to liege destruction");
                    titleParser.DoSetLiegeEvent(null);
                }
                else
                {
                    titleParser.Log("Setting liege to this liege in SoftDestroy");
                    titleParser.DoSetLiegeEvent(lastLiege);
                }
            }
        }

        public int republicdynasty;
        public string republicreligion;
        private ProvinceParser _capitalProvince;
        public string religion;
        private Rectangle _bounds;
        private bool dirty = true;
        private Point _textPos;

        public void DoTechPointTick()
        {
            float mil = 0;
            float cul = 0;
            float eco = 0;
            if (TechnologyManager.instance.CentresOfTech.Count == 0)
            {
                int max = 1;
                 max = 2;
                for (int x = 0; x < max; x++)
                {
                    var count = TitleManager.instance.GetRandomCount();
                    bool doIt = true;
                    foreach (var provinceParser in TechnologyManager.instance.CentresOfTech)
                    {
                        if (provinceParser.DistanceTo(count) < 600)
                        {
                            doIt = false;
                        }
                    }

                    if (!doIt)
                    {
                        x--;
                        continue;
                    }

                    TechnologyManager.instance.CentresOfTech.Add(count);
                }
            }

            if (this.Liege == null)
            {
                if (this.Rank == 2)
                {
                    mil += 2;
                    cul += 2;
                    eco += 2;
                }

                if (this.Rank == 3)
                {
                    mil += 3;
                    cul += 3;
                    eco += 3;
                }

                if (this.Rank == 4)
                {
                    mil += 4;
                    cul += 4;
                    eco += 4;
                }



                var p = this.GetAllProvinces();
                foreach (var provinceParser in p)
                {
                    float cul2 = cul;
                    float eco2 = eco;
                    float mil2 = mil;
                    var cap = this.CapitalProvince;


                    {
                        if (provinceParser.Title.Holder != null && provinceParser.Title.Holder.TopLiegeCharacter == provinceParser.Title.Holder)
                        {
                            cul2 += 1;
                            eco2 += 1;
                            mil2 += 4;
                        }
                    }

                    if (this.TopmostTitle.Holder == provinceParser.Title.Holder)
                    {
                        mil2 += 2;
                    }

                    if (provinceParser.Title.Holder.PrimaryTitle.Rank == 1)
                    {
                        mil2 -= 1;
                    }

                    if (provinceParser.Title.Holder.PrimaryTitle.Rank == 2)
                    {
                        mil2 += 4;
                    }

                    if (provinceParser.Title.Holder.PrimaryTitle.Rank == 3)
                    {
                        mil2 += 6;
                    }

                    if (provinceParser.Title.Holder.PrimaryTitle.Rank == 4)
                    {
                        mil2 += 8;
                    }

                    if (provinceParser.Title.TopmostTitle.Holder != null)
                    {
                        cap = provinceParser.Title.TopmostTitle.Holder.EffectiveCapitalProvince;
                    }

                    if (provinceParser == cap)
                    {
                        cul2 += 1;
                        eco2 += 1;
                        mil2 += 4;
                    }

                    if (provinceParser.Adjacent.Contains(cap))
                    {
                        cul2 += 1;
                        eco2 += 1;
                        mil2 += 1;
                    }

                    if (provinceParser.Title.Liege != null && provinceParser.Title.Liege.Holder != null)
                    {
                        cap = provinceParser.Title.Liege.Holder.EffectiveCapitalProvince;
                    }

                    if (provinceParser == cap)
                    {
                        mil2 += 2;
                    }

                    float closest = 1000000;

                    foreach (var parser in TechnologyManager.instance.CentresOfTech)
                    {
                        float dist = parser.DistanceTo(provinceParser);
                        if (dist > TitleManager.instance.maxDist)
                        {
                            TitleManager.instance.maxDist = dist;
                        }

                        if (dist < closest)
                        {
                            closest = dist;
                        }
                    }


                    float techAdd = closest/1500.0f;
                    if (techAdd > 1.0f)
                    {
                        techAdd = 1.0f;
                    }

                    if (techAdd < 0)
                    {
                        techAdd = 0;
                    }

                    techAdd = 1.0f - techAdd;

                   // if (TechnologyManager.instance.CentresOfTech.Contains(provinceParser))
                    {
                        cul2 += 20 * techAdd;
                        eco2 += 20 * techAdd;
                        mil2 += 7 * techAdd;
                    }

                    provinceParser.cultureTechPoints += (int)cul2;
                    provinceParser.economicTechPoints += (int)eco2;
                    provinceParser.militaryTechPoints += (int)mil2;
                    foreach (var parser in provinceParser.Adjacent)
                    {
                        //     if(Rand.Next(4)==0)
                        if (parser.militaryTechPoints < provinceParser.militaryTechPoints)
                        {
                            parser.militaryTechPoints++;
                        }

                        if (parser.cultureTechPoints < provinceParser.cultureTechPoints)
                        {
                            parser.cultureTechPoints++;
                        }

                        if (parser.economicTechPoints < provinceParser.economicTechPoints)
                        {
                            parser.economicTechPoints++;
                        }

                        if (parser.cultureTechPoints < provinceParser.cultureTechPoints)
                        {
                            parser.cultureTechPoints++;
                        }

                        if (parser.economicTechPoints < provinceParser.economicTechPoints)
                        {
                            parser.economicTechPoints++;
                        }

                        if (parser.cultureTechPoints < provinceParser.cultureTechPoints)
                        {
                            parser.cultureTechPoints++;
                        }
                        if (parser.economicTechPoints < provinceParser.economicTechPoints)
                        {
                            parser.economicTechPoints++;
                        }
                    }
                }
            }
        }

        public bool IsAdjacent(TitleParser titleParser)
        {
            var a = titleParser.GetAllProvinces();
            var b = this.GetAllProvinces();

            foreach (var provinceParser in a)
            {
                foreach (var parser in b)
                {
                    if (parser.Adjacent.Contains(provinceParser))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsAdjacentSea(TitleParser titleParser)
        {
            var a = titleParser.GetAllProvinces();
            var b = this.GetAllProvinces();

            foreach (var provinceParser in a)
            {
                foreach (var parsera in provinceParser.Adjacent)
                {
                    if (!parsera.land)
                    {
                        foreach (var parser in b)
                        {
                            if (parser.Adjacent.Contains(parsera))
                            {
                                return true;
                            }
                        }
                    }
                }

                foreach (var parser in b)
                {
                    if (parser.Adjacent.Contains(provinceParser))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<ProvinceParser> OverseaProvinces()
        {
            var a = this.GetAllProvinces();
            List<ProvinceParser> oversea = new List<ProvinceParser>();
            foreach (var provinceParser in a)
            {
                foreach (var parser in provinceParser.Adjacent)
                {
                    if (!parser.land)
                    {
                        List<ProvinceParser> landAdjacent = new List<ProvinceParser>();

                        landAdjacent = parser.GetAdjacentLand(6, landAdjacent);
                        oversea.AddRange(landAdjacent);
                    }
                }
            }

            oversea = oversea.Distinct().ToList();
            oversea.RemoveAll(b => a.Contains(b));

            return oversea;
        }

        public bool IsAdjacentSea2(TitleParser titleParser)
        {
            var a = titleParser.GetAllProvinces();
            var b = this.OverseaProvinces();

            foreach (var provinceParser in a)
            {
                foreach (var parser in b)
                {
                    if (provinceParser == (parser))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsAdjacent(TitleParser titleParser, int num)
        {
            var a = titleParser.GetAllProvinces();
            var b = this.GetAllProvinces();
            int i = 0;
            foreach (var provinceParser in a)
            {
                foreach (var parser in b)
                {
                    if (parser.Adjacent.Contains(provinceParser))
                    {
                        i++;
                    }

                    if (i >= num)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool HasGovernmentInChain(string type)
        {
            if (this.government == type)
            {
                return true;
            }

            if (this.Liege != null)
            {
                return this.Liege.HasGovernmentInChain(type);
            }

            return false;
        }

        public void SetRealmCulture(string s)
        {
            this.culture = s;
            foreach (var value in this.SubTitles.Values)
            {
                value.SetRealmCulture(this.culture);
            }
        }

        public TreeNode AddTreeNode(TreeView inspectTree, TreeNode parent=null)
        {
            if (this.Name.StartsWith("e_spain"))
            {
            }

            CultureParser c = null;
            if (this.Rank == 0)
                return null;
            else if (this.Rank == 1)
                c = this.Owns[0].Culture;
            else
            {
                c = this.FindBestCulture();
            }

            if (c == null)
            {
                return null;
            }

            string tit = "";
            switch (this.Rank)
            {
                case 0:
                    tit = LanguageManager.instance.Get(c.dna.baronTitle);
                    break;
                case 1:
                    tit = LanguageManager.instance.Get(c.dna.countTitle);
                    break;
                case 2:
                    tit = LanguageManager.instance.Get(c.dna.dukeTitle);
                    break;
                case 3:
                    tit = LanguageManager.instance.Get(c.dna.kingTitle);
                    break;
                case 4:
                    tit = LanguageManager.instance.Get(c.dna.empTitle);
                    break;
            }

            TreeNode res = null;
            if (parent != null)
            {
                if (this.Rank == 1)
                {
                    res = parent.Nodes.Add(this.Owns[0].EditorName + " " + "(" + tit + ")");
                }
                else
                    res = parent.Nodes.Add(LanguageManager.instance.Get(this.Name) + " " + "("+tit+")");
                res.Tag = this;
                res.ImageIndex = this.Rank;
            }

            else
            {
                if (this.Rank == 1)
                {
                    res = inspectTree.Nodes.Add(this.Owns[0].EditorName + " " + "(" + tit + ")");
                }
                else res = inspectTree.Nodes.Add(LanguageManager.instance.Get(this.Name) + " " + "(" + tit + ")");
                res.Tag = this;
                res.ImageIndex = this.Rank;
            }

            foreach (var titleParser in this.SubTitles)
            {
                titleParser.Value.AddTreeNode(inspectTree, res);
            }

            return res;
        }

        private CultureParser FindBestCulture()
        {
            var provs = this.GetAllProvinces();

            var groups = provs.GroupBy(a => a.Culture);

            groups = groups.OrderBy(g => g.Count()).Reverse();
            if (groups.Count() == 0)
            {
                return null;
            }

            return groups.First().Key;
        }

        public void Capture(List<ProvinceParser> provinces)
        {
            List<TitleParser> titles = new List<TitleParser>();
            foreach (var provinceParser in provinces)
            {
                if (provinceParser.Title == null)
                {
                    continue;
                }

                if (provinceParser.Title.TopmostTitle == this)
                {
                    continue;
                }

                var title = provinceParser.Title;//provinceParser.Title.FindTopmostTitleContainingAllProvinces(provinces, Holder.PrimaryTitle);
                if (title.Rank < this.Rank && (title.Holder==null || title.Holder.PrimaryTitle.Rank < this.Rank))
                {
               //     if (!title.DoSetLiegeEvent(this))
                    {
                   //     this.Holder.GiveTitleSoft(title, true);
                        title.DoSetLiegeEvent(this, true);
                        titles.Add(title);
                    }
                }
                else
                {
                    if (title.Rank < this.Rank)
                    {
                        this.Holder.GiveTitleSoft(title, true, true, true);
                        title.DoSetLiegeEvent(this);
                        titles.Add(title);
                    }
                    else
                    {
                        this.Holder.GiveTitleSoft(title, true, true, true);
                        titles.Add(title);
                        title.DoSetLiegeEvent(null);
                     }
                }
            }

            titles = titles.Distinct().ToList();

            for (var index = 0; index < titles.Count; index++)
            {
                var titleParser = titles[index];
                if (titleParser.Dejure != null && (titleParser.Dejure.Holder == null || titleParser.Dejure.SubTitles.Count==0))
                {
                    int c = 0;
                    List<TitleParser> has = new List<TitleParser>();
                    foreach (var parser in titleParser.Dejure.DejureSub)
                    {
                        bool b = parser.Value.HasCharacterInChain(this.Holder);
                        c += b ? 1 : 0;
                        if (b)
                        {
                            has.Add(parser.Value);
                        }
                    }

                    float percent = c / (float) titleParser.Dejure.DejureSub.Count;

                    if (percent > 0.5f)
                    {
                        this.Holder.GiveTitleSoft(titleParser.Dejure, true, true);
                        titles.Add(titleParser.Dejure);
                        if(titleParser.Dejure.Rank < this.Rank)
                        {
                            titleParser.Dejure.DoSetLiegeEvent(this);
                        }
                        else
                        {
                            titleParser.Dejure.DoSetLiegeEvent(null);
                        }

                        foreach (var parser in has)
                        {
                            parser.DoSetLiegeEvent(titleParser.Dejure);
                        }
                    }
                }
            }

            this.TopmostTitle.ValidateRealm(new List<CharacterParser>(), this.TopmostTitle);
        }

        internal TitleParser FindTopmostTitleContainingAllProvinces(List<ProvinceParser> provinces, TitleParser except=null, bool dejure = true)
        {
            if (this.Owns.Count > 0)
            {
                if (provinces.Contains(this.Owns[0]))
                {
                    if (this.Liege != null && this.Liege != except)
                    {
                        var title = this.Liege.FindTopmostTitleContainingAllProvinces(provinces, except, dejure);

                        if (title == null)
                        {
                            return this;
                        }

                        return title;
                    }

                    return this;
                }
            }
            else
            {
                List<ProvinceParser> provinceParsers = null;

                if(dejure)
                {
                    provinceParsers  = this.GetAllDejureProvinces();
                }
                else
                {
                    provinceParsers = this.GetAllProvinces();
                }

                float percent = 0;
                float count = 0;
                foreach (var provinceParser in provinceParsers)
                {
                    if (provinces.Contains(provinceParser))
                    {
                        count++;
                    }
                }

                percent = count / provinceParsers.Count;
                if (provinceParsers.Count == 0)
                {
                    percent = 0;
                }

                if (percent <1f || except == this)
                {
                    return null;
                }

                if (this.Liege == null)
                {
                    return this;
                }

                TitleParser parent = this.Liege.FindTopmostTitleContainingAllProvinces(provinces, except, dejure);

                if (parent == null)
                {
                    return this;
                }

                return parent;
            }

            return null;
        }

        public string FindValidCulture()
        {
            if (this.culture != null)
            {
                return this.culture;
            }

            var prov = this.GetAllProvinces();

            var list = prov.Where(a => a.Culture != null).ToList();
            if (list.Count == 0)
            {
                return CultureManager.instance.AllCultures[RandomIntHelper.Next(CultureManager.instance.AllCultures.Count)].Name;
            }

            return list[RandomIntHelper.Next(list.Count)].Culture.Name;
        }

        public string FindValidReligion()
        {
            if (this.religion != null)
            {
                return this.religion;
            }

            var prov = this.GetAllProvinces();

            var list = prov.Where(a => a.Religion != null).ToList();
            if (list.Count == 0)
            {
                return ReligionManager.instance.AllReligions[RandomIntHelper.Next(ReligionManager.instance.AllReligions.Count)].Name;
            }

            return list[RandomIntHelper.Next(list.Count)].Religion.Name;
        }

        public CharacterParser GetRandomSubtitleHolder()
        {
            var choices = new List<CharacterParser>();
            foreach (var titleParser in this.SubTitles)
            {
                if(titleParser.Value.Holder != null && titleParser.Value.Holder.PrimaryTitle.Rank < this.Rank)
                {
                    choices.Add(titleParser.Value.Holder);
                }
            }

            if (choices.Count == 0)
            {
                return null;
            }

            return choices[RandomIntHelper.Next(choices.Count)];
        }

        public void MakeIndependent()
        {
            if (this.Liege != null)
            {
                if (this.Holder != null)
                {
                    if (this.Holder.PrimaryTitle != this)
                    {
                        this.Holder.NonRelatedHeir.GiveTitleSoft(this);
                    }
                }
                else
                {
                    CharacterManager.instance.CreateNewCharacter(null, false,
                        SimulationManager.instance.Year - 16, this.religion, this.culture).GiveTitleSoft(this);
                }

                this.DoSetLiegeEvent(null);
            }
        }

        public bool GiveToNeighbour()
        {
            var prov = this.GetAllProvinces();
            var list = new List<ProvinceParser>();
            prov.ForEach(a => list.AddRange(a.Adjacent.Where(b => b.land && b.ProvinceTitle != null && b.Title.TopmostTitle != this.TopmostTitle)));
            var titleChoices = new List<TitleParser>();
            foreach (var provinceParser in list)
            {
                titleChoices.Add(provinceParser.Title.Holder.TopLiegeCharacter.PrimaryTitle);
            }

            titleChoices = titleChoices.Distinct().ToList();

            if (titleChoices.Count > 0)
            {
                titleChoices = titleChoices.OrderBy(a => a.Rank).Reverse().ToList();
                var toGiveTo = titleChoices[0];
                if (this.Liege != null)
                {
                    if (toGiveTo.Rank <= this.Rank)
                    {
                        this.Log("Made independent due to being a seperate island prior to giving to neighbour");
                        this.DoSetLiegeEvent(null);
                    }
                    else
                    {
                        this.Log("Made vassal of neighbour (" + toGiveTo.Name+ ") due to being a seperate island");
                        this.DoSetLiegeEvent(toGiveTo);
                    }
                }

                this.Log("Given to neighbour holder of " + toGiveTo.Name);
                toGiveTo.Holder.GiveTitleSoft(this);

                return true;
            }

            return false;
        }

        public List<TitleParser> GetAllDuchies()
        {
            List<TitleParser> duchies = new List<TitleParser>();

            return this.GetAllDuchies(duchies);
        }

        private List<TitleParser> GetAllDuchies(List<TitleParser> duchies)
        {
            if(this.Rank == 2)
            {
                duchies.Add(this);
            }

            if (this.Rank > 2)
            {
                foreach (var subTitlesValue in this.DejureSub.Values)
                {
                    subTitlesValue.GetAllDuchies(duchies);
                }
            }

            return duchies;
        }

        public List<TitleParser> GetAllCounts()
        {
            List<TitleParser> counts = new List<TitleParser>();

            return this.GetAllCounts();
        }

        private List<TitleParser> GetAllCounts(List<TitleParser> counts)
        {
            if (this.Rank == 1)
            {
                counts.Add(this);
            }

            if (this.Rank > 1)
            {
                foreach (var subTitlesValue in this.DejureSub.Values)
                {
                    subTitlesValue.GetAllCounts(counts);
                }
            }

            return counts;
        }
    }
}
