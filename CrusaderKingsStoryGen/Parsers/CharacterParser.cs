// <copyright file="CharacterParser.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class CharacterParser : Parser
    {
        public string religion = "pagan";
        public string culture = "norse";
        public List<TitleParser> Titles = new List<TitleParser>();
        private Color _color;
        private TitleParser _primaryTitle;
        public int lastImportantYear = 0;

        public void DoDynasty()
        {
            var d = DynastyManager.instance.GetDynasty(CultureManager.instance.CultureMap[this.culture]);
            this.Scope.Delete("dynasty");
            this.Scope.Delete("father");
            this.Scope.Delete("mother");
            this.Scope.Add("dynasty", d.ID);
            if (this.Father != null)
            {
                this.Scope.Add("father", this.Father.ID);
            }

            if (this.Mother != null)
            {
                this.Scope.Add("mother", this.Mother.ID);
            }

            if (!d.Members.Contains(this))
            {
                d.Members.Add(this);
            }

            this.Dynasty = d;
        }

        public void SetupExistingDynasty()
        {
            var d = this.Dynasty;
            this.Scope.Delete("dynasty");
            this.Scope.Delete("father");
            this.Scope.Delete("mother");
            this.Scope.Add("dynasty", d.ID);
            if (this.Father != null)
            {
                this.Scope.Add("father", this.Father.ID);
            }

            if (this.Mother != null)
            {
                this.Scope.Add("mother", this.Mother.ID);
            }

            if (!d.Members.Contains(this))
            {
                d.Members.Add(this);
            }

            this.Dynasty = d;
        }

        public Dynasty Dynasty { get; set; }

        private static int ccc = 0;

        public void UpdateCultural(string name = null)
        {
            ccc++;
            if (ccc > 10)
            {
                var sub = this.Titles[0].SubTitles;
            }

            var top = this.TopLiegeCharacter;
            this.religion = top.religion;
            if (top.culture == null || !CultureManager.instance.CultureMap.ContainsKey(top.culture))
            {
                top.culture = CultureManager.instance.CultureMap.Values.OrderBy(r=>RandomIntHelper.Next(10000)).First().Name;
            }

            this.culture = top.culture;
            if (this.religion != "pagan")
            {
            }

            this.SetProperty("religion", this.religion);

            if (this.isFemale)
            {
                this.Scope.Delete("female");
                this.Scope.Add(new ScriptCommand("female", true, this.Scope));
            }

            this.SetProperty("culture", this.culture);
            if (this._chrName != null && name==null)
            {
                name = this._chrName;
            }

            if (name==null)
            {
                name = CultureManager.instance.CultureMap[this.culture].PickCharacterName(this.isFemale);
            }

            this.ChrName = name;
            this.SetProperty("name", name);
            foreach (var titleParser in this.Titles)
            {
                if (titleParser.Owns.Count > 0)
                {
                    titleParser.Owns[0].SetProperty("religion", this.religion);
                    titleParser.Owns[0].SetProperty("culture", this.culture);
                }
                else
                {
              /*      foreach (var subTitle in titleParser.SubTitles)
                    {
                        if (subTitle.Value.Holder != null && subTitle.Value.Holder != this)
                            subTitle.Value.Holder.UpdateCultural();
                    }*/
                }
            }

            if (!this.Scope.HasNamed("dynasty"))
            {
                this.DoDynasty();
            }
            else
            {
                this.SetupExistingDynasty();
            }

            ccc--;
            CharacterManager.instance.SetAllDates(this.YearOfBirth, this.YearOfDeath, this.Scope, this.Titles.Count > 0);
        }

        public string ChrName
        {
            get
            {
                if(this._chrName == null)
                {
                    this.UpdateCultural();
                }

                return this._chrName;
            }

            set { this._chrName = value; }
        }

        public bool isFemale { get; set; }

        public CharacterParser Liege
        {
            get
            {
                if (this.PrimaryTitle == null)
                {
                    return null;
                }

                if (this.PrimaryTitle.Liege == null)
                {
                    return null;
                }

                if (this.PrimaryTitle.Liege.Holder == this)
                {
                    return null;
                }

                return this.PrimaryTitle.Liege.Holder;
            }
        }

        public int PreferedKingdomSize { get; set; }

       // public Random Rand = new Random(Rand.Seed);
        public float Aggressiveness = 0;
        public int ID = 1000000;
        public static int IDMax = 1000000;
        public int ConquererTimer = 0;
        public int MaxDeminse = 4;

        public Color Color
        {
            get
            {
                var chr = this.TopLiegeCharacter;
                if (chr == this)
                {
                    if(this.Dynasty==null)
                    {
                        return new Color();
                    }

                    return this.Dynasty.Color;
                }

                if (chr.Dynasty == null)
                {
                    return new Color();
                }

                return chr.Dynasty.Color;
            }
        }

        public override string ToString()
        {
            return this.ID.ToString();
        }

        public CharacterParser(ScriptScope scope)
            : base(scope)
        {
            this.ID = IDMax++;
            if (this.ID == 1094261)
            {
            }

            this.YearOfBirth = SimulationManager.instance.Year - 16;
            this.YearOfDeath = SimulationManager.instance.Year + 150 + 1 + RandomIntHelper.Next(130);

            this.Aggressiveness = RandomIntHelper.Next(100) / 100.0f;
            this.MaxDeminse = RandomIntHelper.Next(4) + 3;
            this.PreferedKingdomSize = RandomIntHelper.Next(6) + 3;
        }

        public bool bKill = false;

        public CharacterParser()
            : base(CharacterManager.instance.GetNewCreatedCharacter())
        {
            this.ID = Convert.ToInt32(this.Scope.Name);
            if (this.ID == 1094261)
            {
            }

            IDMax = this.ID + 1;
            if (this.ID == 1005121)
            {
            }

            CharacterManager.instance.Unpicked.Remove(this.Scope);
           // Color = Color.FromArgb(255, Rand.Next(200) + 55, Rand.Next(200) + 55, Rand.Next(200) + 55);
            this.Aggressiveness = RandomIntHelper.Next(100) / 100.0f;
            this.MaxDeminse = RandomIntHelper.Next(4) + 3;
            this.PreferedKingdomSize = RandomIntHelper.Next(6) + 3;
            int b = SimulationManager.instance.Year + 1 - (16 + RandomIntHelper.Next(50));
            this.YearOfBirth = SimulationManager.instance.Year - 16;
            this.YearOfDeath = SimulationManager.instance.Year + 150 + 2 + RandomIntHelper.Next(130);
         //   CharacterManager.instance.SetAllDates(YearOfBirth, YearOfDeath, Scope);
        }

        public CharacterParser(bool adult = false)
            : base(CharacterManager.instance.GetNewCreatedCharacter())
        {
            this.ID = Convert.ToInt32(this.Scope.Name);
            if (this.ID == 1094261)
            {
            }

            IDMax = this.ID + 1;
            CharacterManager.instance.Unpicked.Remove(this.Scope);
        //    Color = Color.FromArgb(255, Rand.Next(200) + 55, Rand.Next(200) + 55, Rand.Next(200) + 55);
            this.Aggressiveness = RandomIntHelper.Next(100) / 100.0f;
            this.MaxDeminse = RandomIntHelper.Next(4) + 3;
            this.PreferedKingdomSize = RandomIntHelper.Next(6) + 3;
            int b = SimulationManager.instance.Year + 1 - (16 + RandomIntHelper.Next(50));
            this.YearOfBirth = SimulationManager.instance.Year - 16;
            this.YearOfDeath = SimulationManager.instance.Year + 150 + 2 + RandomIntHelper.Next(130);
            if (adult && this.Age < 16)
            {
                this.YearOfBirth -= 16 - this.Age;
            }

        //    CharacterManager.instance.SetAllDates(YearOfBirth, YearOfDeath, Scope);
        }

        public void TakeProvinceOverseas(ProvinceParser chosen)
        {
          for (int i = 0; i < 1; i++)
            {
                chosen.RenameForCulture(this.Culture);
                chosen.CreateTitle();
                chosen.CreateProvinceDetails(this.Culture);
                ProvinceParser t = chosen;
                TitleParser tit = TitleManager.instance.TitleMap[t.ProvinceTitle];
                //    while (tit.Liege != null)
                //       tit = tit.Liege;
                {
                }

                this.GiveTitle(tit);
                this.culture = CultureManager.instance.BranchCulture(this.culture).Name;
                ReligionParser r = ReligionManager.instance.BranchReligion(this.religion, this.culture);
                this.religion = r.Name;
                this.Culture.Group.ReligionGroup = r.Group;
            }
        }

        public void Tick()
        {
            int lands = 0;
            foreach (var titleParser in this.Titles)
            {
                lands += titleParser.LandedTitlesCount;
            }

            if (lands == 0)
            {
                var title = this.PrimaryTitle.GetRandomLowRankLandedTitle();
                if (title != null)
                    this.GiveTitle(title);
                else
                {
                    this.bKill = true;
                }
            }

            //    ConvertCountTitlesToDuchies();
            /*     if (PrimaryTitle != null && PrimaryTitle.Rank >= 3)
                 {
                     if (PrimaryTitle.Rank == 2)
                     {
                         if (PrimaryTitle.SubTitles.Count < 3)
                         {

                         }
                         else
                         {
                             return;
                         }
                     }
                     else
                     {
                         return;
                     }
                 }*/
            if (this.PrimaryTitle.DirectVassalLandedTitlesCount > 5)
            {
                this.PrimaryTitle.SplitLands();
            }

            this.ConquererTimer--;
            if (RandomIntHelper.Next(10000) == 0)
            {
                this.ConquererTimer = 1000 + RandomIntHelper.Next(2000);
            }

            if (this.Titles.Count == 0)
            {
                return;
            }

            if (RandomIntHelper.Next((int)(2 * (1.0f - this.Aggressiveness))) != 0)
            {
                return;
            }

            List<ProvinceParser> targets = new List<ProvinceParser>();
            List<ProvinceParser> hightargets = new List<ProvinceParser>();
            List<ProvinceParser> test = new List<ProvinceParser>();
            foreach (var titleParser in this.Titles)
            {
                foreach (var provinceParser in titleParser.Owns)
                {
                    if (provinceParser.land)
                    {
                        test.Add(provinceParser);
                    }
                }

                titleParser.AddChildProvinces(test);
            }


            foreach (var provinceParser in test)
            {
                foreach (var parser in provinceParser.Adjacent)
                {
                    if (!parser.land)
                    {
                        continue;
                    }

                    if (parser.ProvinceTitle == null)
                    {
                        parser.RenameForCulture(this.Culture);
                        parser.CreateTitle();
                  //      if (Culture.dna.horde)
                    //        parser.Title.Scope.Do("historical_nomad = yes");

                        parser.CreateProvinceDetails(this.Culture);
                    }

                    if (parser.ProvinceOwner != null && parser.ProvinceOwner.Rank > this.TopLiegeCharacter.PrimaryTitle.Rank)
                    {
                        continue;
                    }

                    if (this.PrimaryTitle.Rank >= 2 && parser.Title.Rank >= 2 &&
                        parser.Title.LandedTitlesCount < 2 && this.PrimaryTitle.LandedTitlesCount < 5)
                    {
                        this.PrimaryTitle.AddVassals(parser.Title.SubTitles.Values);
                        continue;
                    }

                    if (parser.Title.Holder != null)
                    {
                        continue;
                        if (parser.Title.Holder == this || parser.Title.Holder.TopLiegeCharacter == this.TopLiegeCharacter)
                        {
                            {
                                continue;
                            }
                        }
                    }

                    if (!targets.Contains(parser))
                    {
                        TitleParser t = TitleManager.instance.TitleMap[parser.ProvinceTitle];
                        if (!t.Claimed)
                        {
                            hightargets.Add(parser);
                        }

                        targets.Add(parser);
                    }
                }
            }

            if (targets.Count == 0)
            {
                return;
            }

            if (hightargets.Count > 0)
            {
                targets = hightargets;
            }

            int max = 1;
            if (this.ConquererTimer > 0)
            {
                max = Math.Min(targets.Count/3, max);
            }

            targets.Sort(this.DistanceToCapital);
            for (int i = 0; i < 1; i++)
            {
                ProvinceParser t = targets[RandomIntHelper.Next(Math.Min(max*3, targets.Count))];
                TitleParser tit = TitleManager.instance.TitleMap[t.ProvinceTitle];
                //    while (tit.Liege != null)
                //       tit = tit.Liege;
                {
                }

                this.GiveTitle(tit);
                targets.Remove(t);
            }
        }

        private int DistanceToCapital(ProvinceParser x, ProvinceParser y)
        {
            var p = this.PrimaryTitle.CapitalProvince.Points[0];
            float a = p.X - x.Points[0].X;
            float b = p.Y - x.Points[0].Y;
            float c = p.X - y.Points[0].X;
            float d = p.Y - y.Points[0].Y;

            if (Math.Abs(a) + Math.Abs(b) < Math.Abs(c) + Math.Abs(d))
            {
                return -1;
            }
            else if (Math.Abs(a) + Math.Abs(b) > Math.Abs(c) + Math.Abs(d))
            {
                return 1;
            }

            return 0;
        }

        public CharacterParser GetTopLiegeCharacter(int maxRank)
        {
            {
                if (this.Liege == null)
                {
                    return this;
                }

                var liege = this.Liege;

                while (liege.Liege != null && liege.Liege.PrimaryTitle.Rank > liege.PrimaryTitle.Rank && liege.Liege.PrimaryTitle.Rank <= maxRank)
                {
                    liege = liege.Liege;
                }


                return liege;
            }
        }

        public CharacterParser TopLiegeCharacter
        {
            get
            {
                if (this.Liege == null)
                {
                    return this;
                }

                var liege = this.Liege;

                while (liege.Liege != null && liege.Liege.PrimaryTitle.Rank > liege.PrimaryTitle.Rank)
                {
                    liege = liege.Liege;
                }

                return liege;
            }
        }

        public TitleParser PrimaryTitle
        {
            get
            {
                if (this._primaryTitle != null && this._primaryTitle.Holder != this)
                {
                    this._primaryTitle = null;
                }

                if (this._primaryTitle != null)
                {
                    return this._primaryTitle;
                }

                if (this.Titles.Count == 0)
                {
                    return null;
                }

                if (this._primaryTitle == null)
                {
                    foreach (var parser in this.Titles)
                    {
                        if (this._primaryTitle == null || this._primaryTitle.Rank < parser.Rank)
                        {
                            this._primaryTitle = parser;
                        }
                    }
                }

                return this._primaryTitle;
            }

            set { this._primaryTitle = value; }
        }

        //attack / Capture duchies
        public void GiveTitle(TitleParser t, bool doLaws=true)
        {
            if (t.Holder == this)
            {
                return;
            }

            if (t.government == "republicpalace" && t.republicdynasty != this.Dynasty.ID)
            {
                return;
            }

            if (t.Owns.Count > 0 && MapManager.instance.ToFill.Contains(t.Owns[0]))
            {
                if (t.Owns[0] == null)
                {
                }
                else
                {
                    MapManager.instance.ToFill.Remove(t.Owns[0]);
                    MapManager.instance.Filled.Add(t.Owns[0]);
                }
            }

            this.HadTitle = true;

            t.CurrentHolder = null;
            if (this.Titles.Contains(t))
            {
                return;
            }

            if (t != null)
            {
                if (t.Holder != null && t.Holder.PrimaryTitle != null)
                {
                    if (t.Holder.ID == 70560)
                    {
                    }

                    t.Holder.PrimaryTitle.Log("Holder " + this.ID + " - Primary title: " + t.Holder.PrimaryTitle.Name + " has lost the title: " + t.Name);
                }

                if (this.PrimaryTitle != null)
                {
                    t.Log("Title given to " + this.ID + " - Primary title: " + this.PrimaryTitle.Name);
                    this.PrimaryTitle.Log("Holder (" + this.ID + ") has gained " + t.Name);
                }
                else
                {
                    t.Log("Title given to " + this.ID);
                }
            }

            if (t.Holder != null)
            {
                if (t.Holder.PrimaryTitle == t)
                {
                    t.Holder.PrimaryTitle = null;
                }

                t.Holder.Titles.Remove(t);
            }

            t.Active = true;
            t.Holder = this;
            this._primaryTitle = null;
            if ((this.PrimaryTitle == null || t.Rank > this.PrimaryTitle.Rank))
            {
                this.Titles.Insert(0, t);
                this.PrimaryTitle = t;
            }
            else
            {
                this.Titles.Add(t);
            }

            foreach (var titleParser in t.SubTitles)
            {
            //    titleParser.Value.Liege = t;
            }

            if (t.Owns.Count > 0)
            {
                t.Owns[0].Culture = this.Culture;
                t.Owns[0].Religion = this.Religion;
                if (this.GetProperty("culture") != null)
                {
                    t.Owns[0].SetProperty("culture", this.GetProperty("culture").Value);
                }

                if (this.GetProperty("religion") != null)
                {
                    t.Owns[0].SetProperty("religion", this.GetProperty("religion").Value);
                }

                t.Owns[0].ProvinceOwner = t;
            }


            {
                /* t.heldDates.Add(new TitleParser.HeldDate()
                 {
                     chrid = ID,
                     date = SimulationManager.instance.Year
                 });*/
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";
                thing.Add(new ScriptCommand() { Name = "holder", Value = this.ID });
                if (doLaws)
                {
                    thing.Add(new ScriptCommand() { Name = "law", Value = this.Culture.Group.PreferedSuccession });
                    thing.Add(new ScriptCommand() { Name = "law", Value = this.Culture.Group.PreferedGenderLaw });
                }

                t.GenderLaw = this.Culture.Group.PreferedGenderLaw;
                t.Succession = this.Culture.Group.PreferedSuccession;

                this.lastImportantYear = SimulationManager.instance.Year;

                t.titleScripts.Add(thing);
            }


            //   AddDateEvent(SimulationManager.instance.Year, 1, 1, new ScriptCommand("capital", PrimaryTitle.CapitalProvince.Title.Name, null));

            this.SetColorOfChildren(t);
        }

        public void GiveTitleSoftPlusIntermediate(TitleParser t)
        {
            if (t.government == "republicpalace" && t.republicdynasty != this.Dynasty.ID)
            {
                return;
            }

            if (t.Holder == this)
            {
                return;
            }

            if (this.PrimaryTitle != null && t.government == "republic")
            {
                return;
            }

            if (t.Rank == 0)
            {
                return;
            }

            if (t.Rank == 0 && this.PrimaryTitle != null && this.PrimaryTitle.Rank > 0)
            {
                return;
            }

            if (t.Liege != null && this.PrimaryTitle != null && t.TopmostTitle != this.PrimaryTitle.TopmostTitle)
            {
                return;
            }

            this.HadTitle = true;
            //     if (PrimaryTitle != null && t.Rank > PrimaryTitle.Rank)
            //   {
            ////      return;
            //   }

            if (t != null)
            {
                if (t.Holder != null && t.Holder.PrimaryTitle != null)
                {
                    t.Holder.PrimaryTitle.Log("Holder " + this.ID + " - Primary title: " + t.Holder.PrimaryTitle.Name + " has lost the title: " + t.Name);
                }

                if (this.PrimaryTitle != null)
                {
                    t.Log("Title given to " + this.ID + " - Primary title: " + this.PrimaryTitle.Name);
                    this.PrimaryTitle.Log("Holder (" + this.ID + ") has gained " + t.Name);
                }
                else
                {
                    t.Log("Title given to " + this.ID);
                }
            }

            t.CurrentHolder = null;
            if (this.Titles.Contains(t))
            {
                return;
            }

            if (t.Holder != null)
            {
                if (t.Holder.PrimaryTitle == t)
                {
                    t.Holder.PrimaryTitle = null;
                }

                t.Holder.Titles.Remove(t);
            }

            t.Active = true;
            t.Holder = this;
            this._primaryTitle = null;
            if ((this.PrimaryTitle == null || t.Rank > this.PrimaryTitle.Rank))
            {
                this.Titles.Insert(0, t);
                this.PrimaryTitle = t;
            }
            else
            {
                this.Titles.Add(t);
            }

            foreach (var titleParser in t.SubTitles)
            {
                //     titleParser.Value.Liege = t;
            }

            //   if(PrimaryTitle != null)
            //       t.Liege = PrimaryTitle.Liege;

            if (t.Owns.Count > 0)
            {
                if (this.GetProperty("culture") != null)
                {
                    t.Owns[0].SetProperty("culture", this.GetProperty("culture").Value);
                }

                if (this.GetProperty("religion") != null)
                {
                    t.Owns[0].SetProperty("religion", this.GetProperty("religion").Value);
                }

                t.Owns[0].ProvinceOwner = t;
            }

            this.SetColorOfChildren(t);

            {
                /* t.heldDates.Add(new TitleParser.HeldDate()
                 {
                     chrid = ID,
                     date = SimulationManager.instance.Year
                 });*/
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";
                thing.Add(new ScriptCommand() { Name = "holder", Value = this.ID });
                this.lastImportantYear = SimulationManager.instance.Year;

                t.titleScripts.Add(thing);
            }

            if (t.Liege != null && t.Liege.Holder != this)
            {
                this.GiveTitleSoftPlusIntermediate(t.Liege);
            }

            foreach (var titleParser in this.Titles)
            {
                if (titleParser.Liege != null && SimulationManager.instance.AutoValidateRealm)
                {
                    titleParser.Liege.ValidateRealm(new List<CharacterParser>(), titleParser.TopmostTitle);
                }
            }
        }

        public bool GiveTitleSoft(TitleParser t, bool capture=false, bool doScript=true, bool delayValidate=false)
        {
            if (t == null)
            {
                return false;
            }

            if (t.Holder == this)
            {
                return true;
            }

            if (this.ID == 1036062 || (t.Holder != null && t.Holder.ID== 1036062))
            {
            }

            if (t.Holder != null && t.Holder.TopLiegeCharacter != this.TopLiegeCharacter)
            {
            }

            if (t.government == "republicpalace" && t.republicdynasty != this.Dynasty.ID)
            {
                return false;
            }

            if (t.Rank == 0 && t.government != "republicpalace")
            {
                return false;
            }

            if (this.PrimaryTitle != null && t.government == "republic")
            {
                return false;
            }

            if (this.PrimaryTitle != null && t.Rank >= 2 && this.Titles.Any(tt => tt.government == "republic"))
            {
                return false;
            }

            if (!capture && t.Liege != null && this.PrimaryTitle != null && t.TopmostTitle != this.PrimaryTitle.TopmostTitle && t.republicdynasty == 0)
            {
                return false;
            }

            if (t.government == "republicpalace")
            {
                this.GiveTitleSoft(t.Liege);
            }

            this.HadTitle = true;
            //     if (PrimaryTitle != null && t.Rank > PrimaryTitle.Rank)
            //   {
            ////      return;
            //   }
            SimulationManager.instance.DestroyedTitles.Remove(t);

            t.CurrentHolder = null;
            if (this.Titles.Contains(t))
            {
                return false;
            }

            if (t != null)
            {
                if (t.Holder != null && t.Holder.PrimaryTitle != null)
                {
                    t.Holder.PrimaryTitle.Log("Holder " + this.ID + " - Primary title: " + t.Holder.PrimaryTitle.Name + " has lost the title: " + t.Name);
                }

                if (this.PrimaryTitle != null)
                {
                    t.Log("Title given to " + this.ID + " - Primary title: " + this.PrimaryTitle.Name);
                    this.PrimaryTitle.Log("Holder (" + this.ID + ") has gained " + t.Name);
                }
                else
                {
                    t.Log("Title given to " + this.ID);
                }
            }

            if (t.Holder != null)
            {
                if (t.Holder.PrimaryTitle == t)
                {
                    t.Holder.PrimaryTitle = null;
                }

                t.Holder.Titles.Remove(t);
            }

            t.Active = true;
            t.Holder = this;
            this._primaryTitle = null;
            if ((this.PrimaryTitle == null || t.Rank > this.PrimaryTitle.Rank))
            {
                this.Titles.Insert(0, t);
                this.PrimaryTitle = t;
            }
            else
            {
                this.Titles.Add(t);
            }

            foreach (var titleParser in t.SubTitles)
            {
           //     titleParser.Value.Liege = t;
            }

         //   if(PrimaryTitle != null)
         //       t.Liege = PrimaryTitle.Liege;

            if (doScript && t.Owns.Count > 0)
            {
                if (this.GetProperty("culture") != null)
                {
                    t.Owns[0].SetProperty("culture", this.GetProperty("culture").Value);
                }

                if (this.GetProperty("religion") != null)
                {
                    t.Owns[0].SetProperty("religion", this.GetProperty("religion").Value);
                }

                t.Owns[0].ProvinceOwner = t;
            }

            this.SetColorOfChildren(t);

            if(doScript)
            {
                /* t.heldDates.Add(new TitleParser.HeldDate()
                 {
                     chrid = ID,
                     date = SimulationManager.instance.Year
                 });*/
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";
                thing.Add(new ScriptCommand() { Name = "holder", Value = this.ID });
                this.lastImportantYear = SimulationManager.instance.Year;

                t.titleScripts.Add(thing);
            }

            foreach (var titleParser in this.Titles)
            {
                if (titleParser.Liege != null && SimulationManager.instance.AutoValidateRealm && !delayValidate)
                {
                    titleParser.Liege.ValidateRealm(new List<CharacterParser>(), titleParser.TopmostTitle);
                }
            }

            return true;
        }



        public void GiveTitleSoftPlusOneLower(TitleParser t, CharacterParser origHolder, bool conquer = false)
        {
            if (t.Holder == this)
            {
                return;
            }

            if (t.government == "republicpalace" && t.republicdynasty != this.Dynasty.ID)
            {
                return;
            }

            if (this.PrimaryTitle != null && t.government == "republic")
            {
                return;
            }

            if (this.PrimaryTitle != null && t.Rank >= 2 && this.Titles.Any(tt => tt.government == "republic"))
            {
                return;
            }

            if (t.Rank == 0)
            {
                return;
            }

            if (t.Rank == 0 && this.PrimaryTitle != null && this.PrimaryTitle.Rank > 0)
            {
                return;
            }

            if (t.Liege != null && this.PrimaryTitle != null && t.TopmostTitle != this.PrimaryTitle.TopmostTitle)
            {
                return;
            }

            if (t.government == "republicpalace")
            {
                this.GiveTitleSoft(t.Liege);
            }

            this.HadTitle = true;
            //     if (PrimaryTitle != null && t.Rank > PrimaryTitle.Rank)
            //   {
            ////      return;
            //   }

            t.CurrentHolder = null;
            if (this.Titles.Contains(t))
            {
                return;
            }

            if (t != null)
            {
                if (t.Holder != null && t.Holder.PrimaryTitle != null)
                {
                    t.Holder.PrimaryTitle.Log("Holder " + this.ID + " - Primary title: " + t.Holder.PrimaryTitle.Name + " has lost the title: " + t.Name);
                }

                if (this.PrimaryTitle != null)
                {
                    t.Log("Title given to " + this.ID + " - Primary title: " + this.PrimaryTitle.Name);
                    this.PrimaryTitle.Log("Holder (" + this.ID + ") has gained " + t.Name);
                }
                else
                {
                    t.Log("Title given to " + this.ID);
                }
            }

            if (t.Holder != null)
            {
                if (t.Holder.PrimaryTitle == t)
                {
                    t.Holder.PrimaryTitle = null;
                }

                t.Holder.Titles.Remove(t);
            }

            foreach (var titleParser in t.SubTitles)
            {
                if (this.PrimaryTitle == null || t.Rank == this.PrimaryTitle.Rank)
                {
                    if (conquer)
                    {
                        titleParser.Value.ConquerLiege = t;
                    }

               //     titleParser.Value.Liege = t;
                }
            }

            if (this.PrimaryTitle != null && t.Rank < this.PrimaryTitle.Rank)
            {
          //      t.Liege = PrimaryTitle.Liege;

                if (conquer)
                {
                    t.ConquerLiege = this.PrimaryTitle;
                }
            }

            t.Active = true;
            t.Holder = this;
            this._primaryTitle = null;
            if ((this.PrimaryTitle == null || t.Rank > this.PrimaryTitle.Rank))
            {
                this.Titles.Insert(0, t);
                this.PrimaryTitle = t;
            }
            else
            {
                this.Titles.Add(t);
            }

            if (t.Owns.Count > 0)
            {
                if (this.GetProperty("culture") != null)
                {
                    t.Owns[0].SetProperty("culture", this.GetProperty("culture").Value);
                }

                if (this.GetProperty("religion") != null)
                {
                    t.Owns[0].SetProperty("religion", this.GetProperty("religion").Value);
                }

                t.Owns[0].ProvinceOwner = t;
            }

            this.SetColorOfChildren(t);

            {
                /* t.heldDates.Add(new TitleParser.HeldDate()
                 {
                     chrid = ID,
                     date = SimulationManager.instance.Year
                 });*/
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";
                thing.Add(new ScriptCommand() { Name = "holder", Value = this.ID });
                this.lastImportantYear = SimulationManager.instance.Year;
                t.prevLeaderNames.Add(this.ChrName);
                t.titleScripts.Add(thing);
            }

            foreach (var titleParser in t.SubTitles)
            {
                if (t.CapitalProvince.Title == titleParser.Value ||
                    t.CapitalProvince.Title.HasLiegeInChain(titleParser.Value))
                {
                    this.GiveTitleSoftPlusOneLower(titleParser.Value, origHolder, conquer);
                    return;
                }
            }

            foreach (var titleParser in t.SubTitles)
            {
                if (titleParser.Value.Rank > 0)
                {
                    this.GiveTitleSoftPlusOneLower(titleParser.Value, origHolder, conquer);
                }

                return;
            }

            foreach (var titleParser in this.Titles)
            {
                if (titleParser.Liege != null && SimulationManager.instance.AutoValidateRealm)
                {
                    titleParser.Liege.ValidateRealm(new List<CharacterParser>(), titleParser.TopmostTitle);
                }
            }
        }


        public void GiveTitleSoftPlusAllLower(TitleParser t, CharacterParser origHolder, bool conquer = false)
        {
            if (t.Holder == this)
            {
                return;
            }

            if (t.government == "republicpalace" && t.republicdynasty != this.Dynasty.ID)
            {
                return;
            }

            if (this.PrimaryTitle != null && t.government == "republic")
            {
                return;
            }

            if (this.PrimaryTitle != null && t.Rank >= 2 && this.Titles.Any(tt => tt.government == "republic"))
            {
                return;
            }

            if (t.Rank == 0)
            {
                return;
            }

            if (t.Rank == 0 && this.PrimaryTitle != null && this.PrimaryTitle.Rank > 0)
            {
                return;
            }

            if (t.Liege != null && this.PrimaryTitle != null && t.TopmostTitle != this.PrimaryTitle.TopmostTitle)
            {
                return;
            }

            if (t.government == "republicpalace")
            {
                this.GiveTitleSoft(t.Liege);
            }

            this.HadTitle = true;
            //     if (PrimaryTitle != null && t.Rank > PrimaryTitle.Rank)
            //   {
            ////      return;
            //   }

            t.CurrentHolder = null;
            if (this.Titles.Contains(t))
            {
                return;
            }

            if (t != null)
            {
                if (t.Holder != null && t.Holder.PrimaryTitle != null)
                {
                    t.Holder.PrimaryTitle.Log("Holder " + this.ID + " - Primary title: " + t.Holder.PrimaryTitle.Name + " has lost the title: " + t.Name);
                }

                if (this.PrimaryTitle != null)
                {
                    t.Log("Title given to " + this.ID + " - Primary title: " + this.PrimaryTitle.Name);
                    this.PrimaryTitle.Log("Holder (" + this.ID + ") has gained " + t.Name);
                }
                else
                {
                    t.Log("Title given to " + this.ID);
                }
            }

            if (t.Holder != null)
            {
                if (t.Holder.PrimaryTitle == t)
                {
                    t.Holder.PrimaryTitle = null;
                }

                t.Holder.Titles.Remove(t);
            }

            foreach (var titleParser in t.SubTitles)
            {
                if (this.PrimaryTitle == null || t.Rank == this.PrimaryTitle.Rank)
                {
                    if (conquer)
                    {
                        titleParser.Value.ConquerLiege = t;
                    }

                    //     titleParser.Value.Liege = t;
                }
            }

            if (this.PrimaryTitle != null && t.Rank < this.PrimaryTitle.Rank)
            {
             //   t.Liege = PrimaryTitle.Liege;

                if (conquer)
                {
                    t.ConquerLiege = this.PrimaryTitle;
                }
            }

            t.Active = true;
            t.Holder = this;
            this._primaryTitle = null;
            if ((this.PrimaryTitle == null || t.Rank > this.PrimaryTitle.Rank))
            {
                this.Titles.Insert(0, t);
                this.PrimaryTitle = t;
            }
            else
            {
                this.Titles.Add(t);
            }

            if (t.Owns.Count > 0)
            {
                if (this.GetProperty("culture") != null)
                {
                    t.Owns[0].SetProperty("culture", this.GetProperty("culture").Value);
                }

                if (this.GetProperty("religion") != null)
                {
                    t.Owns[0].SetProperty("religion", this.GetProperty("religion").Value);
                }

                t.Owns[0].ProvinceOwner = t;
            }

            this.SetColorOfChildren(t);

            {
                /* t.heldDates.Add(new TitleParser.HeldDate()
                 {
                     chrid = ID,
                     date = SimulationManager.instance.Year
                 });*/
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";
                thing.Add(new ScriptCommand() { Name = "holder", Value = this.ID });
                this.lastImportantYear = SimulationManager.instance.Year;
                t.prevLeaderNames.Add(this.ChrName);
                t.titleScripts.Add(thing);
            }

            var p = t.SubTitles.ToList();
            for (var index = 0; index < p.Count; index++)
            {
                var titleParser = p[index];
                if (titleParser.Value.Rank > 0)
                    if (titleParser.Value.Holder == origHolder || origHolder == null || titleParser.Value.Holder == null ||
                        titleParser.Value.Holder.PrimaryTitle.Rank > titleParser.Value.Rank)
                    {
                        this.GiveTitleSoftPlusAllLower(titleParser.Value, origHolder, conquer);
                    }
                //     return;
            }

            foreach (var titleParser in this.Titles)
            {
                if (titleParser.Liege != null && SimulationManager.instance.AutoValidateRealm)
                {
                    titleParser.Liege.ValidateRealm(new List<CharacterParser>(), titleParser.TopmostTitle);
                }
            }
        }

        public void GiveTitleSoftPlusLowerPrimary(TitleParser t)
        {
            if (t.Holder == this)
            {
                return;
            }

            if (t.government == "republicpalace" && t.republicdynasty != this.Dynasty.ID)
            {
                return;
            }

            if (this.PrimaryTitle != null && t.government == "republic")
            {
                return;
            }

            if (t.Rank == 0)
            {
                return;
            }

            if (t.Rank == 0 && this.PrimaryTitle != null && this.PrimaryTitle.Rank > 0)
            {
                return;
            }

            if (this.PrimaryTitle != null && t.Rank >= 2 && this.Titles.Any(tt => tt.government == "republic"))
            {
                return;
            }

            if (t.Liege != null && this.PrimaryTitle != null && t.TopmostTitle != this.PrimaryTitle.TopmostTitle)
            {
                return;
            }

            if (t.government == "republicpalace")
            {
                this.GiveTitleSoft(t.Liege);
            }

            this.HadTitle = true;
            //     if (PrimaryTitle != null && t.Rank > PrimaryTitle.Rank)
            //   {
            ////      return;
            //   }

            t.CurrentHolder = null;
            if (this.Titles.Contains(t))
            {
                return;
            }

            if (t != null)
            {
                if (t.Holder != null && t.Holder.PrimaryTitle != null)
                {
                    t.Holder.PrimaryTitle.Log("Holder " + this.ID + " - Primary title: " + t.Holder.PrimaryTitle.Name + " has lost the title: " + t.Name);
                }

                if (this.PrimaryTitle != null)
                {
                    t.Log("Title given to " + this.ID + " - Primary title: " + this.PrimaryTitle.Name);
                    this.PrimaryTitle.Log("Holder (" + this.ID + ") has gained " + t.Name);
                }
                else
                {
                    t.Log("Title given to " + this.ID);
                }
            }

            if (t.Holder != null)
            {
                if (t.Holder.PrimaryTitle == t)
                {
                    t.Holder.PrimaryTitle = null;
                }

                t.Holder.Titles.Remove(t);
            }

            foreach (var titleParser in t.SubTitles)
            {
           //     titleParser.Value.Liege = t;
            }

         //   if (PrimaryTitle != null)
          //      t.Liege = PrimaryTitle.Liege;
            t.Active = true;
            t.Holder = this;
            this._primaryTitle = null;
            if ((this.PrimaryTitle == null || t.Rank > this.PrimaryTitle.Rank))
            {
                this.Titles.Insert(0, t);
                this.PrimaryTitle = t;
            }
            else
            {
                this.Titles.Add(t);
            }

            if (t.Owns.Count > 0)
            {
                if (this.GetProperty("culture") != null)
                {
                    t.Owns[0].SetProperty("culture", this.GetProperty("culture").Value);
                }

                if (this.GetProperty("religion") != null)
                {
                    t.Owns[0].SetProperty("religion", this.GetProperty("religion").Value);
                }

                t.Owns[0].ProvinceOwner = t;
            }

            this.SetColorOfChildren(t);

            {
                /* t.heldDates.Add(new TitleParser.HeldDate()
                 {
                     chrid = ID,
                     date = SimulationManager.instance.Year
                 });*/
                ScriptScope thing = new ScriptScope();
                thing.Name = SimulationManager.instance.Year + ".2.1";
                thing.Add(new ScriptCommand() { Name = "holder", Value = this.ID });
                this.lastImportantYear = SimulationManager.instance.Year;

                t.titleScripts.Add(thing);
            }

            foreach (var titleParser in this.Titles)
            {
                if (titleParser.Liege != null && SimulationManager.instance.AutoValidateRealm)
                {
                    titleParser.Liege.ValidateRealm(new List<CharacterParser>(), titleParser.TopmostTitle);
                }
            }

            foreach (var titleParser in t.SubTitles)
            {
                this.GiveTitleSoftPlusLowerPrimary(titleParser.Value);
                return;
            }
           }

        private void SetColorOfChildren(TitleParser t)
        {
            if (t.Owns.Count > 0 && this.Dynasty != null)
            {
                t.Owns[0].SetProperty("color", this.Dynasty.Color);
                t.Owns[0].SetProperty("color2", this.Dynasty.Color);
            }

            foreach (var titleParser in t.SubTitles)
            {
                if (titleParser.Value.Rank >= this.PrimaryTitle.Rank)
                {
                    continue;
                }

                this.SetColorOfChildren(titleParser.Value);
            }
        }

        public List<ProvinceParser> GetProvinceGroup(int i, CharacterParser chr)
        {
            List<ProvinceParser> list = new List<ProvinceParser>();
            List<ProvinceParser> list2 = new List<ProvinceParser>();
            foreach (var titleParser in this.Titles)
            {
                titleParser.GetAllProvinces(list);
            }

            foreach (var provinceParser in list)
            {
                if (!provinceParser.land)
                {
                    continue;
                }

                if (provinceParser.ProvinceTitle == null)
                {
                    continue;
                }

                if (!(provinceParser.Title.Liege == null && (chr == provinceParser.Title.Holder || chr == null)))
                {
                    list2.Add(provinceParser);
                }
            }

            if (list2.Count == 0)
            {
                return list2;
            }

            ProvinceParser p = null;
            p = list2[RandomIntHelper.Next(list2.Count)];

            List<ProvinceParser> provinces = new List<ProvinceParser>();
            provinces.Add(p);

            int last = provinces.Count;

            do
            {
                last = provinces.Count;
                MapManager.instance.FindAdjacent(provinces, i - provinces.Count, chr);
            }
            while (last != provinces.Count && provinces.Count < i);

            return provinces;
        }

        public List<ProvinceParser> GetProvinceGroupSameRealm(int i, CharacterParser chr)
        {
            List<ProvinceParser> list = new List<ProvinceParser>();
            List<ProvinceParser> list2 = new List<ProvinceParser>();
            foreach (var titleParser in this.Titles)
            {
                titleParser.GetAllProvinces(list);
            }

            foreach (var provinceParser in list)
            {
                //      if ((provinceParser.Title.SameRealm(chr.PrimaryTitle)))
                {
                    list2.Add(provinceParser);
                }
            }

            if (list2.Count == 0)
            {
                return list2;
            }

            ProvinceParser p = null;
            p = list2[RandomIntHelper.Next(list2.Count)];

            List<ProvinceParser> provinces = new List<ProvinceParser>();
            provinces.Add(p);

            int last = provinces.Count;

            do
            {
                last = provinces.Count;
                MapManager.instance.FindAdjacentSameRealm(provinces, i - provinces.Count, chr);
            }
            while (last != provinces.Count && provinces.Count < i);

            return provinces;
        }

        static Stack<List<ProvinceParser>> resultsStack = new Stack<List<ProvinceParser>>();

        public void ConvertCountTitlesToDuchies()
        {
            if (this.Titles.Count == 0)
            {
                return;
            }

            int last = 0;
            int timeout = 30;
            List<ProvinceParser> homeless = new List<ProvinceParser>();
            while (this.NumberofCountTitles > 0 && homeless.Count < this.NumberofCountTitles)
            {
                int nc = this.NumberofCountTitles;
                if (last == nc)
                {
                    timeout--;
                }
                else if (timeout <= 0)
                {
                    break;
                }
                else
                {
                    timeout = 10;
                }

                if (timeout == 1)
                {
                }

                last = nc;
                int duchySize = 3;
                if (duchySize == 3 && RandomIntHelper.Next(3) == 0)
                {
                    duchySize = 2;
                }

                var results = this.GetProvinceGroupSameRealm(duchySize, this);

                for (int index = 0; index < results.Count; index++)
                {
                    var provinceParser = results[index];
                    if (provinceParser.Title.Name == "c_kanaidaspin")
                    {
                    }

                    if (provinceParser.Title.Liege != null && provinceParser.Title.Liege != provinceParser.Title.TopmostTitle)
                    {
                        results.Remove(provinceParser);
                        index--;
                    }
                }

                if (results.Count == 1 && results[0].Title.Liege == null)
                {
                    if (!homeless.Contains(results[0]))
                    {
                        homeless.Add(results[0]);
                        timeout++;
                    }
                }

               if (results.Count <= 1)
                {
                    continue;
                }

                foreach (var provinceParser in results)
                {
                    this.RemoveTitle(provinceParser.Title);
                }

                ProvinceParser capital = results[RandomIntHelper.Next(results.Count)];

                var chr = SimulationManager.instance.AddCharacter(this.culture, this.religion);
                var title = TitleManager.instance.CreateDukeScriptScope(capital, chr);
                chr.GiveTitle(title);

                //    results.RemoveAt(0);
                int n = 0;
                foreach (var provinceParser in results)
                {
                    //   var ruler = TitleManager.instance.PromoteNewRuler(provinceParser.Title);
                    //  SimulationManager.instance.characters.Remove(ruler);
                    if (provinceParser.Title.Holder == null)
                    {
                        SimulationManager.instance.AddCharacterForTitle(provinceParser.Title, this.culture, this.religion);
                    }

                    title.AddSub(provinceParser.Title);
                    chr.GiveTitleAsHolder(provinceParser.Title);
                    if (RandomIntHelper.Next(2) == 0 && n != 0)
                    {
                        SimulationManager.instance.AddCharacterForTitle(provinceParser.Title);
                    }

                    n++;
                    //  this.PrimaryTitle.RemoveVassal(provinceParser.Title);
                }
            }

            List<ProvinceParser> blankPlacesToSpreadTo = new List<ProvinceParser>();
            List<ProvinceParser> blankPlacesToSteal = new List<ProvinceParser>();
            int c = 0;
            int lasthomelessCount = 1000000;
            while (homeless.Count > 0 && lasthomelessCount == homeless.Count)
            {
                lasthomelessCount = homeless.Count;
                c++;
                for (int index = 0; index < homeless.Count; index++)
                {
                    var homelessCounty = homeless[index];

                    if (homelessCounty.ProvinceName == "c_kanaidaspin")
                    {
                    }

                    {
                        TitleParser smallestDuchy = null;
                        int smallest = 10000;
                        foreach (var provinceParser in homelessCounty.Adjacent)
                        {
                            if (!provinceParser.land)
                            {
                                continue;
                            }

                            if (provinceParser.ProvinceTitle == null)
                            {
                                blankPlacesToSpreadTo.Add(provinceParser);
                                continue;
                            }

                            if (provinceParser.Title.Liege != null)
                            {
                                if (smallest > provinceParser.Title.Liege.SubTitles.Count)
                                {
                                    smallest = provinceParser.Title.Liege.SubTitles.Count;
                                    smallestDuchy = provinceParser.Title.Liege;
                                }
                            }
                            else
                            {
                                blankPlacesToSteal.Add(provinceParser);
                            }
                        }

                            if (smallestDuchy != null)
                            {
                                var title2 = smallestDuchy;

                                // SimulationManager.instance.characters.Remove(ruler);
                                title2.AddSub(homelessCounty.Title);
                                SimulationManager.instance.AddCharacterForTitle(homelessCounty.Title);
                                homeless.Remove(homelessCounty);
                                index--;
                                break;
                            }
                            else
                            {
                                if (blankPlacesToSpreadTo.Count > 0)
                                {
                                    var province = blankPlacesToSpreadTo[0];

                                    var chr = SimulationManager.instance.AddCharacter(this.culture, this.religion);
                                    var title = TitleManager.instance.CreateDukeScriptScope(province, chr);
                                    chr.GiveTitle(title);
                                    var ctitle = province.CreateTitle();
                                    province.ProvinceTitle = ctitle.Name;
                                    title.AddSub(ctitle);
                                    title.AddSub(homelessCounty.Title);
                                    chr.GiveTitleAsHolder(province.Title);
                                    chr.GiveTitleAsHolder(homelessCounty.Title);
                                    homeless.Remove(homelessCounty);
                                    index--;
                                } else if (blankPlacesToSteal.Count > 0)
                                {
                                    var province = blankPlacesToSteal[0];

                                    var chr = SimulationManager.instance.AddCharacter(this.culture, this.religion);
                                    var title = TitleManager.instance.CreateDukeScriptScope(province, chr);
                                    chr.GiveTitle(title);
                                    if(province.Title.Holder != null)
                                {
                                    province.Title.Holder.RemoveTitle(province.Title);
                                }

                                homelessCounty.Title.Holder.RemoveTitle(homelessCounty.Title);
                                    title.AddSub(province.Title);
                                    title.AddSub(homelessCounty.Title);
                                    chr.GiveTitleAsHolder(province.Title);
                                    chr.GiveTitleAsHolder(homelessCounty.Title);
                                    homeless.Remove(homelessCounty);
                                    index--;
                                }
                                else
                                {
                                }
                           }
                    }
                }
            }

            if (homeless.Count > 0)
            {
            }

            for (int index = 0; index < this.Titles.Count; index++)
            {
                var titleParser = this.Titles[index];
                if (titleParser.Name == "c_kanaidaspin")
                {
                }

                if (titleParser.Liege == null)
                {
                    titleParser.DoSetLiegeEvent(this.PrimaryTitle);
                }

                if (titleParser.Rank == 1)
                {
                    //  if (titleParser.Liege.Rank==2)
                    //       GiveTitle(titleParser.Liege);
                }
            }

            /*
            // now if we're an empire, create some kings! Eep...

            if (PrimaryTitle.Rank == 4)
            {
                timeout = 10;
                while (this.NumberofDukeVassals > 1 && timeout > 0)
                {
                    int nc = this.NumberofDukeVassals;
                    if (last == nc)
                        timeout--;
                    else if (timeout <= 0)
                        break;
                    else
                        timeout = 10;
                    if (timeout == 1)
                    {

                    }
                    last = nc;
                    int kingdomSize = 16 + Rand.Next(16);
                    var results = this.GetProvinceGroupSameRealm(kingdomSize, this);
                    for (int index = 0; index < results.Count; index++)
                    {
                        var provinceParser = results[index];
                        if (provinceParser.Title.Liege == null)
                        {
                            results.Remove(provinceParser);
                            index--;
                        }
                    }

                    CharacterManager.instance.Prune();
                    if (results.Count <= 16)
                        continue;

                    ProvinceParser capital = results[Rand.Next(results.Count)];
                    var title = TitleManager.instance.CreateKingScriptScope(capital);
                    var chr = SimulationManager.instance.AddCharacterForTitle(title);
                    chr.GiveTitle(title);

                    //    results.RemoveAt(0);
                    foreach (var provinceParser in results)
                    {
                        if (!provinceParser.land)
                            continue;
                        if (provinceParser.title == null)
                            continue;

                        var ruler = provinceParser.Title.Liege;
                        if (provinceParser.Title.Liege != null && provinceParser.Title.Liege.Rank == 2)
                        {
                            if (provinceParser.Title.Liege != title)
                            {
                                if(ruler.Liege != null)
                                    ruler.Liege.RemoveVassal(ruler);
                                title.AddSub(ruler);

                            }
                        }
                        //  this.PrimaryTitle.RemoveVassal(provinceParser.Title);
                        if (Rand.Next(10) == 0 && NumberofCountTitles < 6)
                        {
                            GiveTitleAsHolder(title);

                        }
                    }
                    PrimaryTitle.AddSub(title);
                    CharacterManager.instance.Prune();
                }

            }
            */
            /*
                        // Now we're at duke level, do the same for kings (but getting entire duchies into the results list...
                        last = 0;
                        timeout = 30;
                        while (this.NumberofDukeVassals > 0)
                        {

                            if (last == NumberofDukeTitles)
                                timeout--;
                            if (timeout <= 0)
                                break;
                            last = NumberofDukeTitles;
                             int kingdomSize = 16 + Rand.Next(32);
                            var results = this.GetProvinceGroup(kingdomSize, this);
                            for (int index = 0; index < results.Count; index++)
                            {
                                var provinceParser = results[index];
                                if (provinceParser.Title.Liege != null)
                                {
                                    foreach (var titleParser in provinceParser.Title.Liege.SubTitles)
                                    {
                                        if (titleParser.Value.Owns.Count > 0 && !results.Contains(titleParser.Value.Owns[0]))
                                        {
                                            results.Add(titleParser.Value.Owns[0]);
                                        }
                                    }
                                }
                            }
                            ProvinceParser capital = results[Rand.Next(results.Count)];
                            var title = TitleManager.instance.CreateKingScriptScope(capital);
                            var chr = SimulationManager.instance.AddCharacterForTitle(title);
                          //   chr.GiveTitle(title);

                            //    results.RemoveAt(0);
                            foreach (var provinceParser in results)
                            {
                               // var ruler = SimulationManager.instance.AddCharacterForTitle(provinceParser.Title.Liege);
                               // SimulationManager.instance.characters.Remove(ruler);
                                title.AddSub(provinceParser.Title.Liege);
                                this.PrimaryTitle.RemoveVassal(provinceParser.Title.Liege);
                            }
                            PrimaryTitle.AddSub(title);

                        }*/
            return;
            if (this.Titles.Count == 0)
            {
                return;
            }

            var t = this.Titles[0].GetAllProvinces()[0];
            this.GiveTitle(t.Title);
            this.GiveTitle(t.Title.Liege);
            {
            }

            while (this.NumberofDukeTitles > 0)
            {
                int duchySize = 2 + RandomIntHelper.Next(3);
                var results = this.GetProvinceGroup(duchySize, this);
                ProvinceParser capital = results[RandomIntHelper.Next(results.Count)];
                var title = TitleManager.instance.CreateDukeScriptScope(capital);
                var chr = SimulationManager.instance.AddCharacterForTitle(title);
                chr.GiveTitle(title);

                //    results.RemoveAt(0);
                foreach (var provinceParser in results)
                {
                    var ruler = TitleManager.instance.PromoteNewRuler(provinceParser.ProvinceOwner);
                    SimulationManager.instance.characters.Remove(ruler);
                    title.AddSub(provinceParser.Title);
                }
            }

            if (this.Titles.Count == 0)
            {
                return;
            }
            /*

        int numCounts = this.NumberofCountTitles;
        int DukeSize = 3;
        int KingSize = 16;
        int EmperorSize = 60;
        if (numCounts > EmperorSize && PrimaryTitle.Liege == null)
        {
            var capital = PrimaryTitle.Owns[0];
            var title = TitleManager.instance.CreateEmperor(capital);
            GiveTitle(title);

            while (numCounts > 1)
            {
               // Make me an emperor, create new vassals with collections of counts in them > 16, then vassalize them
               List<ProvinceParser> results = GetProvinceGroup(Rand.Next((KingSize / 2), KingSize * 4));
                results.Remove(capital);
               if (results.Count > 0)
               {
                   var king = results[0].Title;// TitleManager.instance.CreateKingScriptScope(results[0]);
                   var chr = SimulationManager.instance.AddCharacterForTitle(king);
               //    results.RemoveAt(0);
                   foreach (var provinceParser in results)
                   {
                       chr.GiveTitle(provinceParser.Title);
                   }
                   //king.Liege = title;
                 //  chr.ConvertCountTitlesToDuchies();
                   chr.PrimaryTitle.Liege = title;
                   foreach (var provinceParser in results)
                   {
                       chr.PrimaryTitle.AddSub(provinceParser.Title);
                   }

               }
               numCounts = this.NumberofCountTitles;
            }
            return;
        }
        if (numCounts > KingSize && (PrimaryTitle.Liege == null || Liege.PrimaryTitle.Rank == 4))
        {
            var capital = PrimaryTitle.Owns[0];
            var title = TitleManager.instance.CreateKingScriptScope(capital);
            GiveTitle(title);

            while (numCounts > 1)
            {
                List<ProvinceParser> results = GetProvinceGroup(Rand.Next((DukeSize), (DukeSize * 2)-1));
                results.Remove(capital);
                if (results.Count > 0)
                {
                    var duke = results[0].Title;// TitleManager.instance.CreateKingScriptScope(results[0]);
                    chr = CharacterManager.instance.GetNewCharacter();
                    chr.GiveTitle(title);


                    //    results.RemoveAt(0);
                    foreach (var provinceParser in results)
                    {
                        chr.GiveTitle(provinceParser.Title);

                    }
                //    duke.Liege = title;
                    chr.PrimaryTitle = null;
                    chr.ConvertCountTitlesToDuchies();
                    foreach (var provinceParser in results)
                    {
                        chr.PrimaryTitle.AddSub(provinceParser.Title);
                    }
                }
                numCounts = this.NumberofCountTitles;
            }
            return;
            // Make me a king, create new vassals with collectin of counts in them >= 3 then feudalize them
        }
        if (Liege == null)
        {

        }
        if (numCounts >= DukeSize && (PrimaryTitle.Liege == null || Liege.PrimaryTitle.Rank >= 3))
        {
            var capital = PrimaryTitle.Owns[0];
            var title = TitleManager.instance.CreateDukeScriptScope(capital);
            GiveTitle(title);

            while (numCounts > 1)
            {
                List<ProvinceParser> results = GetProvinceGroup(Math.Min(numCounts-1, Rand.Next((DukeSize), (DukeSize * 2) - 1)));
                results.Remove(capital);
                if (results.Count > 0)
                {


                     //    results.RemoveAt(0);
                    foreach (var provinceParser in results)
                    {
                        TitleManager.instance.PromoteNewRuler(provinceParser.Title);

                    }

                   // chr.ConvertCountTitlesToDuchies();
                }
                numCounts = this.NumberofCountTitles;
            }

            return;
            // Make me a duke, create new count vassals.
        }
        PrimaryTitle = null;
     /*   int numCounts = this.NumberofCountTitles;
        int nGiveup = 10;
        while (numCounts > MaxDeminse && nGiveup > 0)
        {
            var duke= CreateDuke();
            numCounts = this.NumberofCountTitles;
            nGiveup--;
            if (PrimaryTitle.Rank == 3)
            {
                if (duke != null)
                {
                    TitleManager.instance.PromoteNewRuler(duke);
                }
            }
        }
        int numDukes = this.NumberofDukeTitles;
        nGiveup = 10;
        while (numDukes > PreferedKingdomSize && numDukes > 0)
        {
            bool bGive = false;
            if (PrimaryTitle.Rank == 3)
            {
                bGive = true;
            }

            var king = CreateKing();
            if (bGive)
            {
                if (king != null)
                {
                    TitleManager.instance.PromoteNewRuler(king);
                }

            }
            numCounts = this.NumberofDukeTitles;
            nGiveup--;
        }
        int numKings = this.NumberofKingTitles;
        nGiveup = 10;
        while (numKings > 1)
        {
            PrimaryTitle = null;
            var prim = PrimaryTitle;
            for (int index = 0; index < Titles.Count; index++)
            {
                var titleParser = Titles[index];
                if (titleParser != prim && titleParser.Rank == 3)
                {
                    TitleManager.instance.PromoteNewRuler(titleParser);
                    break;
                }
            }
            numKings = this.NumberofKingTitles;
        }

        */
        }

        public void RemoveTitleAddEvent(TitleParser title)
        {
            this.Titles.Remove(title);
            title.Holder = null;

            ScriptScope thing = new ScriptScope();
            thing.Name = SimulationManager.instance.Year + ".1.1";
            thing.Add(new ScriptCommand() { Name = "holder", Value = 0 });
            this.lastImportantYear = SimulationManager.instance.Year;

            title.titleScripts.Add(thing);
            if (title == this.PrimaryTitle)
            {
                this.PrimaryTitle = null;
            }
        }

        public void RemoveTitle(TitleParser title)
        {
            this.Titles.Remove(title);
            title.Holder = null;
        }

        public void GiveTitleAsHolder(TitleParser title)
        {
            title.CurrentHolder = this;
        }


        public int NumberofCountTitles
        {
            get
            {
                int c = 0;
                foreach (var titleParser in this.Titles)
                {
                    if (titleParser.Rank == 1)
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public int NumberofDukeVassals
        {
            get
            {
                int c = 0;
                foreach (var titleParser in this.Titles)
                {
                    foreach (var sub in titleParser.SubTitles.Values)
                    {
                        if (sub.Rank == 2)
                        {
                            c++;
                        }
                    }
                }

                return c;
            }
        }

        public int NumberofKingTitles
        {
            get
            {
                int c = 0;
                foreach (var titleParser in this.Titles)
                {
                    if (titleParser.Rank == 3)
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public int NumberofEmpireTitles
        {
            get
            {
                int c = 0;
                foreach (var titleParser in this.Titles)
                {
                    if (titleParser.Rank == 4)
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public int NumberofDukeTitles
        {
            get
            {
                int c = 0;
                foreach (var titleParser in this.Titles)
                {
                    if (titleParser.Rank == 2)
                    {
                        c++;
                    }
                }

                return c;
            }
        }

        public bool TickDisable { get; set; }

        private void RemoveDeathDates(ScriptScope scope)
        {
            //     foreach (var child in scope.Children)
            {
                if (scope.Children.Count > 0)
                {
                    ScriptScope c = scope;
                    if (c.Children[0] is ScriptCommand && (c.Children[0] as ScriptCommand).Name == "death")
                    {
                        c.Parent.Children.Remove(c);
                        return;
                    }
                }
            }
        }

        public void MakeAlive()
        {
            var arr = this.Scope.Children.ToArray();
            //   Scope.Delete("father");
            foreach (var child in arr)
            {
                if (child is ScriptScope)
                {
                    ScriptScope scope = child as ScriptScope;
                    this.RemoveDeathDates(scope);
                }
            }
        }

        public void KillTitles()
        {
            foreach (var titleParser in this.Titles)
            {
                titleParser.Kill();
            }
        }

        public void MakeEmperor()
        {
            var emp = TitleManager.instance.CreateEmpireScriptScope(this.Titles[RandomIntHelper.Next(this.Titles.Count)].CapitalProvince);

            this.GiveTitle(emp);
            TitleParser tit = emp.CapitalProvince.Title;
            while (tit.Liege != null && !this.Titles.Contains(tit) && tit.Liege.Rank > tit.Rank)
            {
                this.GiveTitleAsHolder(tit);
            }
        }

        public void MakeKing()
        {
            var emp = TitleManager.instance.CreateKingScriptScope(this.Titles[RandomIntHelper.Next(this.Titles.Count)].CapitalProvince);

            this.GiveTitle(emp);
            this.GiveTitleAsHolder(emp.CapitalProvince.Title);
            TitleParser tit = emp.CapitalProvince.Title;
            while (tit.Liege != null && !this.Titles.Contains(tit) && tit.Liege.Rank > tit.Rank)
            {
                this.GiveTitleAsHolder(tit);
            }
        }

        public CharacterParser Father { get; set; }

        public CharacterParser Mother { get; set; }

        public List<CharacterParser> Kids = new List<CharacterParser>();

        public List<CharacterParser> Spouses = new List<CharacterParser>();
        public List<CharacterParser> Concubines = new List<CharacterParser>();
        private string _chrName;
        public List<ProvinceIsland> SeperateIslands = new List<ProvinceIsland>();

        public void AddDateEvent(int year, int month, int day, ScriptCommand command)
        {
            int indexForDate = 0;
            string date = year.ToString() + "." + month.ToString() + "." + day.ToString();
            int index = 0;
            bool found = false;
            ScriptScope foundScope = null;
            foreach (var child in this.Scope.Children)
            {
                if (child is ScriptScope)
                {
                    var scope = ((ScriptScope)child);
                    var split = scope.Name.Split('.');
                    if (split.Length == 3)
                    {
                        int y = Convert.ToInt32(split[0]);
                        int m = Convert.ToInt32(split[1]);
                        int d = Convert.ToInt32(split[2]);
                        if ((y == year && m == month && d == day))
                        {
                            found = true;
                            foundScope = child as ScriptScope;
                            break;
                        }

                        if (y > year || (y == year && m > month) || (y == year && m == month && d > day))
                        {
                            break;
                        }
                    }
                }

                index++;
            }

            indexForDate = index;
            ScriptScope s = null;
            if (found)
            {
                s = new ScriptScope(date);
            }
            else
            {
                s = new ScriptScope(date);
                this.Scope.Insert(indexForDate, s);
            }

            s.Add(command);
            command.Parent = s;
        }

        private CharacterParser GetSuitableMotherForChild(string religion, string culture, int min, int max, int year)
        {
            CharacterParser mother = null;

            if (RandomIntHelper.Next(3) == 0)
            {
                mother = CharacterManager.instance.FindUnmarriedChildbearingAgeWomen(year, religion, culture);
                if (mother == null)
                {
                    int nn = year - min;
                    int snn = year - max;
                    max -= 16 - snn;
                    if (max < min)
                    {
                        return null;
                    }

                    return CharacterManager.instance.CreateNewHistoricCharacter(DynastyManager.instance.GetDynasty(this.Culture), true, religion, culture, RandomIntHelper.Next(min, max));
                }
                else
                {
                    int n = 0;
                }
            }
            else
            {
                int nn = year - min;
                int snn = year - max;
                max -= 16 - snn;
                if (max < min)
                {
                    return null;
                }

                return CharacterManager.instance.CreateNewHistoricCharacter(DynastyManager.instance.GetDynasty(this.Culture), true, religion, culture, RandomIntHelper.Next(min, max));
            }

            return mother;
        }

        public void CreateFamily(int depth = 0, int maxdepth = 4, int minYearForHeirs = -1)
        {
            if (depth > maxdepth)
            {
                return;
            }

            int deathdate = -1;
            if (depth == 0)
            {
                deathdate = 768 - 1;
            }

            if (deathdate <= this.YearOfBirth)
            {
                deathdate = this.YearOfBirth + 1;
            }

            this.Father = CharacterManager.instance.CreateNewHistoricCharacter(this.Dynasty, false, this.religion, this.culture, this.YearOfBirth - (16 + RandomIntHelper.Next(30)), deathdate);
            int max = this.YearOfBirth - 19;
            int min = this.Father.YearOfBirth - 5;

            this.Mother = this.GetSuitableMotherForChild(this.religion, this.culture, min, max, this.YearOfBirth);
            if (this.Mother.ID == 1002182)
            {
            }

            this.Father.Spouses.Add(this.Mother);
            this.Mother.Spouses.Add(this.Father);
            if (this.Age > 16 && this.Spouses.Count < this.Religion.max_wives && !this.isFemale)
            {
                int numWives = 1;
                if (this.Age > 30)
                {
                    numWives++;
                }

                if (this.Age > 40)
                {
                    numWives++;
                }

                numWives = 1;
                min = this.YearOfBirth - 3;
                max = this.YearOfBirth + 16;
                max = DateHelper.MakeDOBAtLeastAdult(max);
                numWives = Math.Min(numWives, this.Religion.max_wives - this.Spouses.Count);

                int dateWhenSixteen = this.YearOfBirth + 16;
                int startAdulthood = dateWhenSixteen;
                if (minYearForHeirs > startAdulthood)
                {
                    startAdulthood = minYearForHeirs;
                }
            }

            if (this.Father.YearOfDeath < this.YearOfBirth)
            {
                this.Father.YearOfDeath = this.YearOfBirth + 1;
            }

            if (this.Mother.YearOfDeath < this.YearOfBirth)
            {
                this.Mother.YearOfDeath = this.YearOfBirth + 1;
            }

            this.Father.CreateFamily(depth + 1, maxdepth);
            this.Father.Kids.Add(this);
            this.Mother.Kids.Add(this);
            if (depth < 4)
            {
                this.Mother.CreateFamily(depth + 1, maxdepth);
            }

            this.Father.UpdateCultural();
            this.Mother.UpdateCultural();
            //  Father.SetupExistingDynasty();
            //   Mother.SetupExistingDynasty();
        }

        public CharacterParser CreateKidWith(CharacterParser otherParent, int dateOfBirth)
        {
            var kid = CharacterManager.instance.CreateNewHistoricCharacter(this.Dynasty, RandomIntHelper.Next(2) == 0, this.religion, this.culture, dateOfBirth, -1, false);
            if (!this.isFemale)
            {
                kid.Father = this;
                kid.Mother = otherParent;
            }
            else
            {
                kid.Mother = this;
                kid.Father = otherParent;
            }

            kid.SetupExistingDynasty();
            this.Kids.Add(kid);
            otherParent.Kids.Add(kid);

            return kid;
        }

        public ReligionParser Religion
        {
            get { return ReligionManager.instance.ReligionMap[this.religion]; }
        }

        public int Age
        {
            get { return SimulationManager.instance.Year - this.YearOfBirth; }
        }

        public int YearOfBirth { get; set; }

        public int YearOfDeath { get; set; }

        public CultureParser Culture
        {
            get
            {
                while (!CultureManager.instance.CultureMap.ContainsKey(this.culture))
                {
                    this.culture = CultureManager.instance.AllCultures[RandomIntHelper.Next(CultureManager.instance.AllCultures.Count)].Name;
                }

                return CultureManager.instance.CultureMap[this.culture];
            }
        }

        public bool IsAlive {
            get { return SimulationManager.instance.Year < this.YearOfDeath; }
        }

        public bool IsMarried
        {
            get { return this.Spouses.Count > 0 && this.Spouses[this.Spouses.Count - 1].IsAlive; }
        }

        public CharacterParser CurrentSpouse
        {
            get
            {
                if (this.Spouses.Count == 0)
                {
                    return null;
                }

                if (!this.Spouses[this.Spouses.Count - 1].IsAlive)
                {
                    return null;
                }

                return this.Spouses[this.Spouses.Count - 1];
            }
        }

        public CharacterParser Heir
        {
            get
            {
                if (this.ID == 70560)
                {
                }

                //  if (Kids.Count > 0)
                {
                    bool firstMale = true;
                    bool firstFemale = false;
                    bool allowMale = true;
                    bool allowFemale = true;

                    if (this.PrimaryTitle.GenderLaw == "enatic_succession")
                    {
                        allowMale = false;
                        firstMale = false;
                        firstFemale = true;
                    }

                    if (this.PrimaryTitle.GenderLaw == "agnatic_succession")
                    {
                        allowFemale = false;
                    }

                    if (this.PrimaryTitle.GenderLaw == "enatic_cognatic_succession")
                    {
                        firstMale = false;
                        firstFemale = true;
                    }

                    if (this.PrimaryTitle.GenderLaw == "true_cognatic_succession")
                    {
                        firstMale = false;
                        firstFemale = false;
                    }

                    if (this.PrimaryTitle.TopmostTitle.government != "feudalism" &&
                        this.PrimaryTitle.TopmostTitle.government != "republic")
                    {
                        allowFemale = false;
                        allowMale = true;
                    }

                    if (firstMale)
                        foreach (var characterParser in this.Kids)
                        {
                            if (characterParser.IsAlive && !characterParser.isFemale)
                            {
                                if (!characterParser.Purged)
                                {
                                    return characterParser;
                                }
                            }
                        }

                    if (firstFemale)
                        foreach (var characterParser in this.Kids)
                        {
                            if (characterParser.IsAlive && characterParser.isFemale)
                            {
                                if (!characterParser.Purged)
                                {
                                    return characterParser;
                                }
                            }
                        }


                    foreach (var characterParser in this.Kids)
                    {
                        if (characterParser.IsAlive && ((characterParser.isFemale && allowFemale) || (!characterParser.isFemale && allowMale)))
                        {
                            if (!characterParser.Purged)
                            {
                                return characterParser;
                            }
                        }
                    }

                    if (this.PrimaryTitle != null && this.PrimaryTitle.government == "republic")
                    {
                        bool fm = !firstMale;
                        if (!firstMale && !firstFemale)
                        {
                            fm = RandomIntHelper.Next(2) == 0;
                        }

                        if (!allowFemale)
                        {
                            fm = false;
                        }

                        if (!allowMale)
                        {
                            fm = true;
                        }

                        var republicdynasties = this.PrimaryTitle.republicdynasties;
                        if (this.PrimaryTitle.republicdynasties.Count == 0 && this.PrimaryTitle.Liege.Holder != this)
                        {
                            return this.PrimaryTitle.Liege.Holder;
                        }
                        else if (this.PrimaryTitle.republicdynasties.Count == 0)
                        {
                            republicdynasties = this.PrimaryTitle.Liege.republicdynasties;
                        }

                        return CharacterManager.instance.CreateNewCharacter(DynastyManager.instance.DynastyMap[republicdynasties[RandomIntHelper.Next(republicdynasties.Count)]], fm,
                        SimulationManager.instance.Year - 16, this.religion, this.culture);
                    }

                    if (this.PrimaryTitle.Liege != null && this.PrimaryTitle.Liege.Holder != null && this.PrimaryTitle.Liege.Holder != this)
                    {
                        foreach (var titleParser in this.PrimaryTitle.Liege.SubTitles)
                        {
                            if (titleParser.Value != this.PrimaryTitle)
                            {
                                if (titleParser.Value.Holder != null && !titleParser.Value.Holder.Purged)
                                    if (RandomIntHelper.Next(4) == 0 && titleParser.Value.Holder != null)
                                    {
                                        return titleParser.Value.Holder;
                                    }
                            }
                        }

                        return this.PrimaryTitle.Liege.Holder;
                    }



                    if (this.PrimaryTitle.SubTitles.Count > 0)
                    {
                        foreach (var titleParser in this.PrimaryTitle.SubTitles)
                        {
                            if (titleParser.Value.Holder != this && titleParser.Value.Holder != null && RandomIntHelper.Next(3) == 0)
                            {
                                if(!titleParser.Value.Holder.Purged)
                                {
                                    return titleParser.Value.Holder;
                                }
                            }
                        }
                    }

                    if (RandomIntHelper.Next(2) == 0)
                    {
                        if (firstMale)
                        {
                            var re = CharacterManager.instance.FindUnlandedMan(SimulationManager.instance.Year - 16, this.religion, this.culture);

                            if (re != null)
                                if (!re.Purged)
                                {
                                    return re;
                                }
                        }
                        else if (firstFemale)
                        {
                            var re = CharacterManager.instance.FindUnmarriedChildbearingAgeWomen(SimulationManager.instance.Year - 16, this.religion, this.culture);

                            if (re != null)
                                if (!re.Purged)
                                {
                                    return re;
                                }
                        }
                    }

                    bool fem = !firstMale;
                    if (!firstMale && !firstFemale)
                    {
                        fem = RandomIntHelper.Next(2)==0;
                    }

                    if (!allowFemale)
                    {
                        fem = false;
                    }

                    if (!allowMale)
                    {
                        fem = true;
                    }

                    return CharacterManager.instance.CreateNewCharacter(null, fem,
                        SimulationManager.instance.Year - 16, this.religion, this.culture);
                }
            }
        }

        public bool Purged { get; set; }

        public CharacterParser NonRelatedHeir
        {
            get
            {
                if (this.ID == 70560)
                {
                }

                bool firstMale = true;
                bool firstFemale = false;
                bool allowMale = true;
                bool allowFemale = true;

                if (this.PrimaryTitle.GenderLaw == "enatic_succession")
                {
                    allowMale = false;
                    firstMale = false;
                    firstFemale = true;
                }

                if (this.PrimaryTitle.GenderLaw == "agnatic_succession")
                {
                    allowFemale = false;
                }

                if (this.PrimaryTitle.GenderLaw == "enatic_cognatic_succession")
                {
                    firstMale = false;
                    firstFemale = true;
                }

                if (this.PrimaryTitle.GenderLaw == "true_cognatic_succession")
                {
                    firstMale = false;
                    firstFemale = false;
                }

                if (this.PrimaryTitle.TopmostTitle.government != "feudalism" &&
                      this.PrimaryTitle.TopmostTitle.government != "republic")
                {
                    allowFemale = false;
                    allowMale = true;
                }

                if (this.PrimaryTitle != null && this.PrimaryTitle.government == "republic")
                {
                    bool fm = !firstMale;
                    if (!firstMale && !firstFemale)
                    {
                        fm = RandomIntHelper.Next(2) == 0;
                    }

                    if (!allowFemale)
                    {
                        fm = false;
                    }

                    if (!allowMale)
                    {
                        fm = true;
                    }

                    var republicdynasties = this.PrimaryTitle.republicdynasties;
                    if (this.PrimaryTitle.republicdynasties.Count == 0 && this.PrimaryTitle.Liege.Holder != this)
                    {
                        return this.PrimaryTitle.Liege.Holder;
                    }
                    else if (this.PrimaryTitle.republicdynasties.Count == 0)
                    {
                        republicdynasties = this.PrimaryTitle.Liege.republicdynasties;
                    }

                    return CharacterManager.instance.CreateNewCharacter(DynastyManager.instance.DynastyMap[republicdynasties[RandomIntHelper.Next(republicdynasties.Count)]], fm,
                    SimulationManager.instance.Year - 16, this.religion, this.culture);
                }

                bool fem = !firstMale;
                if (!firstMale && !firstFemale)
                {
                    fem = RandomIntHelper.Next(2) == 0;
                }

                if (!allowFemale)
                {
                    fem = false;
                }

                if (!allowMale)
                {
                    fem = true;
                }


                //  if (Kids.Count > 0)
                if (RandomIntHelper.Next(12) == 0)
                    CharacterManager.instance.CreateNewCharacter(null, fem,
                        SimulationManager.instance.Year - 16, this.religion, this.culture);

                {
                    if (!fem)
                    {
                        var re = CharacterManager.instance.FindUnlandedMan(SimulationManager.instance.Year - 16,
                            this.religion, this.culture);

                        if (re != null)
                            if (!re.Purged)
                            {
                                return re;
                            }
                    }
                    else
                    {
                        var re = CharacterManager.instance.FindUnmarriedChildbearingAgeWomen(SimulationManager.instance.Year - 16,
                           this.religion, this.culture);

                        if (re != null)
                            if (!re.Purged)
                            {
                                return re;
                            }
                    }

                    return CharacterManager.instance.CreateNewCharacter(null, fem,
                        SimulationManager.instance.Year - 16, this.religion, this.culture);
                }
            }
        }

        public bool HadTitle { get; set; }

        public int NumLivingKids
        {
            get
            {
                int count = 0;
                foreach (var characterParser in this.Kids)
                {
                    if (characterParser.IsAlive)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public bool IsConquerer { get; set; }

        public bool IsMinorConquerer { get; set; }

        public int RealmSize
        {
            get
            {
                int size = 0;

                foreach (var titleParser in this.Titles)
                {
                    foreach (var provinceParser in titleParser.GetAllProvinces())
                    {
                        size += provinceParser.ActiveBaronies;
                    }
                }

                return size;
            }
        }

        public bool IsMajorPlayer { get; set; }

        public bool IsMinorPlayer { get; set; }

        public ProvinceParser EffectiveCapitalProvince
        {
            get
            {
                if (this.PrimaryTitle.CapitalProvince.Title.Holder != null && this.PrimaryTitle.CapitalProvince.Title.Holder == this)
                {
                    return this.PrimaryTitle.CapitalProvince;
                }

                foreach (var titleParser in this.Titles)
                {
                    if (titleParser.Rank == 1)
                    {
                        return titleParser.Owns[0];
                    }
                }

                foreach (var titleParser in this.PrimaryTitle.SubTitles)
                {
                    if (titleParser.Value.Rank == 0)
                    {
                        continue;
                    }

                    if (titleParser.Value.Holder != null && titleParser.Value.Holder != this)
                    {
                        return titleParser.Value.Holder.EffectiveCapitalProvince;
                    }
                }

                return this.PrimaryTitle.CapitalProvince;
            }
        }

        public int DistanceBeforeImportantDate
        {
            get
            {
                int lowest = SimulationManager.instance.Year - this.YearOfBirth;
                foreach (var importantYear in BookmarkManager.instance.ImportantYears)
                {
                        if (importantYear - this.YearOfBirth < lowest && importantYear - this.YearOfBirth > 0)
                        {
                            lowest = importantYear - this.YearOfBirth;
                        }
                }

                if (SimulationManager.instance.Year - this.YearOfDeath >= 0 &&
                    SimulationManager.instance.Year - this.YearOfDeath < lowest)
                {
                    lowest = SimulationManager.instance.Year - this.YearOfDeath;
                }

                foreach (var importantYear in BookmarkManager.instance.ImportantYears)
                {
                    if (importantYear - this.YearOfDeath < lowest && importantYear - this.YearOfDeath > 0)
                    {
                        lowest = importantYear - this.YearOfDeath;
                    }
                }

                return lowest;
            }
        }

        public string Behaviour { get; set; }

        public List<ProvinceIsland> Islands { get; set; }

        public ProvinceIsland MainIsland { get; set; }

        public TitleParser UsurpCountTitle()
        {
            var list = this.PrimaryTitle.GetAllProvinces();

            var options = new List<TitleParser>();
            foreach (var provinceParser in list)
            {
                if (provinceParser.Title.Holder == null || !provinceParser.Title.AnyHolder() || provinceParser.Title.Holder.PrimaryTitle.Rank == 1)
                {
                    if(!options.Contains(provinceParser.Title) && !this.Titles.Contains(provinceParser.Title))
                    {
                        options.Add(provinceParser.Title);
                    }
                }
            }

            if (options.Count > 0)
            {
                var r = options[RandomIntHelper.Next(options.Count)];
                this.GiveTitleSoft(r);
                return r;
            }

            return null;
        }

        public bool AddDated(string date, string command, ScriptScope scope = null)
        {
            bool added = false;
            bool useDate = !command.Contains("=") ;
                foreach (var child in this.Scope.Children)
            {
                if (scope != null)
                {
                    if (child is ScriptCommand)
                    {
                        ScriptCommand c2 = (ScriptCommand)child;
                        string test = date;
                        if (!useDate)
                        {
                            test = command.Split('=')[0].Trim();
                        }

                        if (c2.Name == test)
                        {
                            // if (c.Value.ToString().Split('.').Length == 3)
                            {
                                if (useDate)
                                {
                                    c2.Value = date;
                                }
                                else
                                {
                                    c2.Value = command.Split('=')[1].Trim();
                                }

                                scope.Name = c2.Value.ToString();
                                return true;
                            }
                        }
                    }
                }
                else if (child is ScriptScope)
                {
                    added = this.AddDated(date, command, child as ScriptScope);
                    if (added)
                    {
                        return added;
                    }
                }
            }

            if (scope == null && !added)
            {
              //  if (scope == null)
                {
                    ScriptScope s = new ScriptScope(date);
                    ScriptCommand c = new ScriptCommand();
                    c.Name = command;
                    if (!useDate)
                    {
                        c.Name = command.Split('=')[0].Trim();
                    }

                    {
                        // if (c.Value.ToString().Split('.').Length == 3)
                        {
                            if (useDate)
                            {
                                c.Value = date;
                            }
                            else
                            {
                                c.Value = command.Split('=')[1].Trim();
                            }

                            s.Name = date;
                            s.Add(c);
                            added = true;
                        }
                    }

                  //  s.Add(c);
                    this.Scope.SetChild(s);
                }
            }

            return added;
        }


        public void DoFamilyDatesOfBirth()
        {
            this.AddSiblings();
            CharacterManager.instance.SetAllDates(this.YearOfBirth, this.YearOfDeath, this.Scope, this.Titles.Count > 0);

            int birthYear = this.YearOfBirth - 1;
            int deathYear = this.YearOfBirth + 1;
            if (this.Father != null)
            {
                foreach (var characterParser in this.Father.Kids)
                {
                    if (characterParser.YearOfBirth - 1 < birthYear)
                    {
                        birthYear = characterParser.YearOfBirth - 1;
                    }

                    if (characterParser.YearOfBirth + 1 > deathYear)
                    {
                        deathYear = characterParser.YearOfBirth + 1;
                    }

                    if (characterParser != this)
                    {
                        CharacterManager.instance.SetAllDates(characterParser.YearOfBirth, characterParser.YearOfDeath, characterParser.Scope, this.Titles.Count > 0);
                    }
                }
            }

            // now we have a range that definitely covers all the kids.
            int parentMarriage = birthYear - 1;
            // now make the parents definitely 16 years old before marriage...
            int parentBirthYear = parentMarriage - 16;
            int parentDeathYear = deathYear + 1;
            if (this.Father != null)
            {
                this.Father.YearOfBirth = parentBirthYear - 16;
                this.Father.YearOfDeath = parentDeathYear + 150 + RandomIntHelper.Next(150);

                this.Father.DoFamilyDatesOfBirth();
            }

            if (this.Mother != null)
            {
                this.Mother.YearOfBirth = parentBirthYear - RandomIntHelper.Next(5);
                this.Mother.YearOfDeath = parentDeathYear + 150 + RandomIntHelper.Next(150);
                this.Mother.DoFamilyDatesOfBirth();
            }


            if (this.Father != null && this.Mother != null)
            {
                this.Mother.AddDateEvent(parentMarriage, 1, 1, new ScriptCommand("add_spouse", this.Father.ID, null));
                this.Father.AddDateEvent(parentMarriage, 1, 1, new ScriptCommand("add_spouse", this.Mother.ID, null));
            }
        }

        public void AddSiblings()
        {
            if (this.Mother == null || this.Father == null)
            {
                return;
            }

            int num = RandomIntHelper.Next(8);

            int yearOfBirth = this.YearOfBirth + 1;

            for (int n = 0; n < num; n++)
            {
                yearOfBirth += RandomIntHelper.Next(3)+1;

                var kid = this.Father.CreateKidWith(this.Mother, yearOfBirth);

                kid.YearOfDeath = yearOfBirth + 150 + RandomIntHelper.Next(160);
            }
        }

        public void GiveNickname()
        {
            string nick = NicknameManager.instance.getNick(this);
            this.Scope.Add(new ScriptCommand("give_nickname",nick, this.Scope));
        }

        public void SortProvincesToCapital(List<ProvinceParser> provinceParsers)
        {
            provinceParsers.Sort(this.DistanceToCapital);
        }

        public TitleParser GetMostAppropriateTitleForLiege(TitleParser toTake)
        {
            if (this.Titles.Count == 1)
            {
                return this.PrimaryTitle;
            }

            var choices = new List<TitleParser>(this.Titles);

            for (int index = 0; index < choices.Count; index++)
            {
                var titleParser = choices[index];
                if (titleParser.Rank <= toTake.Rank)
                {
                    choices.Remove(titleParser);
                    index--;
                }
            }

            if (choices.Count > 0)
            {
                for (int index = 0; index < choices.Count; index++)
                {
                    var titleParser = choices[index];

                    if (!titleParser.Adjacent(toTake))
                    {
                        choices.Remove(titleParser);
                        index--;
                    }
                }
            }

            if (choices.Count > 0)
            {
                var choice = choices[RandomIntHelper.Next(choices.Count)];
                return choice;
            }
            else
            {
                foreach (var titleParser in this.Titles)
                {
                    if (titleParser.Rank > toTake.Rank + 1)
                    {
                        if (titleParser.Holder != null && titleParser.Holder != this)
                        {
                            var sub = titleParser.Holder.GetMostAppropriateTitleForLiege(toTake);

                            if (sub != null)
                            {
                                return sub;
                            }
                        }
                    }
                }
            }

            return this.PrimaryTitle;
        }

        public List<ProvinceParser> GetAllProvincesReal()
        {
            var provinces = new List<ProvinceParser>();
            var used = new List<CharacterParser>();
            used.Add(this);
            for (var index = 0; index < this.Titles.Count; index++)
            {
                var titleParser = this.Titles[index];
                if (titleParser.Rank == 1 && titleParser.Owns.Count > 0)
                {
                    provinces.Add(titleParser.Owns[0]);
                }

                for (var i = 0; i < titleParser.SubTitles.Values.ToList().Count; i++)
                {
                    var value = titleParser.SubTitles.Values.ToList()[i];
                    if (value.Holder != null && value.Holder != this && value.Liege != null &&
                        this.Titles.Contains(value.Liege) && !this.HasLiegeInChain(value.Holder))
                    {
                        var list = value.Holder.GetAllProvinces(used);

                        provinces.AddRange(list);
                    }
                }
            }

            HashSet<ProvinceParser> p = new HashSet<ProvinceParser>(provinces);

            return p.ToList().Where(a => a.Title.Holder.TopLiegeCharacter == this.TopLiegeCharacter).ToList();
        }

        public List<ProvinceParser> GetAllProvinces()
        {
            return this.Provinces;

            var provinces = new List<ProvinceParser>();
            var used = new List<CharacterParser>();
            used.Add(this);
            for (var index = 0; index < this.Titles.Count; index++)
            {
                var titleParser = this.Titles[index];
                if (titleParser.Rank == 1 && titleParser.Owns.Count > 0)
                {
                    provinces.Add(titleParser.Owns[0]);
                }

                for (var i = 0; i < titleParser.SubTitles.Values.ToList().Count; i++)
                {
                    var value = titleParser.SubTitles.Values.ToList()[i];
                    if (value.Holder != null && value.Holder != this && value.Liege != null &&
                        this.Titles.Contains(value.Liege) && !this.HasLiegeInChain(value.Holder))
                    {
                        var list = value.Holder.GetAllProvinces(used);

                        provinces.AddRange(list);
                    }
                }
            }

            HashSet<ProvinceParser> p = new HashSet<ProvinceParser>(provinces);

            return p.ToList().Where(a => a.Title.Holder.TopLiegeCharacter == this.TopLiegeCharacter).ToList();
        }

        public List<ProvinceParser> GetAllProvinces(List<CharacterParser> used )
        {
            var provinces = new List<ProvinceParser>();
            if (used.Contains(this))
            {
                return provinces;
            }

            used.Add(this);
            for (var index = 0; index < this.Titles.Count; index++)
            {
                var titleParser = this.Titles[index];
                if (titleParser.Rank == 1 && titleParser.Owns.Count > 0)
                {
                    provinces.Add(titleParser.Owns[0]);
                }

                for (var i = 0; i < titleParser.SubTitles.Values.ToList().Count; i++)
                {
                    var value = titleParser.SubTitles.Values.ToList()[i];
                    if (value.Holder != null && value.Holder != this && value.Liege != null &&
                        this.Titles.Contains(value.Liege) && !this.HasLiegeInChain(value.Holder))
                    {
                        var list = value.Holder.GetAllProvinces(used);

                        provinces.AddRange(list);
                    }
                }
            }

            HashSet<ProvinceParser> p = new HashSet<ProvinceParser>(provinces);

            return p.ToList();
        }

        public bool HasLiegeInChain(CharacterParser chr)
        {
            if (this.Liege == chr)
            {
                return true;
            }

            for (var index = 0; index < this.Titles.Count; index++)
            {
                var titleParser = this.Titles[index];
                if (titleParser.Liege != null && titleParser.Liege.Holder == chr)
                {
                    return true;
                }
            }

            if (this.Liege == null)
            {
                return false;
            }

            return this.Liege.HasLiegeInChain(chr);
        }

        public void DoMajorPlayer()
        {
            if (SimulationManager.instance.AllowSimConquer)
            if (this.Behaviour == "conquerer")
            {
                var provinces = this.MainIsland.Provinces;
                int count = provinces.Count;
                List<TitleParser> neighbours = new List<TitleParser>();

                provinces.ForEach(p => p.Adjacent.Where(pp => pp.ProvinceTitle != null && pp.Title.Holder != null && pp.Title.Holder.TopLiegeCharacter != this.TopLiegeCharacter && pp.Title.TopmostTitle.government != "republic").ToList().ForEach(ppp => neighbours.Add(ppp.Title.Holder.TopLiegeCharacter.PrimaryTitle)));


                neighbours = neighbours.Distinct().ToList();
                    for (var index = 0; index < Math.Min(neighbours.Count, 2); index++)

                    if (neighbours.Count > 0)
                    {
                        neighbours = neighbours.OrderBy(a => a.CapitalProvince.DistanceTo(this.PrimaryTitle.CapitalProvince)).ToList();
                        var titleParser = neighbours[0];
                        if (titleParser.TopmostTitle != this.PrimaryTitle.TopmostTitle && titleParser.TopmostTitle.Rank < 4)
                        {
                            StoryManager.instance.CreateEvent(SimulationManager.instance.Year, this.ChrName + " the " + this.PrimaryTitle.LangTitleName + " captures " + titleParser.TopmostTitle.LangName, this.PrimaryTitle);
                            titleParser.TopmostTitle.Log("Given to " + this.ID + " in conquest");
                            this.GiveTitleSoft(titleParser.TopmostTitle);
                        }
                    }

                    provinces = this.Provinces;

                    if (provinces.Count > 180)
                    {
                        // if (Rand.Next(2) == 0)
                        {
                            this.IsMajorPlayer = false;
                            BookmarkManager.instance.AddImportantYear(SimulationManager.instance.Year);
                        }
                    }
                    else
                    {
                        if (provinces.Count == count)
                        {
                            var coast = provinces.Where(p => p.Adjacent.Where(pp => pp.land != true).Any()).ToList();
                            if (!coast.Any())
                            {
                                this.IsMajorPlayer = false;
                                BookmarkManager.instance.AddImportantYear(SimulationManager.instance.Year);
                                return;
                            }

                            var otherCoasts = MapManager.instance.Provinces.Where(
                                p => p.ProvinceTitle != null && !this.Titles.Contains(p.Title.TopmostTitle) && p.Adjacent.Where(pp => !pp.land).Any() && p.Title.TopmostTitle.Rank > 2).ToList();

                            otherCoasts = otherCoasts.OrderBy(o => o.DistanceTo(coast[0])).ToList();

                            if (otherCoasts.Any())
                            {
                                otherCoasts[0].Title.TopmostTitle.Log("Given to " + this.ID + " in conquest");
                                this.GiveTitleSoft(otherCoasts[0].Title.TopmostTitle);
                            }
                        }
                    }
                }
        }

        public int NumberOfProvinces
        {
            get
            {
                int count = 0;
                this.Islands.ForEach(c => count += c.Provinces.Count);
                return count;
            }
        }

        public List<ProvinceParser> Provinces
        {
            get
            {
                if (this.Islands == null)
                {
                    this.GetIslands();
                    if (this.Islands.Count == 0)
                    {
                        this.Islands = null;
                        return new List<ProvinceParser>();
                    }
                }

                List<ProvinceParser> provinces = new List<ProvinceParser>();
                this.Islands.ForEach(c => provinces.AddRange(c.Provinces));
                return provinces;
            }
        }

        public void DoMinorPlayer()
        {
            if (SimulationManager.instance.AllowSimConquer && RandomIntHelper.Next(10)==0)
            {
                List<TitleParser> duchies = new List<TitleParser>();
                 var provinces2 = this.GetAllProvinces();

                provinces2.ForEach(p => p.Adjacent.Where(o => o.ProvinceTitle != null && o.Title.Liege == null && o.Title.Rank == 1 && o.Title.TopmostTitle != this.PrimaryTitle.TopmostTitle).ToList().ForEach(aa=>duchies.Add(aa.Title)));

                if (duchies.Count > 0)
                {
                    var duchy = duchies[RandomIntHelper.Next(duchies.Count)];

                    if (this.PrimaryTitle.Rank > duchy.Rank)
                    {
                        duchy.Log("Captured by" + this.PrimaryTitle);
                        duchy.DoSetLiegeEvent(this.PrimaryTitle);
                    }
                    else
                    {
                        duchy.Log("Captured by" + this.ID + " " + this.PrimaryTitle.Name);
                        duchy.DoSetLiegeEvent(null);
                        duchy.Log("Given to " + this.ID + " after capture along with lower");
                        this.GiveTitleSoftPlusAllLower(duchy, null, true);
                    }

                    if (duchy.LangRealmName == null)
                    {
                        string str = duchy.LangRealmName;
                    }
                }
            }

            if (SimulationManager.instance.AllowSimConquer)
                if (this.Behaviour == "war")
            {
                if (this.PrimaryTitle.Wars.Count == 0)
                {
                    this.IsMinorPlayer = false;
                    return;
                }

                var war = this.PrimaryTitle.Wars[0];

                switch (RandomIntHelper.Next(2))
                {
                    case 0:
                    {
                        // attacker lose a duchy...

                        var provinces =
                            this.GetAllProvinces()
                                .Where(p => p.Adjacent.Any(o => o.ProvinceTitle != null && o.Title.TopmostTitle == war && o.Title.Liege != null && o.Title.Liege.Rank==2));

                            List<TitleParser> duchies = new List<TitleParser>();

                            provinces.ToList().ForEach(p=> duchies.Add(p.Title.Liege));
                                duchies.RemoveAll(a => a == null);
                                duchies.RemoveAll(a => a.Liege == null);
                                if (duchies.Count > 0)
                            {
                                var duchy = duchies[RandomIntHelper.Next(duchies.Count)];

                                if (war.Rank > duchy.Rank)
                                {
                                        duchy.Log("Captured by" + war + " in minor war");
                                        duchy.DoSetLiegeEvent(war);
                                }
                                else
                                {
                                        duchy.Log("Captured by" + war + " in minor war");
                                        duchy.DoSetLiegeEvent(null);
                                        duchy.Log("Given to " + this.ID + " in minor war along with lower");
                                        war.Holder.GiveTitleSoftPlusAllLower(duchy, null, true);
                                }

                                if (duchy.LangRealmName == null)
                                {
                                    string str = duchy.LangRealmName;
                                }

                                if (this.PrimaryTitle == null)
                                {
                                        //StoryManager.instance.CreateEvent(SimulationManager.instance.Year, "the war between " + PrimaryTitle.LangRealmName + " and " + war.LangRealmName + " ends after " + war.LangRealmName + " takes control of " + duchy.LangRealmName, war);
                                    }
                                else
                                {
                                        StoryManager.instance.CreateEvent(SimulationManager.instance.Year, "the war between " + this.PrimaryTitle.LangRealmName + " and " + war.LangRealmName + " ends after " + war.LangRealmName + " takes control of " + duchy.LangRealmName, war);
                                    }
                            }
                        }

                        break;
                    case 1:
                    {
                            // attacker gain a duchy...

                            var provinces =
                                war.GetAllProvinces()
                                    .Where(p => p.Adjacent.Where(o => o.ProvinceTitle != null && o.Title.TopmostTitle == this.PrimaryTitle && o.Title.Liege != null && o.Title.Liege.Rank == 2).Any());

                            List<TitleParser> duchies = new List<TitleParser>();

                            provinces.ToList().Where(p=>p.Title.Liege != null).ToList().ForEach(p => duchies.Add(p.Title.Liege));

                            if (duchies.Count > 0)
                            {
                                var duchy = duchies[RandomIntHelper.Next(duchies.Count)];

                                if (this.PrimaryTitle.Rank > duchy.Rank)
                                {
                                        duchy.Log("Captured by" + this.PrimaryTitle + " in minor war");

                                        duchy.DoSetLiegeEvent(this.PrimaryTitle);
                                }
                                else
                                {
                                    duchy.Log("Captured by" + this.PrimaryTitle + " in minor war");
                                    duchy.DoSetLiegeEvent(null);
                                        duchy.Log("Given to " + this.ID + " in minor war along with lower");
                                        this.GiveTitleSoftPlusAllLower(duchy, null, true);
                                }

                                if (duchy.LangRealmName.Trim() == "")
                                {
                                }

                                StoryManager.instance.CreateEvent(SimulationManager.instance.Year, "the war between " + this.PrimaryTitle.LangRealmName + " and " + war.LangRealmName + " ends after " + this.PrimaryTitle.LangRealmName + " takes control of " + duchy.LangRealmName, war);
                            }
                        }

                        break;
                }

                if (this.PrimaryTitle == null)
                {
                    this.IsMajorPlayer = false;
                    this.IsMinorPlayer = false;
                }

                    if (this.PrimaryTitle != null && this.PrimaryTitle.Wars.Count > 0)
                    {
                        this.PrimaryTitle.Wars.RemoveAt(0);
                    }

                    if (this.PrimaryTitle != null && this.PrimaryTitle.Wars.Count == 0)
                    {
                        this.IsMinorPlayer = false;
                    }
                }
        }

        public enum PrimaryTrait
        {
            diplomacy,
            martial,
            intrigue,
            stewardship,
            learning,
        }

        public void AddRandomTraits()
        {
            PrimaryTrait trait = (PrimaryTrait) RandomIntHelper.Next(5);

            TraitManager.instance.FillCharacter(this, trait);
        }

        public bool CalculateAgeFromScope(int endDate)
        {
            int birthYear = 0;
            int deathYear = 0;
            foreach (var scopeChild in this.Scope.Children)
            {
                if (scopeChild is ScriptScope)
                {
                    var c = scopeChild as ScriptScope;
                    var date = Convert.ToInt32(c.Name.Split('.')[0]);
                    foreach (var cChild in c.Children)
                    {
                        if (cChild is ScriptCommand)
                        {
                            ScriptCommand cc = cChild as ScriptCommand;

                            if (cc.Name == "birth")
                            {
                                birthYear = date;
                            }

                            if (cc.Name == "death")
                            {
                                deathYear = date;
                            }
                        }
                    }
                }
            }


            if (birthYear > endDate)
            {
                return false;
            }

            this.YearOfBirth = birthYear;


            if (deathYear != 0)
            {
                this.YearOfDeath = deathYear;
            }
            else
            {
                this.YearOfDeath = this.YearOfBirth + 10;
            }

            return true;
        }


        public void FixupFromScope(int endDate)
        {
            this.culture = this.Scope.GetString("culture");
            this.religion = this.Scope.GetString("religion");

            int dd = this.Scope.GetInt("dynasty");
            if (dd != 0)
            {
                if (!DynastyManager.instance.DynastyMap.ContainsKey(dd))
                {
                    this.Dynasty = DynastyManager.instance.GetDynasty(this.Culture);
                    this.Dynasty.Name = this.Dynasty.NameScope.Value.ToString();
                }
                    else
                {
                    this.Dynasty = DynastyManager.instance.DynastyMap[dd];
                }
            }
            else
            {
                this.Dynasty = DynastyManager.instance.GetDynasty(this.Culture);
                this.Dynasty.Name = this.Dynasty.NameScope.Value.ToString();
            }

            this.Dynasty.Members.Add(this);
            int m = this.Scope.GetInt("mother");
            if(m != 0 && CharacterManager.instance.CharacterMap.ContainsKey(m))
            {
                this.Mother = CharacterManager.instance.CharacterMap[m];
            }

            int f = this.Scope.GetInt("father");
            if(f != 0 && CharacterManager.instance.CharacterMap.ContainsKey(f))
            {
                this.Father = CharacterManager.instance.CharacterMap[f];
            }

            foreach (var scopeChild in this.Scope.Children)
            {
                if (scopeChild is ScriptScope)
                {
                    var c = scopeChild as ScriptScope;
                    var date = Convert.ToInt32(c.Name.Split('.')[0]);
                    if (date < endDate)
                    {
                        foreach (var cChild in c.Children)
                        {
                            if (cChild is ScriptCommand)
                            {
                                ScriptCommand cc = cChild as ScriptCommand;

                                if (cc.Name == "add_spouse" && CharacterManager.instance.CharacterMap.ContainsKey(cc.GetInt()))
                                {
                                    this.Spouses.Add(CharacterManager.instance.CharacterMap[cc.GetInt()]);
                                }
                        }
                        }
                    }
                }
            }
        }

        public bool HasTitle(string title)
        {
            return this.Titles.Where(t => t.Name == title).Any();
        }

        public class ProvinceIsland
        {
            public List<ProvinceParser> Provinces = new List<ProvinceParser>();

            public void Extract(List<ProvinceParser> provs)
            {
                bool bDone = false;

                while (!bDone)
                {
                    bDone = true;
                    for (var i = 0; i < this.Provinces.Count; i++)
                    {
                        var provinceParser = this.Provinces[i];
                        for (var index = 0; index < provinceParser.Adjacent.Count; index++)
                        {
                            var parser = provinceParser.Adjacent[index];
                            if (provs.Contains(parser))
                            {
                                this.Provinces.Add(parser);
                                provs.Remove(parser);
                                bDone = false;
                            }
                        }
                    }
                }
            }
        }

        public List<ProvinceIsland> GetIslands()
        {
            if (this.PrimaryTitle.Name == "e_spain")
            {
            }

            var provs = this.GetAllProvincesReal();
            List<ProvinceIsland> isles = new List<ProvinceIsland>();
            while (provs.Count > 0)
            {
                ProvinceIsland isle = new ProvinceIsland();
                isles.Add(isle);
                isle.Provinces.Add(provs[0]);
                provs.RemoveAt(0);
                isle.Extract(provs);
            }

            this.Islands = isles;

            this.SeperateIslands = new List<CharacterParser.ProvinceIsland>();
            this.MainIsland = null;
            foreach (var provinceIsland in this.Islands)
            {
                if (!provinceIsland.Provinces.Contains(this.PrimaryTitle.DejureCapitalProvince))
                {
                    this.SeperateIslands.Add(provinceIsland);
                }
                else
                {
                    this.MainIsland = provinceIsland;
                }
            }

            if (this.MainIsland == null)
            {
                this.SeperateIslands.Clear();
                foreach (var provinceIsland in this.Islands)
                {
                    if (!provinceIsland.Provinces.Contains(this.PrimaryTitle.CapitalProvince))
                    {
                        this.SeperateIslands.Add(provinceIsland);
                    }
                    else
                    {
                        this.MainIsland = provinceIsland;
                    }
                }

                if (this.MainIsland == null && this.Islands.Count > 0)
                {
                    this.MainIsland = this.Islands[RandomIntHelper.Next(this.Islands.Count)];
                    this.SeperateIslands.Remove(this.MainIsland);
                }
            }

            return isles;
        }
    }
}