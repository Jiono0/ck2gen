// <copyright file="ScripterTriggerManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System.Collections.Generic;

    class ScripterTriggerManager
    {
        public static ScripterTriggerManager instance = new ScripterTriggerManager();

        public List<string> removals = new List<string>();

        public Script script;

        public void Load()
        {
            this.script = ScriptLoader.instance.Load(Globals.GameDir + "common\\scripted_triggers\\00_scripted_triggers.txt");

            this.removals.Add("_prerequisites");
            for (var i = 0; i < this.script.Root.Children.Count; i++)
            {
                var rootChild = this.script.Root.Children[i];
                if (rootChild is ScriptScope)
                {
                    var trigger = (rootChild as ScriptScope);
                    for (var index = 0; index < this.removals.Count; index++)
                    {
                        var removal = this.removals[index];
                        if (trigger.Name.Contains(removal))
                        {
                            this.script.Root.Remove(rootChild);
                            i--;
                            continue;
                        }
                    }
                }
            }
        }

        public void Save()
        {
            /*
            script.Root.Do($@"
            has_assassins_prerequisites = {{
                age = 16
	            true_religion_{ReligionManager.instance.ShiiteEquiv.Name}_trigger = yes
	                OR = {{
		                is_female = no
		                has_game_rule = {{
			                name = gender
			                value = all
		                }}
	                }}
                }}
            ");

            script.Root.Do($@"
            has_common_devilworship_prerequisites = {{
	            age = 16
	            custom_tooltip = {{
		            text = must_be_a_sinner_tooltip
		            hidden_tooltip = {{
			            OR = {{
				            has_impious_trait_trigger = yes
				            has_vice_trigger = yes
				            trait = drunkard
				            trait = possessed
				            trait = lunatic
			            }}
		            }}
	            }}
            }}

            ");

            script.Root.Do($@"

            has_satanists_prerequisites = {{
	            has_common_devilworship_prerequisites = yes
	            OR = {{
		            religion_group = {ReligionManager.instance.ChristianGroupSub.Name}
		            religion_group = {ReligionManager.instance.MuslimGroupSub.Name}
		            religion_group = {ReligionManager.instance.ZoroGroupSub.Name}
		            religion_group = {ReligionManager.instance.JewGroupSub.Name}
	            }}
            }}

            ");



            script.Root.Do($@"

        has_trollcrafters_prerequisites = {{
	        has_common_devilworship_prerequisites = yes
	        OR = {{
		        religion_group = {ReligionManager.instance.PaganGroupSub.Name}
	        }}
        }}

        ");


            script.Root.Do($@"

        has_cult_of_kali_prerequisites = {{
	        has_common_devilworship_prerequisites = yes
	        OR = {{
		        religion = {ReligionManager.instance.HinduEquiv.Name}
	        }}
        }}

        ");

            script.Root.Do($@"

        has_cold_ones_prerequisites = {{
	        has_common_devilworship_prerequisites = yes
	        OR = {{
		        religion = {ReligionManager.instance.JainEquiv.Name}
	        }}
        }}

        ");



            script.Root.Do($@"

        has_plaguebringers_prerequisites = {{
	        has_common_devilworship_prerequisites = yes
	        OR = {{
		        religion = {ReligionManager.instance.BuddhistEquiv.Name}
	        }}
        }}

        ");
        */

            this.script.Save();
        }

        public void AddTrigger(ReligionParser religion)
        {
            this.script.Root.Do($@"

                true_religion_{religion.Name}_trigger = {{
	                true_religion = {religion.Name}
                }}


");
        }

        public void AddTrigger(ReligionGroupParser group)
        {
            if (this.script == null)
            {
                return;
            }

            this.script.Root.Do($@"
                true_religion_group_{group.Name}_trigger = {{
	                true_religion_group = {group.Name}
                }}


");
        }
    }
}
