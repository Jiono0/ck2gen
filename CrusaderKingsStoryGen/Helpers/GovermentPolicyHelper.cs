// <copyright file="Government.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using System.Collections.Generic;
    using System.Drawing;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class GovermentPolicyHelper
    {
        public string type = "nomadic";
        public List<string> preferred_holdings = new List<string>() {"NOMAD"};

        public List<string> builds_with_prestige = new List<string>();
        public List<string> builds_with_piety = new List<string>();
        public List<string> allowed_holdings = new List<string>() { "NOMAD"};
        public List<string> allowed_holdings_culture = new List<string>();
        public List<string> allowed_holdings_religion = new List<string>();
        public List<string> allowed_holdings_culture_and_religion = new List<string>();
        public List<string> accepts_liege_governments = new List<string>();
        public List<string> free_revoke_on_governments_religion = new List<string>();

        public bool allowReligionHead = false;

        public List<string> cultureGroupAllow = new List<string>();
        public List<string> cultureAllow = new List<string>();
        public bool is_patrician = false;

        public Color color;


        public List<string> ignore_in_vassal_limit_calculation = new List<string>();
        public bool allows_matrilineal_marriage = true;

        public string title_prefix = null;
        public string frame_suffix = null;
        public bool merchant_republic = false;
        public bool uses_decadence = false;
        public bool uses_jizya_tax = false;
        public bool uses_piety_for_law_change = false;
        public bool uses_prestige_for_law_change = false;
        public bool allow_title_revokation = true;
        public bool allow_looting = true;
        public bool can_imprison_without_reason = true;
        public bool can_revoke_without_reason = true;
        public bool ignores_de_jure_laws = false;
        public bool dukes_called_kings = true;
        public bool barons_need_dynasty = false;
        public bool can_create_kingdoms = true;
        public bool can_usurp_kingdoms_and_empires = true;
        public bool have_gender_laws = true;
        public bool can_build_holdings = true;
        public bool can_build_forts = false;
        public bool can_build_castle = false;
        public bool can_build_city = false;
        public bool can_build_temple = true;
        public bool can_build_tribal = true;
        public bool can_grant_kingdoms_and_empires_to_other_government = true;
        public bool can_be_granted_kingdoms_and_empires_by_other_government = true;
        public bool free_retract_vassalage = true;
        public int max_consorts = 3 ;

        public double aggression = 4;
        public string name;

          private bool can_change_to_nomad_on_start = false;
        private bool can_build_hospitals = true;

        private string GetWordList(List<string> strs)
        {
            GovernmentManager.instance.done.Clear();
            string re = "";
            foreach (var str in strs)
            {
                if (!GovernmentManager.instance.done.Contains(str))
                {
                    re += str + " ";
                }

                GovernmentManager.instance.done.Add(str);
            }

            return re;
        }

        public void Save(ScriptScope scriptScope)
        {
            string cultureBlock = "";

            foreach (var cultureParser in this.cultureGroupAllow)
            {
                cultureBlock += "culture_group = " + cultureParser + @"
                ";
            }

            if (this.cultureGroupAllow.Count == 0)
            {
                return;
            }

            int r = RandomIntHelper.Next(255);
            int g = RandomIntHelper.Next(255);
            int b = RandomIntHelper.Next(255);

            scriptScope.Do(@"
                color = { " + this.color.R + " " + this.color.G + " " + this.color.B + @" }
                allow_looting = " + (this.allow_looting ? "yes" : "no") + @"
                allow_title_revokation = " + (this.allow_title_revokation ? "yes" : "no") + @"
                allows_matrilineal_marriage = " + (this.allows_matrilineal_marriage ? "yes" : "no") + @"
                barons_need_dynasty = " + (this.barons_need_dynasty ? "yes" : "no") + @"
                can_be_granted_kingdoms_and_empires_by_other_government = " + (this.can_be_granted_kingdoms_and_empires_by_other_government ? "yes" : "no") + @"
                can_build_castle = " + (this.can_build_castle ? "yes" : "no") + @"
                can_build_city = " + (this.can_build_city ? "yes" : "no") + @"
                can_build_forts = " + (this.can_build_forts ? "yes" : "no") + @"
                can_build_holdings = " + (this.can_build_holdings ? "yes" : "no") + @"
                can_build_temple = " + (this.can_build_temple ? "yes" : "no") + @"
                can_build_tribal = " + (this.can_build_tribal ? "yes" : "no") + @"
                can_build_hospitals = " + (this.can_build_hospitals ? "yes" : "no") + @"
                can_create_kingdoms = " + (this.can_create_kingdoms ? "yes" : "no") + @"
                can_grant_kingdoms_and_empires_to_other_government = " + (this.can_grant_kingdoms_and_empires_to_other_government ? "yes" : "no") + @"
                can_imprison_without_reason = " + (this.can_imprison_without_reason ? "yes" : "no") + @"
                can_revoke_without_reason = " + (this.can_revoke_without_reason ? "yes" : "no") + @"
                can_usurp_kingdoms_and_empires = " + (this.can_usurp_kingdoms_and_empires ? "yes" : "no") + @"
                dukes_called_kings = " + (this.dukes_called_kings ? "yes" : "no") + @"
                free_retract_vassalage = " + (this.free_retract_vassalage ? "yes" : "no") + @"
                ignores_de_jure_laws = " + (this.ignores_de_jure_laws ? "yes" : "no") + @"
                can_change_to_nomad_on_start = " + (this.can_change_to_nomad_on_start ? "yes" : "no") + @"
                " + (this.frame_suffix != null ? @"
			        frame_suffix = " + this.frame_suffix : "") + @"
                " + (this.title_prefix != null ? @"
			        title_prefix = " + this.title_prefix : "") + @"
            
                merchant_republic = " + (this.type=="republic" ? "yes" : "no") + @"
                uses_decadence = " + (this.uses_decadence ? "yes" : "no") + @"
                uses_piety_for_law_change = " + (this.uses_piety_for_law_change ? "yes" : "no") + @"
                uses_prestige_for_law_change = " + (this.uses_prestige_for_law_change ? "yes" : "no") + @"
                " + (this.preferred_holdings.Count > 0 ? @"
                preferred_holdings = { 
                    " + this.GetWordList(this.preferred_holdings) + @" 
                }" : "" ) + @"
                " + (this.allowed_holdings.Count > 0 ? @"
                allowed_holdings = { 
                    " + this.GetWordList(this.allowed_holdings) + @" 
                }" : "") + @"
                " + (this.allowed_holdings_culture.Count > 0 ? @"
                allowed_holdings_culture = { 
                    " + this.GetWordList(this.allowed_holdings_culture) + @" 
                }" : "") + @"
                " + (this.allowed_holdings_culture_and_religion.Count > 0 ? @"
                allowed_holdings_culture_and_religion = { 
                    " + this.GetWordList(this.allowed_holdings_culture_and_religion) + @" 
                }" : "") + @"
                " + (this.builds_with_prestige.Count > 0 ? @"
                builds_with_prestige = { 
                    " + this.GetWordList(this.builds_with_prestige) + @" 
                }" : "") + @"
                " + (this.builds_with_piety.Count > 0 ? @"
                builds_with_piety = { 
                    " + this.GetWordList(this.builds_with_piety) + @" 
                }" : "") + @"
             
                " + (this.accepts_liege_governments.Count > 0 ? @"
                accepts_liege_governments = { 
                    " + this.GetWordList(this.accepts_liege_governments) + @" 
                }" : "") + @"
    
                potential = {
                    " + (this.cultureGroupAllow.Count > 1 ? @"
			        OR = {
                        " + cultureBlock + @"
                    } " : cultureBlock ) + @"
			        is_patrician = " + (this.type == "republic" ? "yes" : "no") + @"
		            mercenary = no
			        holy_order = no
                }
                ");
        }

        public GovermentPolicyHelper Mutate(int numChanges)
        {
            GovermentPolicyHelper g = new GovermentPolicyHelper();

            g.frame_suffix = this.frame_suffix;
            g.title_prefix = this.title_prefix;
            g.aggression = this.aggression;
            g.allowReligionHead = this.allowReligionHead;
            g.allow_looting = this.allow_looting;
            g.allow_title_revokation = this.allow_title_revokation;
            g.allows_matrilineal_marriage = this.allows_matrilineal_marriage;
            g.barons_need_dynasty = this.barons_need_dynasty;
            g.can_be_granted_kingdoms_and_empires_by_other_government = this.can_be_granted_kingdoms_and_empires_by_other_government;
            g.can_build_castle = this.can_build_castle;
            g.can_build_city = this.can_build_city;
            g.can_build_forts = this.can_build_forts;
            g.can_build_holdings = this.can_build_holdings;
            g.can_build_temple = this.can_build_temple;
            g.can_build_tribal = this.can_build_tribal;
            g.can_create_kingdoms = this.can_create_kingdoms;
            g.can_grant_kingdoms_and_empires_to_other_government = this.can_grant_kingdoms_and_empires_to_other_government;
            g.can_imprison_without_reason = this.can_imprison_without_reason;
            g.can_revoke_without_reason = this.can_revoke_without_reason;
            g.can_usurp_kingdoms_and_empires = this.can_usurp_kingdoms_and_empires;
            g.dukes_called_kings = this.dukes_called_kings;
            g.free_retract_vassalage = this.free_retract_vassalage;
            g.have_gender_laws = this.have_gender_laws;
            g.ignores_de_jure_laws = this.ignores_de_jure_laws;
            g.is_patrician = this.is_patrician;
            g.merchant_republic = this.merchant_republic;
            g.uses_decadence = this.uses_decadence;
            g.uses_jizya_tax = this.uses_jizya_tax;
            g.uses_piety_for_law_change = this.uses_piety_for_law_change;
            g.uses_prestige_for_law_change = this.uses_prestige_for_law_change;

            g.accepts_liege_governments.AddRange(this.accepts_liege_governments);
            g.preferred_holdings.AddRange(this.preferred_holdings);
            g.allowed_holdings.AddRange(this.allowed_holdings);
            g.allowed_holdings_culture.AddRange(this.allowed_holdings_culture);
            g.allowed_holdings_culture_and_religion.AddRange(this.allowed_holdings_culture_and_religion);
//            g.cultureAllow.AddRange(cultureAllow);
            g.builds_with_prestige.AddRange(this.builds_with_prestige);
            g.builds_with_piety.AddRange(this.builds_with_piety);
            g.color = this.color;
            g.type = this.type;
            g.SetType(this.type);


            for (int n = 0; n < numChanges; n++)
            {
                g.DoChange();
            }

            if(!GovernmentManager.instance.governments.Contains(g))
            {
                GovernmentManager.instance.governments.Add(g);
            }

            return g;
        }

        public void SetType(string type)
        {
            this.allowed_holdings.Clear();
            this.preferred_holdings.Clear();
            this.allowed_holdings_culture.Clear();
            this.accepts_liege_governments.Clear();
            this.type = type;
            this.accepts_liege_governments.Add(this.name);
            switch (type)
            {
                case "nomadic":
                    this.allowed_holdings.Add("NOMAD");
                    this.preferred_holdings.Add("NOMAD");
                    this.can_build_castle = false;
                    this.can_build_city = false;
                    this.can_build_forts = false;
                    this.can_build_holdings = false;
                    this.can_build_temple = true;
                    this.can_build_tribal = false;
                    this.can_create_kingdoms = false;
                    this.can_build_hospitals = false;
                    this.frame_suffix = "_nomadic";
                    this.title_prefix = "nomadic_";
                    this.is_patrician = false;

                    break;
                case "tribal":
                    this.allowed_holdings.Add("TRIBAL");
                    this.allowed_holdings.Add("FORT");
                    this.preferred_holdings.Add("TRIBAL");
                    this.can_build_castle = false;
                    this.can_build_city = false;
                    this.can_build_forts = false;
                    this.can_build_holdings = true;
                    this.can_build_temple = true;
                    this.can_build_tribal = true;
                    this.can_create_kingdoms = true;
                      this.frame_suffix = "_tribal";
                    this.title_prefix = "tribal_";
                    this.is_patrician = false;
                    this.can_build_hospitals = false;
                    this.can_change_to_nomad_on_start = true;
                {
                        var red = 134;
                        var green = 72;
                        var blue = 11;

                        this.color = Color.FromArgb(255, red, green, blue);
                    }

                    break;
                case "feudal":
                    this.allowed_holdings.Add("CASTLE");
                    this.allowed_holdings.Add("FORT");
                    this.allowed_holdings.Add("HOSPITAL");
                    this.preferred_holdings.Add("CASTLE");
                    this.can_build_hospitals = true;
                    this.can_build_castle = true;
                    this.can_build_city = true;
                    this.can_build_forts = true;
                    this.can_build_holdings = true;
                    this.can_build_temple = true;
                    this.can_build_tribal = false;
                    this.frame_suffix = null;
                    this.title_prefix = null;
                    this.is_patrician = false;
                    this.can_create_kingdoms = true;
                    this.allowed_holdings_culture.Add("TRIBAL");
                    {
                            var red = 132;
                            var green = 172;
                            var blue = 219;

                            this.color = Color.FromArgb(255, red, green, blue);
                        }

                    break;
                case "theocracy":
                    this.allowed_holdings.Add("TEMPLE");
                    this.allowed_holdings.Add("CASTLE");
                    this.allowed_holdings.Add("HOSPITAL");
                    this.allowed_holdings.Add("FORT");
                    this.preferred_holdings.Add("TEMPLE");
                    this.frame_suffix = "_theocracy";
                    this.title_prefix = "temple_";
                    this.can_build_hospitals = true;
                    this.can_build_castle = true;
                    this.can_build_city = true;
                    this.can_build_forts = true;
                    this.can_build_holdings = true;
                    this.can_build_temple = true;
                    this.can_build_tribal = false;
                    this.can_create_kingdoms = true;
                    this.is_patrician = false;
                    this.allowed_holdings_culture.Add("TRIBAL");
                    {
                        var red = 164;
                        var green = 164;
                        var blue = 164;

                        this.color = Color.FromArgb(255, red, green, blue);
                    }

                    break;
                case "republic":
                    this.allowed_holdings.Add("TRADE_POST");
                    this.allowed_holdings.Add("CITY");
                    this.allowed_holdings.Add("CASTLE");
                    this.allowed_holdings.Add("FAMILY_PALACE");
                    this.allowed_holdings.Add("FORT");
                    this.allowed_holdings.Add("HOSPITAL");
                    this.can_build_hospitals = true;
                    this.preferred_holdings.Add("CITY");
                    this.frame_suffix = "_merchantrepublic";
                    this.title_prefix = "city_";
                    this.can_build_castle = true;
                    this.can_build_city = true;
                    this.can_build_forts = true;
                    this.can_build_holdings = true;
                    this.can_build_temple = true;
                    this.can_build_tribal = false;
                    this.can_create_kingdoms = true;
                    this.is_patrician = true;
                    this.allowed_holdings_culture.Add("TRIBAL");
                    {
                        var red = 234;
                        var green = 101;
                        var blue = 101;

                        this.color = Color.FromArgb(255, red, green, blue);
                    }

                    break;
            }
        }

        private void DoChange()
        {
            switch (RandomIntHelper.Next(15))
            {
                case 0:
                    this.aggression = RandomIntHelper.Next(5);
                    if (this.aggression > 3)
                    {
                        this.DoWarryChanges();
                    }

                    if (this.aggression < 2)
                    {
                        this.DoPeaceyChanges();
                    }

                    break;
                case 1:
                    this.allow_title_revokation = !this.allow_title_revokation;
                    break;
                case 2:
                    this.can_revoke_without_reason = !this.can_revoke_without_reason;
                    if (this.can_revoke_without_reason)
                    {
                        this.allow_title_revokation = true;
                        this.can_usurp_kingdoms_and_empires = true;
                    }

                    break;
                case 3:
                    this.can_imprison_without_reason = !this.can_imprison_without_reason;
                    if (this.can_imprison_without_reason)
                    {
                        this.can_revoke_without_reason = true;
                        this.allow_title_revokation = true;
                        this.can_usurp_kingdoms_and_empires = true;
                    }

                    break;

                case 4:
                    this.can_create_kingdoms = !this.can_create_kingdoms;
                    break;
                case 5:
                    this.can_grant_kingdoms_and_empires_to_other_government = !this.can_grant_kingdoms_and_empires_to_other_government;
                    break;
                case 6:
                    this.dukes_called_kings = !this.dukes_called_kings;
                    break;
                case 7:
                    this.free_retract_vassalage = !this.free_retract_vassalage;
                    break;
                 case 8:
                    this.ignores_de_jure_laws = !this.ignores_de_jure_laws;
                    break;
                case 9:
                    this.is_patrician = !this.is_patrician;
                    break;
                case 10:
                    this.merchant_republic = !this.merchant_republic;
                    break;
                case 11:
                    this.uses_decadence = !this.uses_decadence;
                    break;
                case 12:
                    this.uses_jizya_tax = !this.uses_jizya_tax;
                    break;
                case 13:
                    this.uses_piety_for_law_change = !this.uses_piety_for_law_change;
                    if (this.uses_piety_for_law_change)
                    {
                        this.uses_prestige_for_law_change = false;
                    }

                    break;
                case 14:
                    this.uses_prestige_for_law_change = !this.uses_prestige_for_law_change;
                    if (this.uses_prestige_for_law_change)
                    {
                        this.uses_piety_for_law_change = false;
                    }

                    break;
            }
        }

        private void DoPeaceyChanges()
        {
            this.allow_looting = false;
        }

        private void DoWarryChanges()
        {
            this.allow_looting = true;
        }
    }
}
