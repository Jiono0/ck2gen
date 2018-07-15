using CrusaderKingsStoryGen.Simulation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml;

namespace CrusaderKingsStoryGen
{
    class ReligionParser : Parser
    {
        public ReligionGroupParser Group;
        public ProvinceParser capital;
        public TitleParser ReligiousHeadTitle { get; set; }

        public string LanguageName { get; set; }

        public string PopeName { get; set; }

        public List<String> gods = new List<string>();
        public List<String> evilgods = new List<string>();

        public string high_god_name;
        public string devil;
        public string priest;
        public string scripture_name;
        public string crusade_name;
        public string god;
        public string egods;
        public string safecrusade_name;
        public string safescripture_name;
        public string safepriest;
        public string safehigh_god_name;

        public int Resilience = 0;
        public int icon = 0;
        public int heresy_icon = 0;
        public int ai_convert_other_group = 0;
        public int religious_clothing_head = 0;
        public int religious_clothing_priest = 0;
        public int max_wives = 1;
        public int max_consorts = 3;

        public bool investiture = false;
        public bool can_have_antipopes = false;
        public bool can_excommunicate = false;
        public bool can_grant_divorce = false;
        public bool can_grant_claim = false;
        public bool can_call_crusade = false;
        public bool can_retire_to_monastery = false;
        public bool priests_can_inherit = true;
        public bool can_hold_temples = true;
        public bool pacifist = false;
        public bool bs_marriage = false;
        public bool pc_marriage = false;
        public bool psc_marriage = false;
        public bool cousin_marriage = false;
        public bool matrilineal_marriages = true;
        public bool allow_viking_invasion = false;
        public bool allow_looting = true;
        public bool allow_rivermovement = true;
        public bool female_temple_holders = true;
        public bool autocephaly = false;
        public bool divine_blood = false;
        public bool has_heir_designation = false;
        public bool peace_prestige_loss = false;
        public bool hasLeader = false;
        public bool polytheism = true;
        public bool uses_decadence = false;
        public bool uses_jizya_tax = false;
        public bool intermarry = true;

        private CulturalDna dna;

        public int r;
        public int g;
        public int b;

        public Color Color
        {
            get { return Color.FromArgb(255, r, g, b); }
            set
            {
                r = value.R;
                g = value.G;
                b = value.B;
            }
        }

        public HashSet<ProvinceParser> holySites = new HashSet<ProvinceParser>();

        public ReligionParser(ScriptScope scope) : base(scope)
        {
            Name = scope.Name;
        }

        public ReligionParser BranchReligion(String name)
        {
            ScriptScope scope = new ScriptScope();
            scope.Name = name;
            Group.Scope.Add(scope);

            ReligionParser r = new ReligionParser(scope);
            r.Group = Group;

            ReligionManager.instance.AllReligions.Add(r);

            return r;
        }

