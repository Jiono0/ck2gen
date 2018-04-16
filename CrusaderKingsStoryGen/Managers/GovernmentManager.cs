// <copyright file="GovernmentManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    class GovernmentManager : ISerializeXml
    {
        public static GovernmentManager instance = new GovernmentManager();
        public List<GovermentPolicyHelper> governments = new List<GovermentPolicyHelper>();

        public void Init()
        {
        }

        public HashSet<string> done = new HashSet<string>();
        public List<string> cultureDone = new List<string>();

        public GovermentPolicyHelper BranchGovernment(GovermentPolicyHelper gov, CultureGroupParser group, CultureParser culture)
        {
            var newGov = gov.Mutate(10);

            {
                if (newGov.type == "nomadic")
                {
                    newGov.type = "tribal";
                    newGov.SetType(newGov.type);
                }
            }

            do
            {
                newGov.name = culture.dna.GetUniqueMaleName();
            } while (LanguageManager.instance.Get(StarHelper.SafeName(newGov.name) + "_government") != null);

            string s = newGov.name;
            newGov.name = StarHelper.SafeName(newGov.name) + "_government";
            LanguageManager.instance.Add(newGov.name, s);
            group.Governments.Add(newGov);
            if (!newGov.cultureGroupAllow.Contains(culture.Group.Name))
            {
                newGov.cultureGroupAllow.Add(culture.Group.Name);
            }

            return newGov;
        }

        public void Save()
        {
            return;
            foreach (var cultureParser in CultureManager.instance.AllCultureGroups)
            {
                if (cultureParser.Governments.Count == 0)
                {
                //    var gov = GovernmentManager.instance.CreateNewGovernment(cultureParser.Cultures[0]);
                }

                if (!this.cultureDone.Contains(cultureParser.Name))
                {
                    var g = GovernmentManager.instance.CreateNewGovernment(cultureParser.Cultures[0]);

                    cultureParser.Governments.Add(g);
                }

                foreach (var government in cultureParser.Governments)
                {
                    if (government.cultureGroupAllow.Count == 0)
                    {
                        government.cultureGroupAllow.Add(cultureParser.Name);
                    }
                }
            }

            if (!Directory.Exists(Globals.ModDir + "gfx\\interface\\"))
            {
                Directory.CreateDirectory(Globals.ModDir + "gfx\\interface\\");
            }

            var files = Directory.GetFiles(Globals.ModDir + "gfx\\interface\\");
            foreach (var file in files)
            {
                File.Delete(file);
            }


            foreach (var government in this.governments)
            {
                try
                {
                    File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_republic.dds",
                        Globals.ModDir + "gfx\\interface\\government_icon_" +
                        government.name.Replace("_government", "republic_government") + ".dds");
                    File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_feudal.dds",
                                 Globals.ModDir + "gfx\\interface\\government_icon_" +
                                 government.name.Replace("_government", "feudal_government") + ".dds");
                    File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_tribal.dds",
                                              Globals.ModDir + "gfx\\interface\\government_icon_" +
                                              government.name.Replace("_government", "tribal_government") + ".dds");
                    File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_nomadic.dds",
                                                      Globals.ModDir + "gfx\\interface\\government_icon_" +
                                                      government.name.Replace("_government", "nomadic_government") + ".dds");
                    File.Copy(Globals.GameDir + "gfx\\interface\\government_icon_theocracy.dds",
                                                                    Globals.ModDir + "gfx\\interface\\government_icon_" +
                                                                    government.name.Replace("_government", "theocracy_government") + ".dds");
                }
                catch (Exception)
                {
                }
            }




            Script s = new Script();
            var scope = new ScriptScope();
            /*
             s.Name = Globals.ModDir + "common\\governments\\nomadic_governments.txt";

             s.Root = new ScriptScope();

              scope.Name = "nomadic_governments";
             s.Root.Add(scope);
             foreach (var government in governments)
             {
                 if (government.cultureAllow.Count > 0)
                 {
                     {
                         government.SetType("nomadic");

                         var g = new ScriptScope();
                         g.Name = government.name.Replace("_government", government.type+"_government");
                         var last = government.name;
                         government.name = g.Name;
                         LanguageManager.instance.Add(g.Name,
                              CultureManager.instance.CultureMap[government.cultureAllow[0]].dna.GetUniqueMaleName());
                         government.SetType("nomadic");

                         government.Save(g);
                         scope.Add(g);
                         government.name = last;
                     }

                 }
             }

             s.Save();
             */
            s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\feudal_governments.txt";

            s.Root = new ScriptScope();

            scope = new ScriptScope();
            scope.Name = "feudal_governments";
            s.Root.Add(scope);
            foreach (var government in this.governments)
            {
                if (government.cultureGroupAllow.Count > 0)
                {
                    government.SetType("feudal");
                    var g = new ScriptScope();
                    g.Name = government.name.Replace("_government", government.type + "_government");
                    var last = government.name;
                    government.name = g.Name;
                    LanguageManager.instance.Add(g.Name,
                         CultureManager.instance.CultureGroupMap[government.cultureGroupAllow[0]].Cultures[0].dna.GetUniqueMaleName());
                    government.SetType("feudal");
                    government.Save(g);
                    scope.Add(g);
                    government.name = last;
                }
            }

            s.Save();
            s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\theocracy_governments.txt";

            s.Root = new ScriptScope();

            scope = new ScriptScope();
            scope.Name = "theocracy_governments";
            s.Root.Add(scope);
            foreach (var government in this.governments)
            {
                if (government.cultureGroupAllow.Count > 0)
                {
                    government.SetType("theocracy");
                    var g = new ScriptScope();
                    g.Name = government.name.Replace("_government", government.type + "_government");
                    var last = government.name;
                    government.name = g.Name;
                    LanguageManager.instance.Add(g.Name,
                         CultureManager.instance.CultureGroupMap[government.cultureGroupAllow[0]].Cultures[0].dna.GetUniqueMaleName());
                    government.SetType("theocracy");
                    government.Save(g);
                    scope.Add(g);
                    government.name = last;
                }
            }

            s.Save();
            s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\republic_governments.txt";

            s.Root = new ScriptScope();

            scope = new ScriptScope();
            scope.Name = "republic_governments";
            s.Root.Add(scope);
            foreach (var government in this.governments)
            {
                if (government.cultureGroupAllow.Count > 0)
                {
                    government.SetType("republic");
                    var g = new ScriptScope();
                    g.Name = government.name.Replace("_government", government.type + "_government");
                    var last = government.name;
                    government.name = g.Name;
                    LanguageManager.instance.Add(g.Name,
                         CultureManager.instance.CultureGroupMap[government.cultureGroupAllow[0]].Cultures[0].dna.GetUniqueMaleName());
                    government.SetType("republic");
                    government.Save(g);
                    scope.Add(g);
                    government.name = last;
                }
            }

            s.Save();
            s = new Script();

            s.Name = Globals.ModDir + "common\\governments\\tribal_governments.txt";

            s.Root = new ScriptScope();

            scope = new ScriptScope();
            scope.Name = "tribal_governments";
            s.Root.Add(scope);
            foreach (var government in this.governments)
            {
                if (government.cultureGroupAllow.Count > 0)
                {
                    government.SetType("tribal");
                    var g = new ScriptScope();
                    g.Name = government.name.Replace("_government", government.type + "_government");
                    var last = government.name;
                    government.name = g.Name;
                    SpriteManager.instance.AddGovernment(government);
                    LanguageManager.instance.Add(g.Name,
                       CultureManager.instance.CultureGroupMap[government.cultureGroupAllow[0]].Cultures[0].dna.GetUniqueMaleName());
                    government.SetType("tribal");
                    government.Save(g);
                    scope.Add(g);
                    government.name = last;
                }
            }

            s.Save();
        }

        public int numNomadic = 0;
        public int numTribal = 0;

        public GovermentPolicyHelper CreateNewGovernment(CultureParser culture)
        {
            GovermentPolicyHelper g = new GovermentPolicyHelper();
            g.type = "tribal";
            GovermentPolicyHelper r = g.Mutate(16);
            r.name = culture.dna.GetUniqueMaleName();
            string s = r.name;
            r.name = StarHelper.SafeName(r.name) + "_government";
            LanguageManager.instance.Add(r.name, s);
            culture.Group.Governments.Add(r);
            r.SetType(r.type);
            if (!r.cultureGroupAllow.Contains(culture.Group.Name))
            {
                r.cultureGroupAllow.Add(culture.Group.Name);   //    governments.Add(r);
            }

            return r;
        }



        public void SaveProject(XmlWriter writer)
        {
        }

        public void LoadProject(XmlReader reader)
        {
        }
    }
}
