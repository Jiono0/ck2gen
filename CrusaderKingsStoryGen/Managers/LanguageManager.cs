// <copyright file="LanguageManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using CrusaderKingsStoryGen.Helpers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    class LanguageManager
    {
        public static LanguageManager instance = new LanguageManager();
        Dictionary<string, string> english = new Dictionary<string, string>();

        public LanguageManager()
        {
            this.AddSafe("Emperor");
            this.AddSafe("King");
            this.AddSafe("Duke");
            this.AddSafe("Count");
            this.AddSafe("Baron");
            this.AddSafe("Mayor");
        }

        public string Add(string key, string english)
        {
            if (english == null || english.Length == 1)
            {
                english = StarHelper.Generate(RandomIntHelper.Next(45645546));
            }

            this.english[key] = english;
            this.english[key + "_adj"] = english;
            this.english[key + "_desc"] = english;
            return english;
        }

        public string AddDirect(string key, string english)
        {
            this.english[key] = english;

            return english;
        }

        public void LoadVanilla()
        {
            var files = ModManager.instance.GetFiles("localisation");
            foreach (var file in files)
            {
                using (System.IO.StreamReader load =
                    new System.IO.StreamReader(file, Encoding.GetEncoding(1252)))
                {
                    using (System.IO.StreamWriter file2 =
                        new System.IO.StreamWriter(
                            Globals.ModDir + "localisation\\" + file.Substring(file.LastIndexOf('\\')), false,
                            Encoding.GetEncoding(1252)))
                    {
                        try
                        {
                            while (!load.EndOfStream)
                            {
                                string str = load.ReadLine();
                                var split = str.Split(';');
                                if (split[0] == "e_britannia")
                                {
                                }

                                LanguageManager.instance.AddDirect(split[0].Trim(), split[1].Trim());
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        public void DoSubstitutes()
        {
            var files = Directory.GetFiles(Globals.ModDir + "localisation\\");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            files = ModManager.instance.GetFiles("localisation");
            foreach (var file in files)
            {
                if (file.Contains("customizable"))
                {
                    continue;
                }

                bool bStripped = false;
                bool bReplaced = false;
                using (System.IO.StreamReader load =
                  new System.IO.StreamReader(file, Encoding.GetEncoding(1252)))
                {
                    using (System.IO.StreamWriter file2 =
                           new System.IO.StreamWriter(Globals.ModDir + "localisation\\" + file.Substring(file.LastIndexOf('\\')), false, Encoding.GetEncoding(1252)))
                    {
                        try
                        {
                            while (!load.EndOfStream)
                            {
                                string str = load.ReadLine();

                                if (str.StartsWith("pagan_group;") || str.StartsWith("pagan;") || str.StartsWith("norse;")  || str.StartsWith("c_") || str.StartsWith("b_") || str.StartsWith("d_") || str.StartsWith("k_") || str.StartsWith("e_"))
                                {
                                    bStripped = true;
                                    continue;
                                }

                                if (this.trimProvinces && str.StartsWith("PROV"))
                                {
                                    try
                                    {
                                        string small = str.Substring(4);
                                        small = small.Substring(0, small.IndexOf(';'));
                                        Convert.ToInt32(small);
                                        bStripped = true;
                                        continue;
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }

                                foreach (var key in this.substitutions.Keys)
                                {
                                    if (str.Contains(key))
                                    {
                                        int i = str.IndexOf(';');
                                        string sub = str.Substring(i+1);
                                        if (sub.Contains(key))
                                        {
                                            str = str.Substring(0, i+1) + sub.ReplaceMinusEscape(key, this.substitutions[key]);
                                            bReplaced = true;
                                        }
                                    }
                                }

                                file2.Write(str + Environment.NewLine);
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }

                    if (!bStripped && !bReplaced)
                    {
                        try
                        {
                            File.Delete(Globals.ModDir + "localisation\\" + file.Substring(file.LastIndexOf('\\')));
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        public void Save()
        {
            string filename = "localisation\\aaa_genLanguage.csv";

            filename = Globals.ModDir + filename;

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
            {
                foreach (var entry in this.english)
                {
                    file.Write(entry.Key + ";" + entry.Value + ";;;;;;;;;;;;;\n");
                }

                file.Close();
            }


            //thing;eng;;;;;;;;;;;;;
        }

        public void Remove(string name)
        {
            this.english.Remove(name);
        }

        public string Get(string name)
        {
            if (!this.english.ContainsKey(name))
            {
                return "";
            }

            if (this.english[name] == null)
            {
                this.english[name] = "";
            }

            return this.english[name].Trim();
        }

        public string AddSafe(string name)
        {
            string safe = StarHelper.SafeName(name);
            this.Add(safe, name);
            return safe;
        }

        public void CopyToolDir(string dir)
        {
            return;
        }

        Dictionary<string, string> substitutions = new Dictionary<string, string>();
        public bool trimProvinces = true;


        public void SetupReligionEventSubsitutions()
        {
            try
            {
                this.substitutions["Pope"] = this.Get(ReligionManager.instance.CatholicSub.PopeName);
                this.substitutions["Jesus"] = ReligionManager.instance.ChristianGroupSub.Religions[0].high_god_name;
                this.substitutions["Christian"] = this.Get(ReligionManager.instance.ChristianGroupSub.Name) + "an";
                this.substitutions["Holy Father"] = "Holy " + this.Get(ReligionManager.instance.ChristianGroupSub.Religions[0].PopeName);
                this.substitutions["Catholic"] = this.Get(ReligionManager.instance.ChristianGroupSub.Name) + "an";
                this.substitutions["christian"] = this.Get(ReligionManager.instance.ChristianGroupSub.Name) + "an";
                this.substitutions["catholic"] = this.Get(ReligionManager.instance.ChristianGroupSub.Name) + "an";
                this.substitutions["Bible"] = ReligionManager.instance.CatholicSub.scripture_name;
                this.substitutions["bible"] = this.substitutions["Bible"].ToLower();
                this.substitutions["Crusade"] = ReligionManager.instance.ChristianGroupSub.Religions[0].crusade_name;
                this.substitutions["crusade"] = ReligionManager.instance.ChristianGroupSub.Religions[0].crusade_name;

                var mainHolyPlace = ReligionManager.instance.MuslimGroupSub.Religions[0].holySites.First();
                this.substitutions["Sadaqah"] = ReligionManager.instance.MuslimGroupSub.holySites.First().Culture.dna.GetPlaceName();
                this.substitutions["Ramadan"] = ReligionManager.instance.MuslimGroupSub.holySites.First().Culture.dna.GetPlaceName();
                this.substitutions["Mecca"] = this.Get(mainHolyPlace.ProvinceTitle);

                this.substitutions["Allah"] = ReligionManager.instance.MuslimGroupSub.Religions[0].high_god_name;
                this.substitutions["Muslim"] = this.Get(ReligionManager.instance.MuslimGroupSub.Name);
                this.substitutions["Islam"] = this.Get(ReligionManager.instance.MuslimGroupSub.Name);
                this.substitutions["Hajj"] = "pilgrimage";
                this.substitutions["Caliph"] = ReligionManager.instance.MuslimGroupSub.Religions[0].priest;
                this.substitutions["Sunni"] = this.Get(ReligionManager.instance.SunniEquiv.Name);
                this.substitutions["Shiite"] = this.Get(ReligionManager.instance.ShiiteEquiv.Name);
                this.substitutions["Ash'ari"] = ReligionManager.instance.MuslimGroupSub.Provinces[0].Culture.dna.ConstructWord(4, 7);
                this.substitutions["Mu'tazila"] = ReligionManager.instance.MuslimGroupSub.Provinces[0].Culture.dna.ConstructWord(4, 7);
                this.substitutions["Fatwa"] = ReligionManager.instance.MuslimGroupSub.Provinces[0].Culture.dna.ConstructWord(4, 7);
                this.substitutions["Kaaba"] = ReligionManager.instance.MuslimGroupSub.Provinces[0].Culture.dna.ConstructWord(4, 7);
                this.substitutions["Muhammad"] = ReligionManager.instance.MuslimGroupSub.Provinces[0].Culture.dna.ConstructWord(4, 7);
                this.substitutions["Hajajj"] = ReligionManager.instance.MuslimGroupSub.Provinces[0].Culture.dna.ConstructWord(4, 7);
                this.substitutions["Furusiyya"] = ReligionManager.instance.MuslimGroupSub.Provinces[0].Culture.dna.ConstructWord(4, 8);

                this.substitutions["Qur'an"] = ReligionManager.instance.ShiiteEquiv.scripture_name;
                this.substitutions["Jihad"] = ReligionManager.instance.MuslimGroupSub.Religions[0].crusade_name;
                this.substitutions["jihad"] = ReligionManager.instance.MuslimGroupSub.Religions[0].crusade_name;
                this.substitutions["imam"] = ReligionManager.instance.MuslimGroupSub.Religions[0].priest;
                this.substitutions["Kafir"] = "infidels";


                this.substitutions["pagan"] = this.Get(ReligionManager.instance.PaganGroupSub.Name);
                this.substitutions["Pagan"] = this.Get(ReligionManager.instance.PaganGroupSub.Name);
                this.substitutions["Blot"] = ReligionManager.instance.NorseSub.Provinces[0].Culture.dna.ConstructWord(4, 5);
                this.substitutions["blot"] = this.substitutions["Blot"];
                this.substitutions["Odin"] = ReligionManager.instance.NorseSub.gods[0];
                this.substitutions["Thor"] = ReligionManager.instance.NorseSub.gods[1];
                this.substitutions["Freyr"] = ReligionManager.instance.NorseSub.gods[2];

                if (ReligionManager.instance.JewGroupSub.Religions[0].holySites.Any())
                {
                    var sHolyPlace = ReligionManager.instance.JewGroupSub.Religions[0].holySites.ToList()[1];//First();
                    this.substitutions["Jerusalem"] = this.Get(sHolyPlace.ProvinceTitle);
                }

                this.substitutions["Jew"] = ReligionManager.instance.JewGroupSub.Provinces[0].Culture.dna.ConstructWord(2, 3);
                this.substitutions["jew"] = this.substitutions["Jew"].ToLower();
                this.substitutions["Third Temple"] = ReligionManager.instance.JewGroupSub.Provinces[0].Culture.dna.ConstructWord(2, 4) + " Temple";
                this.substitutions["Passover"] = ReligionManager.instance.JewGroupSub.Provinces[0].Culture.dna.ConstructWord(5, 8);

                this.substitutions["Jain"] = ReligionManager.instance.JainEquiv.Name;
                this.substitutions["Buddhist"] = ReligionManager.instance.BuddhistEquiv.Name;
                this.substitutions["Hindu"] = ReligionManager.instance.HinduEquiv.Name;
            }
            catch (Exception)
            {
            }
        }
    }
}