        public void RandomReligionProperties()
        {
            divine_blood = Rand.Next(2) == 0;
            female_temple_holders = Rand.Next(2) == 0;
            priests_can_inherit = Rand.Next(2) == 0;
            
            matrilineal_marriages = Rand.Next(4) != 0;
            
            bool warLike = Rand.Next(4) != 0;

            if(Rand.Next(2)==0)
                Resilience = Rand.Next(2);
            else
                Resilience = Rand.Next(5);

            if (warLike)
            {
                allow_looting = true;
                allow_viking_invasion = true;
                can_call_crusade = true;

                if (Rand.Next(2) == 0)
                {
                    peace_prestige_loss = true;
                }
            }

            else
            {
                if (Rand.Next(5) == 0)
                {
                    pacifist = true;
                }

                if (Rand.Next(2) == 0)
                {
                    can_call_crusade = false;
                }
                    
            }

            this.polytheism = Rand.Next(2) == 0;

            if (polytheism)
                hasLeader = Rand.Next(3) != 0;
            else
                hasLeader = true;

            can_grant_claim = Rand.Next(3) != 0;
            can_grant_divorce = Rand.Next(2) != 0;
            can_excommunicate = Rand.Next(2) != 0;
            can_hold_temples = Rand.Next(3) != 0;
            can_retire_to_monastery = Rand.Next(2) != 0;
            can_have_antipopes = Rand.Next(2) != 0 && hasLeader;
            autocephaly = Rand.Next(3) == 0;
            investiture = Rand.Next(2) == 0 && hasLeader;
            icon = Rand.Next(52) + 1;
            heresy_icon = Rand.Next(52) + 1;

            if (Rand.Next(2) == 0)
                ai_convert_other_group = 0;
            else
                ai_convert_other_group = 2;

            has_heir_designation = Rand.Next(4) == 0;

            if (Rand.Next(2) == 0)
            {
                if (Rand.Next(2) == 0)
                {
                    max_consorts = 1 + Rand.Next(5);

                }

                else
                {
                    {
                        max_wives = 2 + Rand.Next(4);
                    }
                }
            }
          

            if (Rand.Next(6) == 0)
            {
                bs_marriage = !bs_marriage;

                if (Rand.Next(3) == 0)

                    pc_marriage = bs_marriage;
            }

            religious_clothing_head = Rand.Next(4);
            religious_clothing_priest = Rand.Next(4);
        }

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
                var chosen = Provinces[Rand.Next(Provinces.Count)];

                if (holySites.Count == Provinces.Count)
                {
                    break;
                }

                if (holySites.Count < 3)
                    chosen = Group.Provinces[holySites.Count];
                else
                {
                    chosen.templeRequirement++;
                }

                if (holySites.Contains(chosen))
                    continue;

                chosen.Title.Scope.Add(new ScriptCommand("holy_site", Name, chosen.Scope));
                
