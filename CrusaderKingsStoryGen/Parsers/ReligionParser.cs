// <copyright file="ReligionParser.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Parsers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Xml;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class ReligionParser : Parser
    {
        public ReligionGroupParser Group;

        private ProvinceParser capital;

        public bool Modern { get; set; }

        public HashSet<ProvinceParser> holySites = new HashSet<ProvinceParser>();

        public ReligionParser(ScriptScope scope) : base(scope)
        {
            this.Name = scope.Name;
        }

        private bool dirty = true;
        Rectangle _bounds = new Rectangle();

        public Rectangle Bounds
        {
            get
            {
                if (this.dirty)
                {
                    this._bounds = this.GetBounds();
                }

                this.dirty = false;
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
            var prov = this.Provinces;

            float avx = 0;
            float avy = 0;

            Rectangle tot = Rectangle.Empty;
            foreach (var provinceParser in prov)
            {
                int cx = provinceParser.Bounds.X + (provinceParser.Bounds.Width / 2);
                int cy = provinceParser.Bounds.Y + (provinceParser.Bounds.Height / 2);

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
            this.TextPos = new Point((int)avx, (int)avy);
            return tot;
        }

        public ReligionParser BranchReligion(string name)
        {
            ScriptScope scope = new ScriptScope();
            scope.Name = name;
            this.Group.Scope.Add(scope);
            ReligionParser r = new ReligionParser(scope);
            r.Group = this.Group;
            ReligionManager.instance.AllReligions.Add(r);
            return r;
        }


        public void RandomReligionProperties()
        {
            this.divine_blood = RandomIntHelper.Next(2) == 0;
            this.female_temple_holders = RandomIntHelper.Next(2) == 0;
            this.priests_can_inherit = RandomIntHelper.Next(2) == 0;

            this.matrilineal_marriages = RandomIntHelper.Next(4) != 0;

            bool warLike = RandomIntHelper.Next(4) != 0;
            if (RandomIntHelper.Next(2) == 0)
            {
                this.Resilience = RandomIntHelper.Next(2);
            }
            else
            {
                this.Resilience = RandomIntHelper.Next(5);
            }

            if (warLike)
            {
                this.allow_looting = true;
                this.allow_viking_invasion = true;
                this.can_call_crusade = true;
                if (RandomIntHelper.Next(2) == 0)
                {
                    this.peace_prestige_loss = true;
                }
            }
            else
            {
                if (RandomIntHelper.Next(5) == 0)
                {
                    this.pacifist = true;
                }

                if (RandomIntHelper.Next(2) == 0)
                {
                    this.can_call_crusade = false;
                }
            }

            this.polytheism = RandomIntHelper.Next(2) == 0;
         //   this.polytheism = true;
            if (this.polytheism)
            {
                this.hasLeader = RandomIntHelper.Next(3) != 0;
            }
            else
            {
                this.hasLeader = true;
            }

            this.can_grant_claim = RandomIntHelper.Next(3) != 0;
            this.can_grant_divorce = RandomIntHelper.Next(2) != 0;
            this.can_excommunicate = RandomIntHelper.Next(2) != 0;
            this.can_hold_temples = RandomIntHelper.Next(3) != 0;
            this.can_retire_to_monastery = RandomIntHelper.Next(2) != 0;
            this.can_have_antipopes = RandomIntHelper.Next(2) != 0 && this.hasLeader;
            this.autocephaly = RandomIntHelper.Next(3) == 0;
            this.investiture = RandomIntHelper.Next(2) == 0 && this.hasLeader;
            this.icon = RandomIntHelper.Next(52) + 1;
            this.heresy_icon = RandomIntHelper.Next(52) + 1;
            if (RandomIntHelper.Next(2) == 0)
            {
                this.ai_convert_other_group = 0;
            }
            else
            {
                this.ai_convert_other_group = 2;
            }

            this.has_heir_designation = RandomIntHelper.Next(4) == 0;

            if (RandomIntHelper.Next(2) == 0)
            {
                if (RandomIntHelper.Next(2) == 0)
                {
                    this.max_consorts = 1 + RandomIntHelper.Next(5);
                }
                else
                {
                    {
                        this.max_wives = 2 + RandomIntHelper.Next(4);
                    }
                }
            }


            if (RandomIntHelper.Next(6) == 0)
            {
                this.bs_marriage = !this.bs_marriage;
                if (RandomIntHelper.Next(3) == 0)
                {
                    this.pc_marriage = this.bs_marriage;
                }
            }



            this.religious_clothing_head = RandomIntHelper.Next(4);
            this.religious_clothing_priest = RandomIntHelper.Next(4);
        }

        public void TryFillHolySites()
        {
            if (this.holySites.Count >= 5)
            {
                while (this.holySites.Count > 5)
                {
                    this.holySites.ToArray()[0].Title.Scope.Remove(this.holySites.ToArray()[0].Title.Scope.Find("holy_site"));
                    this.holySites.Remove(this.holySites.ToArray()[0]);
                }

                return;
            }

            while (this.holySites.Count < 5)
            {
                var chosen = this.Provinces[RandomIntHelper.Next(this.Provinces.Count)];
                if (this.holySites.Count == this.Provinces.Count)
                {
                    if (this.holySites.Count < 5)
                    {
                        this.IncompleteReligion = true;
                    }

                    break;
                }

                if (this.holySites.Count < 3)
                    chosen = this.Group.Provinces[this.holySites.Count];
                else
                {
                    chosen.templeRequirement++;
                }

                if (this.holySites.Contains(chosen))
                {
                    continue;
                }

                chosen.Title.Scope.Add(new ScriptCommand("holy_site", this.Name, chosen.Scope));

                this.holySites.Add(chosen);
            }
        }

        public bool IncompleteReligion { get; set; }

        public List<string> gods = new List<string>();
        List<string> evilgods = new List<string>();

        public void CreateRandomReligion(ReligionGroupParser group)
        {
            string culture = "";
            KingdomHelper dna = null;
            if (this.capital == null)
            {
                dna = CulturalDnaManager.instance.GetVanillaCulture((string)null);
            }
            else
            {
                culture = this.capital.Title.Holder.culture;
                dna = CultureManager.instance.CultureMap[culture].dna;
            }

            this.RandomReligionProperties();
      //     Modern = true;
            int r = RandomIntHelper.Next(255);
            int g = RandomIntHelper.Next(255);
            int b = RandomIntHelper.Next(255);
            string god = dna.GetGodName();
            string devil = dna.GetPlaceName();
            string priest = dna.GetPlaceName();
            this.priest = priest;
            this.high_god_name = dna.GetGodName();
            string scripture_name = dna.GetPlaceName();
            string crusade_name = dna.GetPlaceName();

            this.high_god_name = god;

            this.devil = devil;
            this.priest = priest;
            this.scripture_name = scripture_name;
            this.crusade_name = crusade_name;
  if (this.Name == "sanappa")
            {
                int gfdgdf = 0;
            }

            this.DoReligionScope(god, devil, priest, scripture_name, crusade_name, dna, r, g, b);
        }

        public void RedoReligionScope()
        {
            this.DoReligionScope(this.god, this.devil, this.priest, this.scripture_name, this.crusade_name, this.dna, this.r, this.g, this.b, false);
        }

        private void DoReligionScope(string god, string devil, string priest, string scripture_name, string crusade_name,
            KingdomHelper dna, int r, int g, int b, bool bNew = true)
        {
            string safegod = StarHelper.SafeName(god);
            string safedevil = StarHelper.SafeName(devil);
            string safepriest = StarHelper.SafeName(priest);
            string safehigh_god_name = StarHelper.SafeName(this.high_god_name);
            string safescripture_name = StarHelper.SafeName(scripture_name);
            string safecrusade_name = StarHelper.SafeName(crusade_name);
            string desc = "";
            this.god = god;
            this.devil = devil;
            this.scripture_name = scripture_name;
            if (!this.polytheism)
            {
                desc = "All praise the almighty " + this.high_god_name + "!";
            }
            else
            {
                desc = "The Gods smile upon you.";
            }

            LanguageManager.instance.Add(this.Name + "_DESC", desc);

            LanguageManager.instance.Add(safegod, god);
            LanguageManager.instance.Add(safedevil, devil);
            LanguageManager.instance.Add(safepriest, priest);
            LanguageManager.instance.Add(safehigh_god_name, this.high_god_name);
            LanguageManager.instance.Add(safescripture_name, scripture_name);
            LanguageManager.instance.Add(safecrusade_name, crusade_name);

            this.r = r;
            this.g = g;
            this.b = b;

            string gods = "";
            if (bNew)
            {
                for (int n = 0; n < 10; n++)
                {
                    string go = dna.GetGodName();
                    var sg = StarHelper.SafeName(go);
                    LanguageManager.instance.Add(sg, go);

                    this.gods.Add(sg);
                    gods += sg + " ";
                }
            }

            string egods = "";
            if (bNew)
                for (int n = 0; n < 5; n++)
            {
                string go = dna.GetGodName();
                var sg = StarHelper.SafeName(go);
                LanguageManager.instance.Add(sg, go);
                this.evilgods.Add(sg);

                egods += sg + " ";
            }


            if (!this.polytheism)
            {
                gods = safegod;
                egods = safedevil;
            }
            else
            {
                gods = safehigh_god_name + " " + gods;
            }

            this.safecrusade_name = safecrusade_name;
            this.safescripture_name = safescripture_name;
            this.safepriest = safepriest;
            this.safehigh_god_name = safehigh_god_name;
            this.egods = egods;
            if (this.max_wives > 1 && this.max_consorts > 0)
            {
                this.max_consorts = 0;
            }

            if (this.Name == "sanappa")
            {
                int gfdgdf = 0;
            }

            this.ScopeReligionDetails();
        }

        public bool intermarry;

        public void ScopeReligionDetails()
        {
            this.Scope.Clear();
            this.Scope.Do(@"

	            graphical_culture = westerngfx

		            icon = " + this.icon + @"
		            heresy_icon = " + this.heresy_icon + @"
		         
		            ai_convert_other_group = " + this.ai_convert_other_group + @" # always try to convert
	
		            color = { " + (this.r / 255.0f) + " " + (this.g / 255.0f) + " " + (this.b / 255.0f) + @" }
		
		            crusade_name = " + this.safecrusade_name + @"
		            scripture_name = " + this.safescripture_name + @"
		            priest_title = " + this.safepriest + @"
		
		            high_god_name = " + this.safehigh_god_name + @"
		
		            god_names = {
			            " + this.gods.ToSpaceDelimited() + @"
		            }
		
		            evil_god_names = {
			           " + this.evilgods.ToSpaceDelimited() + @"
		            }
		
		            investiture = " + (this.investiture ? "yes" : "no") + @"
		            can_have_antipopes  = " + (this.can_have_antipopes ? "yes" : "no") + @"
		            can_excommunicate  = " + (this.can_excommunicate ? "yes" : "no") + @"
		            can_grant_divorce  = " + (this.can_grant_divorce ? "yes" : "no") + @"
		            can_grant_claim  = " + (this.can_grant_claim ? "yes" : "no") + @"
		            can_call_crusade  = " + (this.can_call_crusade ? "yes" : "no") + @"
		            can_retire_to_monastery  = " + (this.can_retire_to_monastery ? "yes" : "no") + @"
		            priests_can_inherit  = " + (this.priests_can_inherit ? "yes" : "no") + @"
		            can_hold_temples  = " + (this.can_hold_temples ? "yes" : "no") + @"
		            pacifist  = " + (this.pacifist ? "yes" : "no") + @"
		           
                    bs_marriage  = " + (this.bs_marriage ? "yes" : "no") + @"
		            pc_marriage  = " + (this.pc_marriage ? "yes" : "no") + @"
		            psc_marriage  = " + (this.psc_marriage ? "yes" : "no") + @"
		            cousin_marriage  = " + (this.cousin_marriage ? "yes" : "no") + @"
		            matrilineal_marriages  = " + (this.matrilineal_marriages ? "yes" : "no") + @"
		            intermarry  = " + (this.intermarry ? "yes" : "no") + @"
 
		            allow_viking_invasion  = " + (this.allow_viking_invasion ? "yes" : "no") + @"
		            allow_looting  = " + (this.allow_looting ? "yes" : "no") + @"
		            allow_rivermovement  = " + (this.allow_rivermovement ? "yes" : "no") + @"
		            female_temple_holders  = " + (this.female_temple_holders ? "yes" : "no") + @"
		            autocephaly  = " + (this.autocephaly ? "yes" : "no") + @"
		            divine_blood  = " + (this.divine_blood ? "yes" : "no") + @"
		            has_heir_designation  = " + (this.has_heir_designation ? "yes" : "no") + @"
		            peace_prestige_loss  = " + (this.peace_prestige_loss ? "yes" : "no") + @"
		         
		            " + (this.max_consorts > 0 ? ("max_consorts = " + this.max_consorts.ToString()) : "") + @"
		
                    max_wives  = " + this.max_wives + @"
		            uses_decadence = " + (this.uses_decadence ? "yes" : "no") + @"
                    uses_jizya_tax = " + (this.uses_jizya_tax ? "yes" : "no") + @"
          
                    can_grant_invasion_cb = invasion
		            
		            religious_clothing_head = " + this.religious_clothing_head + @"
		            religious_clothing_priest = " + this.religious_clothing_priest + @"
");
        }

        public int icon = 0;
        public int heresy_icon = 0;
        public int ai_convert_other_group = 0;
        public int religious_clothing_head = 0;
        public int religious_clothing_priest = 1;
        public int max_wives = 1;
        public int max_consorts = 0;
        internal bool investiture = true;
        internal bool can_have_antipopes = true;
        internal bool can_excommunicate = true;
        internal bool can_grant_divorce = true;
        internal bool can_grant_claim = true;
        public bool can_call_crusade = true;
        internal bool can_retire_to_monastery = true;
        internal bool priests_can_inherit = false;
        internal bool can_hold_temples = true;
        public bool pacifist = false;
        internal bool bs_marriage = false;
        internal bool pc_marriage = false;
        internal bool psc_marriage = true;
        internal bool cousin_marriage = true;
        internal bool matrilineal_marriages = true;


        public bool allow_viking_invasion = false;
        public bool allow_looting = false;
        internal bool allow_rivermovement = false;
        internal bool female_temple_holders = false;
        public bool autocephaly = false;
        internal bool divine_blood = false;
        internal bool has_heir_designation = false;
        internal bool peace_prestige_loss = false;
        public bool hasLeader = true;

        public string LanguageName { get; set; }

        public int Resilience = 0;
        public bool polytheism = true;
        public string high_god_name;
        private string devil;
        public string priest;
        public string scripture_name;
        public string crusade_name;
        public int r;
        public int g;
        public int b;
        private KingdomHelper dna;
        public bool uses_decadence;
        private bool uses_jizya_tax;


        public void DoLeader(ProvinceParser capital)
        {
                string popeName = StarHelper.SafeName(StarHelper.Generate(10000000));

                LanguageManager.instance.Add(popeName, StarHelper.Generate(10000000));

                this.PopeName = popeName;
                TitleParser title = null;
                bool bNew = false;
                this.ReligiousHeadTitle = null;
                if (this.ReligiousHeadTitle == null)
                {
                    bNew = true;
                    switch (RandomIntHelper.Next(8))
                    {
                        case 5:
                        case 6:
                        case 7:
                        case 0:
                            title = TitleManager.instance.CreateKingScriptScope(capital, this.Name);
                            break;
                        case 1:
                            title = TitleManager.instance.CreateEmpireScriptScope(capital, this.Name);
                            break;
                        case 2:
                        case 3:
                        case 4:
                            title = TitleManager.instance.CreateDukeScriptScope(capital, this.Name);
                            break;
                    }

                    this.ReligiousHeadTitle = title;
                }

                this.ReligiousHeadTitle.Religious = true;
                this.ReligiousHeadTitle.Active = true;
                this.ReligiousHeadTitle.religion = this.Name;
                TitleManager.instance.ReligiousTitles.Add(this.ReligiousHeadTitle);
                //  ch.UpdateCultural();
                if (bNew)
                {
                    var ch = SimulationManager.instance.AddCharacterForTitle(this.ReligiousHeadTitle, true);
                    ch.religion = this.Name;
                    ch.UpdateCultural();
                   /* var liege = ReligiousHeadTitle.CapitalProvince.Title;
                    if (Rand.Next(3) == 0)
                        ch.GiveTitle(ReligiousHeadTitle.CapitalProvince.Title);
                        */
             //       CharacterManager.instance.Characters.Add(ch);
                }

                string religious_names = "";

                for (int n = 0; n < 40; n++)
                {
                    religious_names = CultureManager.instance.CultureMap[this.ReligiousHeadTitle.Holder.culture].dna.GetMaleName() + " ";
                }

                this.ReligiousHeadTitle.Scope.Do(@"

	        title = """ + popeName + @"""
	        foa = ""POPE_FOA""
	        short_name = " + (RandomIntHelper.Next(2) == 0 ? "yes" : "no") + @"
	        location_ruler_title = " + (RandomIntHelper.Next(2) == 0 ? "yes" : "no") + @"
	        landless = " + (bNew ? "yes" : "no") + @"
	        controls_religion = """ + this.Name + @"""
	        religion = """ + this.Name + @"""
	        primary = yes
	        dynasty_title_names = no
	    

");



                LanguageManager.instance.Add(this.ReligiousHeadTitle.Name, this.LanguageName);
        }

        public string PopeName { get; set; }

        public TitleParser ReligiousHeadTitle { get; set; }


        public void MakeChange()
        {
            switch (RandomIntHelper.Next(24))
            {
                case 0:
                {
                    bool warLike = RandomIntHelper.Next(2) == 0;
                    if (RandomIntHelper.Next(2) == 0)
                        {
                            this.Resilience = RandomIntHelper.Next(2);
                        }
                        else
                        {
                            this.Resilience = RandomIntHelper.Next(5);
                        }

                        if (warLike)
                    {
                        this.allow_looting = true;
                        this.allow_viking_invasion = true;
                        this.can_call_crusade = true;
                        if (RandomIntHelper.Next(2) == 0)
                        {
                            this.peace_prestige_loss = true;
                        }
                    }
                    else
                    {
                        if (RandomIntHelper.Next(5) == 0)
                        {
                            this.pacifist = true;
                        }

                        if (RandomIntHelper.Next(2) == 0)
                            {
                                this.can_call_crusade = false;
                            }
                        }
                }

                    break;
                case 1:
                    this.can_grant_claim = RandomIntHelper.Next(3) != 0;
                    break;
                case 2:
                    this.can_grant_divorce = RandomIntHelper.Next(2) != 0;
                    break;
                case 3:
                    this.can_excommunicate = RandomIntHelper.Next(2) != 0;
                    break;
                case 4:
                    this.can_hold_temples = RandomIntHelper.Next(3) != 0;
                    break;
                case 5:
                    this.can_retire_to_monastery = RandomIntHelper.Next(2) != 0;
                    break;
                case 6:
                    this.can_have_antipopes = RandomIntHelper.Next(2) != 0 && this.hasLeader;
                    break;
                case 7:
                    this.investiture = RandomIntHelper.Next(2) == 0 && this.hasLeader;
                    break;
                case 8:
                    if (RandomIntHelper.Next(2) == 0)
                    {
                        this.ai_convert_other_group = 0;
                    }
                    else
                    {
                        this.ai_convert_other_group = 2;
                    }

                    break;
                case 9:

                    this.has_heir_designation = RandomIntHelper.Next(4) == 0;

                    break;
                case 10:

                    if (RandomIntHelper.Next(2) == 0)
                    {
                        if (RandomIntHelper.Next(2) == 0)
                        {
                            this.max_consorts = 1 + RandomIntHelper.Next(5);
                        }
                        else
                        {
                            {
                                this.max_wives = 2 + RandomIntHelper.Next(4);
                            }
                        }
                    }


                    break;
                case 11:

                    if (RandomIntHelper.Next(6) == 0)
                    {
                        this.bs_marriage = !this.bs_marriage;
                        if (RandomIntHelper.Next(3) == 0)
                        {
                            this.pc_marriage = this.bs_marriage;
                        }
                    }


                    break;
                case 12:

                    this.religious_clothing_head = RandomIntHelper.Next(4);
                    this.religious_clothing_priest = RandomIntHelper.Next(4);



                    break;
                case 13:

                    this.high_god_name = this.dna.GetGodName();

                    break;
                case 14:

                    this.devil = this.dna.GetGodName();

                    break;
                case 15:

                    this.scripture_name = this.dna.GetGodName();

                    break;
                case 16:

                    this.crusade_name = this.dna.GetGodName();

                    break;
                case 17:

                    this.priest = this.dna.GetGodName();

                    break;
                case 18:

                    this.matrilineal_marriages = !this.matrilineal_marriages;

                    break;
                case 19:

               this.divine_blood = RandomIntHelper.Next(2) == 0; ;

                    break;
                case 20:

                    this.female_temple_holders = !this.female_temple_holders;

                    break;
                case 21:
                    this.priests_can_inherit = !this.priests_can_inherit;

                    break;
                case 22:
                    this.uses_decadence = !this.uses_decadence;

                    break;
                case 23:
                    this.uses_jizya_tax = !this.uses_jizya_tax;

                    break;
            }
          }

        public void Mutate(ReligionParser rel, CultureParser culture, int nChanges)
        {
            this.dna = culture.dna;
            this.ai_convert_other_group = rel.ai_convert_other_group;

            this.max_consorts = rel.max_consorts;
            this.max_wives = rel.max_wives;
            this.religious_clothing_head = rel.religious_clothing_head;
            this.religious_clothing_priest = rel.religious_clothing_priest;
            this.allow_looting = rel.allow_looting;
            this.allow_rivermovement = rel.allow_rivermovement;
            this.allow_viking_invasion = rel.allow_viking_invasion;
            this.autocephaly = rel.autocephaly;
            this.bs_marriage = rel.bs_marriage;
            this.can_call_crusade = rel.can_call_crusade;
            this.can_excommunicate = rel.can_excommunicate;
            this.can_grant_claim = rel.can_grant_claim;
            this.can_grant_divorce = rel.can_grant_divorce;
            this.can_have_antipopes = rel.can_have_antipopes;
            this.can_hold_temples = rel.can_hold_temples;
            this.can_retire_to_monastery = rel.can_retire_to_monastery;
            this.divine_blood = rel.divine_blood;
            this.female_temple_holders = rel.female_temple_holders;
            this.hasLeader = rel.hasLeader;
            this.has_heir_designation = rel.has_heir_designation;
            this.investiture = rel.investiture;
            this.matrilineal_marriages = rel.matrilineal_marriages;
            this.pacifist = rel.pacifist;
            this.pc_marriage = rel.pc_marriage;
            this.peace_prestige_loss = rel.peace_prestige_loss;
            this.polytheism = rel.polytheism;
            this.priests_can_inherit = rel.priests_can_inherit;
            this.psc_marriage = rel.psc_marriage;
            this.cousin_marriage = rel.cousin_marriage;

            this.high_god_name = rel.high_god_name;
            this.devil = rel.devil;
            this.priest = rel.priest;

            this.gods.AddRange(rel.gods);


            int r = RandomIntHelper.Next(255);
            int g = RandomIntHelper.Next(255);
            int b = RandomIntHelper.Next(255);

            r = rel.r;
            g = rel.g;
            b = rel.b;
            int mul = -1;
            if (RandomIntHelper.Next(2) == 0)
            {
                mul = 1;
            }

            switch (RandomIntHelper.Next(3))
            {
                case 0:
                    r += RandomIntHelper.Next(10, 35) * mul;
                    g += RandomIntHelper.Next(5, 25) * mul;
                    b += RandomIntHelper.Next(2, 15) * mul;

                    break;
                case 1:
                    g += RandomIntHelper.Next(10, 35) * mul;
                    r += RandomIntHelper.Next(5, 25) * mul;
                    b += RandomIntHelper.Next(2, 15) * mul;

                    break;
                case 2:
                    b += RandomIntHelper.Next(10, 35) * mul;
                    g += RandomIntHelper.Next(5, 25) * mul;
                    r += RandomIntHelper.Next(2, 15) * mul;

                    break;
            }

            if (r > 255)
            {
                r = 255;
            }

            if (g > 255)
            {
                g = 255;
            }

            if (b > 255)
            {
                b = 255;
            }

            if (r < 0)
            {
                r = 0;
            }

            if (g < 0)
            {
                g = 0;
            }

            if (b < 0)
            {
                b = 0;
            }

            for (int n = 0; n < nChanges; n++)
            {
                this.MakeChange();
            }

            this.DoReligionScope(this.high_god_name, this.devil, this.priest, this.scripture_name, this.crusade_name, culture.dna, r, g, b);
        }

        public void RemoveProvince(ProvinceParser provinceParser)
        {
            this.Provinces.Remove(provinceParser);

            this.Group.RemoveProvince(provinceParser);
            this.dirty = true;
        }

        public List<ProvinceParser> Provinces = new List<ProvinceParser>();

        public Color color
        {
            get { return Color.FromArgb(255, this.r, this.g, this.b); }

            set { this.r = value.R;
                this.g = value.G;
                this.b = value.B;
            }
        }

        private string god;
        private Point _textPos;
        public string egods;
        public string safecrusade_name;
        public string safescripture_name;
        public string safepriest;
        public string safehigh_god_name;


        public void AddProvince(ProvinceParser provinceParser)
        {
            if (provinceParser.Religion != null)
            {
                provinceParser.Religion.RemoveProvince(provinceParser);
            }

            if (!this.Provinces.Contains(provinceParser))
            {
                this.Provinces.Add(provinceParser);
            }

            this.Group.AddProvince(provinceParser);
            this.dirty = true;
        }

        public void AddProvinces(List<ProvinceParser> instanceSelectedProvinces)
        {
            foreach (var provinceParser in instanceSelectedProvinces)
            {
                if (!this.Provinces.Contains(provinceParser))
                {
                    this.Provinces.Add(provinceParser);
                }

                this.Group.AddProvince(provinceParser);

                provinceParser.Religion = this;
            }
        }

        public void SaveXml(XmlWriter writer)
        {
            this.SavePropertyXml("Name", writer);

            this.SaveFieldXml("god", writer);
            this.SaveFieldXml("allow_looting", writer);
            this.SaveFieldXml("icon", writer);
            this.SaveFieldXml("heresy_icon", writer);
            this.SaveFieldXml("ai_convert_other_group", writer);
            this.SaveFieldXml("religious_clothing_head", writer);
            this.SaveFieldXml("religious_clothing_priest", writer);

            this.SaveFieldXml("max_wives", writer);
            this.SaveFieldXml("max_consorts", writer);
            this.SaveFieldXml("investiture", writer);
            this.SaveFieldXml("can_have_antipopes", writer);
            this.SaveFieldXml("can_excommunicate", writer);
            this.SaveFieldXml("can_grant_divorce", writer);
            this.SaveFieldXml("can_grant_claim", writer);
            this.SaveFieldXml("can_call_crusade", writer);
            this.SaveFieldXml("can_retire_to_monastery", writer);
            this.SaveFieldXml("priests_can_inherit", writer);
            this.SaveFieldXml("can_hold_temples", writer);
            this.SaveFieldXml("pacifist", writer);
            this.SaveFieldXml("bs_marriage", writer);
            this.SaveFieldXml("pc_marriage", writer);
            this.SaveFieldXml("psc_marriage", writer);
            this.SaveFieldXml("cousin_marriage", writer);
            this.SaveFieldXml("matrilineal_marriages", writer);
            this.SaveFieldXml("allow_viking_invasion", writer);
            this.SaveFieldXml("allow_looting", writer);
            this.SaveFieldXml("allow_rivermovement", writer);
            this.SaveFieldXml("female_temple_holders", writer);
            this.SaveFieldXml("autocephaly", writer);
            this.SaveFieldXml("divine_blood", writer);
            this.SaveFieldXml("has_heir_designation", writer);
            this.SaveFieldXml("peace_prestige_loss", writer);
            this.SaveFieldXml("hasLeader", writer);
            this.SaveFieldXml("Resilience", writer);
            this.SaveFieldXml("polytheism", writer);
            this.SaveFieldXml("high_god_name", writer);
            this.SaveFieldXml("devil", writer);
            this.SaveFieldXml("priest", writer);
            this.SaveFieldXml("scripture_name", writer);
            this.SaveFieldXml("crusade_name", writer);
            this.SaveFieldXml("r", writer);
            this.SaveFieldXml("g", writer);
            this.SaveFieldXml("b", writer);
            this.SaveFieldXml("devil", writer);
            this.SaveFieldXml("uses_decadence", writer);
            this.SaveFieldXml("uses_jizya_tax", writer);
        }

        public void CreateSocietyDetails(string culture)
        {
            LanguageManager.instance.Add("secret_" + this.Name + "_society", "Secret " + this.LanguageName);
            LanguageManager.instance.Add("secret_religious_society_" + this.Name, "Secret " + this.LanguageName);
            LanguageManager.instance.Add("Secret_religious_society_" + this.Name, "Secret " + this.LanguageName);

            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_1_male", StarHelper.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_2_male", StarHelper.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_3_male", StarHelper.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_4_male", StarHelper.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_1_female", StarHelper.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_2_female", StarHelper.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_3_female", StarHelper.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_4_female", StarHelper.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_currency", StarHelper.Generate(culture));

            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_desc", "Those who follow the true faith of " + this.LanguageName + " in secret.");

            ScripterTriggerManager.instance.AddTrigger(this);
            SocietyManager.instance.CreateSocietyForReligion(this, culture, null, SocietyManager.Template.the_assassins);
        }
    }
}