                holySites.Add(chosen);
            }
        }

        public void CreateNewReligion()
        {
            String culture = "";

            CulturalDna dna = null;

            if (capital == null)
            {
                dna = CulturalDnaManager.instance.GetVanillaCulture((string) null);
            }

            else
            {
                culture = capital.Title.Holder.culture;

                dna = CultureManager.instance.CultureMap[culture].dna;
            }

            //RandomReligionProperties();

            int r = Rand.Next(255);
            int g = Rand.Next(255);
            int b = Rand.Next(255);

            string god = dna.GetGodName();
            string devil = dna.GetPlaceName();
            string priest = dna.GetPlaceName();
            string scripture_name = dna.GetPlaceName();
            string crusade_name = dna.GetPlaceName();

            high_god_name = god;
            
            this.devil = devil;
            this.priest = priest;
            this.scripture_name = scripture_name;
            this.crusade_name = crusade_name;

            DoReligionScope(god, devil, priest, scripture_name, crusade_name, dna, r, g, b);
        }

        public void RedoReligionScope()
        {
            DoReligionScope(god, devil, priest, scripture_name, crusade_name, dna, r, g, b, false);
        }

        private void DoReligionScope(string god, string devil, string priest, string scripture_name, string crusade_name, CulturalDna dna, int r, int g, int b, bool bNew = true)
        {
            string safegod = StarNames.SafeName(god);
            string safedevil = StarNames.SafeName(devil);
            string safepriest = StarNames.SafeName(priest);
            string safehigh_god_name = StarNames.SafeName(high_god_name);
            string safescripture_name = StarNames.SafeName(scripture_name);
            string safecrusade_name = StarNames.SafeName(crusade_name);

            String desc = "";

            this.god = god;
            this.devil = devil;
            this.scripture_name = scripture_name;

            if (!polytheism)
                desc = "All praise the almighty " + high_god_name + "!";
            else
                desc = "The Gods smile upon you.";

            LanguageManager.instance.Add(Name + "_DESC", desc);
            LanguageManager.instance.Add(safegod, god);
            LanguageManager.instance.Add(safedevil, devil);
            LanguageManager.instance.Add(safepriest, priest);
            LanguageManager.instance.Add(safehigh_god_name, high_god_name);
            LanguageManager.instance.Add(safescripture_name, scripture_name);
            LanguageManager.instance.Add(safecrusade_name, crusade_name);

            this.r = r;
            this.g = g;
            this.b = b;

            String gods = "";

            if(bNew)
            {
                for (int n = 0; n < 10; n++)
                {
                    string go = dna.GetGodName();
                    var sg = StarNames.SafeName(go);
                    LanguageManager.instance.Add(sg, go);

                    this.gods.Add(sg);
                    gods += sg + " ";
                }
            }

            String egods = "";

            if (bNew)
                for (int n = 0; n < 5; n++)
            {
                string go = dna.GetGodName();
                var sg = StarNames.SafeName(go);
                LanguageManager.instance.Add(sg, go);
                evilgods.Add(sg);

                egods += sg + " ";
            }

            if (!polytheism)
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

            if (max_wives > 1 && max_consorts > 0)
                max_consorts = 0;

            ScopeReligionDetails();
        }
  
        public void ScopeReligionDetails()
        {
            Scope.Clear();
            Scope.Do(@"

	            graphical_culture = westerngfx

		            icon = " + icon + @"
		            heresy_icon = " + heresy_icon + @"
		         
		            ai_convert_other_group = " + ai_convert_other_group + @" # always try to convert
	
		            color = { " + (r/255.0f) + " " + (g/255.0f) + " " + (b/255.0f) + @" }
		
		            crusade_name = " + safecrusade_name + @"
		            scripture_name = " + safescripture_name + @"
		            priest_title = " + safepriest + @"
		
		            high_god_name = " + safehigh_god_name + @"
		
		            god_names = {
			            " + gods.ToSpaceDelimited() + @"
		            }
		
		            evil_god_names = {
			           " + evilgods.ToSpaceDelimited() + @"
		            }
		
		            investiture = " + (investiture ? "yes" : "no") + @"
		            can_have_antipopes  = " + (can_have_antipopes ? "yes" : "no") + @"
		            can_excommunicate  = " + (can_excommunicate ? "yes" : "no") + @"
		            can_grant_divorce  = " + (can_grant_divorce ? "yes" : "no") + @"
		            can_grant_claim  = " + (can_grant_claim ? "yes" : "no") + @"
		            can_call_crusade  = " + (can_call_crusade ? "yes" : "no") + @"
		            can_retire_to_monastery  = " + (can_retire_to_monastery ? "yes" : "no") + @"
		            priests_can_inherit  = " + (priests_can_inherit ? "yes" : "no") + @"
		            can_hold_temples  = " + (can_hold_temples ? "yes" : "no") + @"
		            pacifist  = " + (pacifist ? "yes" : "no") + @"  
                    bs_marriage  = " + (bs_marriage ? "yes" : "no") + @"
		            pc_marriage  = " + (pc_marriage ? "yes" : "no") + @"
		            psc_marriage  = " + (psc_marriage ? "yes" : "no") + @"
		            cousin_marriage  = " + (cousin_marriage ? "yes" : "no") + @"
		            matrilineal_marriages  = " + (matrilineal_marriages ? "yes" : "no") + @"
		            intermarry  = " + (intermarry ? "yes" : "no") + @"
		            allow_viking_invasion  = " + (allow_viking_invasion ? "yes" : "no") + @"
		            allow_looting  = " + (allow_looting ? "yes" : "no") + @"
		            allow_rivermovement  = " + (allow_rivermovement ? "yes" : "no") + @"
		            female_temple_holders  = " + (female_temple_holders ? "yes" : "no") + @"
		            autocephaly  = " + (autocephaly ? "yes" : "no") + @"
		            divine_blood  = " + (divine_blood ? "yes" : "no") + @"
		            has_heir_designation  = " + (has_heir_designation ? "yes" : "no") + @"
		            peace_prestige_loss  = " + (peace_prestige_loss ? "yes" : "no") + @"	         
		            " + (max_consorts > 0 ? ("max_consorts = " + max_consorts.ToString()) : "") + @"
                    max_wives  = " + max_wives + @"
		            uses_decadence = " + (uses_decadence ? "yes" : "no") + @"
                    uses_jizya_tax = " + (uses_jizya_tax ? "yes" : "no") + @"
                    can_grant_invasion_cb = invasion           
		            religious_clothing_head = " + religious_clothing_head + @"
		            religious_clothing_priest = " + religious_clothing_priest + @"
");
        }

        public void DoLeader(ProvinceParser capital)
        {
                String popeName = StarNames.SafeName(StarNames.Generate(10000000));

                LanguageManager.instance.Add(popeName, StarNames.Generate(10000000));

                PopeName = popeName;
                TitleParser title = null;
                bool bNew = false;
                ReligiousHeadTitle = null;

                if (ReligiousHeadTitle == null)
                {
                    bNew = true;

                    switch (Rand.Next(8))
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            title = TitleManager.instance.CreateKingScriptScope(capital, Name);
                            break;
                        case 4:
                            title = TitleManager.instance.CreateEmpireScriptScope(capital, Name);
                            break;
                        case 5:
                        case 6:
                        case 7:
                            title = TitleManager.instance.CreateDukeScriptScope(capital, Name);
                            break;
                    }

                    ReligiousHeadTitle = title;
                }

                ReligiousHeadTitle.Religious = true;
                ReligiousHeadTitle.Active = true;
                ReligiousHeadTitle.religion = Name;

                TitleManager.instance.ReligiousTitles.Add(ReligiousHeadTitle);

                if (bNew)
                {
                    var ch = SimulationManager.instance.AddCharacterForTitle(ReligiousHeadTitle, true);
                    ch.religion = Name;
                    ch.UpdateCultural();

                }

                String religious_names = "";

                for (int n = 0; n < 40; n++)
                {
                    religious_names = CultureManager.instance.CultureMap[ReligiousHeadTitle.Holder.culture].dna.GetMaleName() + " ";
                }

                ReligiousHeadTitle.Scope.Do(@"

	        title = """ + popeName + @"""
	        foa = ""POPE_FOA""
	        short_name = " + (Rand.Next(2) == 0 ? "yes" : "no") + @"
	        location_ruler_title = " + (Rand.Next(2) == 0 ? "yes" : "no") + @"
	        landless = " + (bNew ? "yes" : "no") + @"
	        controls_religion = """ + Name + @"""
	        religion = """ + Name + @"""
	        primary = yes
	        dynasty_title_names = no
	    

");

            LanguageManager.instance.Add(ReligiousHeadTitle.Name, this.LanguageName);
        }

        public void MakeChange()
        {
            switch (Rand.Next(24))
            {
                case 0:
                {
                    bool warLike = Rand.Next(2) == 0;

                    if (Rand.Next(2) == 0)
                        Resilience = Rand.Next(2);
                    else
                        Resilience = Rand.Next(5);

                    if (warLike)
                    {
                        allow_looting = true;
                        allow_viking_invasion = true;
                        can_call_crusade = true;

                        if (Rand.Next(2) == 0)
                        {
                            peace_prestige_loss = true;
                        }
                    }

                    else
                    {
                        if (Rand.Next(5) == 0)
                        {
                            pacifist = true;
                        }

                        if (Rand.Next(2) == 0)
                            can_call_crusade = false;
                    }
                }
                    break;
                case 1:
                    can_grant_claim = Rand.Next(3) != 0;
                    break;
                case 2:
                    can_grant_divorce = Rand.Next(2) != 0;
                    break;
                case 3:
                    can_excommunicate = Rand.Next(2) != 0;
                    break;
                case 4:
                    can_hold_temples = Rand.Next(3) != 0;
                    break;
                case 5:
                    can_retire_to_monastery = Rand.Next(2) != 0;
                    break;
                case 6:
                    can_have_antipopes = Rand.Next(2) != 0 && hasLeader;
                    break;
                case 7:
                    investiture = Rand.Next(2) == 0 && hasLeader;
                    break;
                case 8:
                    if (Rand.Next(2) == 0)
                        ai_convert_other_group = 0;
                    else
                        ai_convert_other_group = 2;
                    break;
                case 9:

                    has_heir_designation = Rand.Next(4) == 0;

                    break;
                case 10:

                    if (Rand.Next(2) == 0)
                    {
                        if (Rand.Next(2) == 0)
                        {
                            max_consorts = 1 + Rand.Next(5);
                        }
                        else
                        {
                            {
                                max_wives = 2 + Rand.Next(4);
                            }
                        }
                    }
                    break;

                case 11:

                    if (Rand.Next(6) == 0)
                    {
                        bs_marriage = !bs_marriage;
                        if (Rand.Next(3) == 0)
                            pc_marriage = bs_marriage;
                    }
                    break;

                case 12:
                    religious_clothing_head = Rand.Next(4);
                    religious_clothing_priest = Rand.Next(4);
                    break;
                case 13:
                    high_god_name = dna.GetGodName();
                    break;
                case 14:
                    devil = dna.GetGodName();
                    break;
                case 15:
                    scripture_name = dna.GetGodName();
                    break;
                case 16:
                    crusade_name = dna.GetGodName();
                    break;
                case 17:
                    priest = dna.GetGodName();
                    break;
                case 18:
                    matrilineal_marriages = !matrilineal_marriages;
                    break;
                case 19:
                    divine_blood = Rand.Next(2) == 0; ;
                    break;
                case 20:
                    female_temple_holders = !female_temple_holders;
                    break;
                case 21:
                    priests_can_inherit = !priests_can_inherit;
                    break;
                case 22:
                    uses_decadence = !uses_decadence;
                    break;
                case 23:
                    uses_jizya_tax = !uses_jizya_tax;
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

            high_god_name = rel.high_god_name;
            devil = rel.devil;
            priest = rel.priest;

            gods.AddRange(rel.gods);

            int r = Rand.Next(255);
            int g = Rand.Next(255);
            int b = Rand.Next(255);

            r = rel.r;
            g = rel.g;
            b = rel.b;
            int mul = -1;

            if (Rand.Next(2) == 0)
                mul = 1;

            switch (Rand.Next(3))
            {
                case 0:
                    r += Rand.Next(10, 35) * mul;
                    g += Rand.Next(5, 25) * mul;
                    b += Rand.Next(2, 15) * mul;

                    break;
                case 1:
                    g += Rand.Next(10, 35) * mul;
                    r += Rand.Next(5, 25) * mul;
                    b += Rand.Next(2, 15) * mul;

                    break;
                case 2:
                    b += Rand.Next(10, 35) * mul;
                    g += Rand.Next(5, 25) * mul;
                    r += Rand.Next(2, 15) * mul;

                    break;
            }

            if (r > 255)
                r = 255;
            if (g > 255)
                g = 255;
            if (b > 255)
                b = 255;

            if (r < 0)
                r = 0;
            if (g < 0)
                g = 0;
            if (b < 0)
                b = 0;

            for(int n = 0;n < nChanges; n++)

                MakeChange();
  
            DoReligionScope(high_god_name, devil, priest, scripture_name, crusade_name, culture.dna, r, g, b);

        }

        // province / drawing stuff ... why is this in here?

        public List<ProvinceParser> Provinces = new List<ProvinceParser>();

        private Point _textPos;

        Rectangle _bounds = new Rectangle();

        private bool dirty = true;

        public void AddProvince(ProvinceParser provinceParser)
        {

            if (provinceParser.Religion != null)
            {
                provinceParser.Religion.RemoveProvince(provinceParser);
            }

            if (!Provinces.Contains(provinceParser))
                Provinces.Add(provinceParser);

            Group.AddProvince(provinceParser);
            dirty = true;

        }

        public void AddProvinces(List<ProvinceParser> instanceSelectedProvinces)
        {
            foreach (var provinceParser in instanceSelectedProvinces)
            {
                if (!Provinces.Contains(provinceParser))
                    Provinces.Add(provinceParser);

                Group.AddProvince(provinceParser);

                provinceParser.Religion = this;
            }
        }

        public void RemoveProvince(ProvinceParser provinceParser)
        {
            Provinces.Remove(provinceParser);

            Group.RemoveProvince(provinceParser);
            dirty = true;

        }

        public void SaveXml(XmlWriter writer)
        {
            SavePropertyXml("Name", writer);

            SaveFieldXml("god", writer);
            SaveFieldXml("allow_looting", writer);
            SaveFieldXml("icon", writer);
            SaveFieldXml("heresy_icon", writer);
            SaveFieldXml("ai_convert_other_group", writer);
            SaveFieldXml("religious_clothing_head", writer);
            SaveFieldXml("religious_clothing_priest", writer);

            SaveFieldXml("max_wives", writer);
            SaveFieldXml("max_consorts", writer);
            SaveFieldXml("investiture", writer);
            SaveFieldXml("can_have_antipopes", writer);
            SaveFieldXml("can_excommunicate", writer);
            SaveFieldXml("can_grant_divorce", writer);
            SaveFieldXml("can_grant_claim", writer);
            SaveFieldXml("can_call_crusade", writer);
            SaveFieldXml("can_retire_to_monastery", writer);
            SaveFieldXml("priests_can_inherit", writer);
            SaveFieldXml("can_hold_temples", writer);
            SaveFieldXml("pacifist", writer);
            SaveFieldXml("bs_marriage", writer);
            SaveFieldXml("pc_marriage", writer);
            SaveFieldXml("psc_marriage", writer);
            SaveFieldXml("cousin_marriage", writer);
            SaveFieldXml("matrilineal_marriages", writer);
            SaveFieldXml("allow_viking_invasion", writer);
            SaveFieldXml("allow_looting", writer);
            SaveFieldXml("allow_rivermovement", writer);
            SaveFieldXml("female_temple_holders", writer);
            SaveFieldXml("autocephaly", writer);
            SaveFieldXml("divine_blood", writer);
            SaveFieldXml("has_heir_designation", writer);
            SaveFieldXml("peace_prestige_loss", writer);
            SaveFieldXml("hasLeader", writer);
            SaveFieldXml("Resilience", writer);
            SaveFieldXml("polytheism", writer);
            SaveFieldXml("high_god_name", writer);
            SaveFieldXml("devil", writer);
            SaveFieldXml("priest", writer);
            SaveFieldXml("scripture_name", writer);
            SaveFieldXml("crusade_name", writer);
            SaveFieldXml("r", writer);
            SaveFieldXml("g", writer);
            SaveFieldXml("b", writer);
            SaveFieldXml("devil", writer);
            SaveFieldXml("uses_decadence", writer);
            SaveFieldXml("uses_jizya_tax", writer);


        }

        public void CreateSocietyDetails(string culture)
        {

            LanguageManager.instance.Add("secret_" + this.Name + "_society", "Secret " + LanguageName);
            LanguageManager.instance.Add("secret_religious_society_" + this.Name, "Secret " + LanguageName);
            LanguageManager.instance.Add("Secret_religious_society_" + this.Name, "Secret " + LanguageName);

            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_1_male", StarNames.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_2_male", StarNames.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_3_male", StarNames.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_4_male", StarNames.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_1_female", StarNames.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_2_female", StarNames.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_3_female", StarNames.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_rank_4_female", StarNames.Generate(culture));
            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_currency", StarNames.Generate(culture));

            LanguageManager.instance.Add("secret_religious_society_" + this.Name + "_desc", "Those who follow the true faith of " + LanguageName + " in secret.");

            ScripterTriggerManager.instance.AddTrigger(this);
            SocietyManager.instance.CreateSocietyForReligion(this, culture, null, SocietyManager.Template.the_assassins);
        }

        public Rectangle Bounds
        {
            get
            {
                if (dirty)
                    _bounds = GetBounds();
                dirty = false;
                return _bounds;
            }
        }

        public Point TextPos
        {
            get
            {
                if (dirty)
                    _bounds = GetBounds();
                dirty = false;

                return _textPos;
            }
            set { _textPos = value; }
        }

        public Rectangle GetBounds()
        {
            var prov = Provinces;

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
            TextPos = new Point((int)avx, (int)avy);
            return tot;
        }

        public override ScriptScope CreateScope()
        {
            return null;
        }

    }
}