// <copyright file="CulturalDna.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using CrusaderKingsStoryGen.Managers;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class CulturalDna
    {
        private List<string> vowels = new List<string>() { "a", "e", "i", "o", "u", "ae", "y" };
        public CultureParser culture;

        public static string[] placeFormatOptions = new[] { "{1}{0}" };
        public string placeFormat = null;
        public float wordLengthBias = 1.0f;

        public string empTitle = "emperor";
        public string kingTitle = "king";
        public string dukeTitle = "duke";
        public string countTitle = "count";
        public string baronTitle = "baron";
        public string mayorTitle = "mayor";
        public string femaleEnd = "";


        private List<string> wordsForLand = new List<string>()
        {
        };

        public static List<string> CommonWordFormats = new List<string>()
        {
            "{0}-{1}-{2}",
            "{0} {1}-{2}",
            "{0} {1} {2}",
            "{0}-{1} {2}",
            "{0}'{1}'{2}",
            "{0}-{1}'{2}",
            "{0}'{1}{2}",
            "{0}'{1}-{2}",
        };

        public List<string> WordFormats = new List<string>()
        {
        };

        public CulturalDna()
        {
            this.wordLengthBias = 1.0f;
        }

        private List<string> firstLetters = new List<string>()
        {
            "a", "e", "d", "k", "q", "w", "r", "t", "r", "s", "t", "l", "n", "k", "b",
        };

        List<string> cons = new List<string> { "q", "w", "r", "t", "r", "s", "t", "l", "n", "k", "b", "m", "c", "f", "g", "h", "p", "v", "x", "y", "z" };

        private List<string> CommonStartNames = new List<string>();
        private List<string> CommonMiddleNames = new List<string>();
        private List<string> CommonEndNames = new List<string>();
        public List<string> portraitPool = new List<string>();
        public bool dukes_called_kings = false;
        public bool baron_titles_hidden = false;
        public bool count_titles_hidden = false;
        public bool allow_looting = false;
        public bool seafarer = false;
        public string male_patronym = "son";
        public string female_patronym = "daughter";
        public bool patronym_prefix = false;
        public bool founder_named_dynasties = false;
        public bool dynasty_title_names = false;

        public string Name { get; set; }

        public string from_dynasty_prefix = "af ";
        public bool horde = false;
        public CultureParser dna;
        private bool tribal;

        public void CreateFromCulture(CultureParser c)
        {
            this.dna = c;
        }

        public CulturalDna MutateSmall(int numChanges)
        {
            var c = new CulturalDna();

            c.empTitle = this.empTitle;
            c.kingTitle = this.kingTitle;
            c.dukeTitle = this.dukeTitle;
            c.countTitle = this.countTitle;
            c.baronTitle = this.baronTitle;
            c.mayorTitle = this.mayorTitle;

            c.tribal = this.tribal;
            c.wordsForLand.AddRange(this.wordsForLand);
            c.WordFormats.AddRange(CommonWordFormats);

            c.CommonStartNames.AddRange(this.CommonStartNames);
            c.CommonMiddleNames.AddRange(this.CommonMiddleNames);
            c.CommonEndNames.AddRange(this.CommonEndNames);
            c.portraitPool.AddRange(this.portraitPool);
            c.placeFormat = this.placeFormat;
            c.firstLetters.AddRange(this.firstLetters);
            c.wordLengthBias = this.wordLengthBias;
            c.from_dynasty_prefix = this.from_dynasty_prefix;
            c.count_titles_hidden = this.count_titles_hidden;
            c.baron_titles_hidden = this.baron_titles_hidden;
            c.allow_looting = this.allow_looting;
            c.female_patronym = this.female_patronym;
            c.male_patronym = this.male_patronym;
            c.founder_named_dynasties = this.founder_named_dynasties;
            c.dynasty_title_names = this.dynasty_title_names;
            c.horde = this.horde;
            c.culture = this.culture;
            c.seafarer = this.seafarer;
            for (int n = 0; n < numChanges; n++)
            {
                c.DoRandomSmallChange();
            }

            return c;
        }

        public CulturalDna Mutate(int numChanges, CultureParser rel)
        {
           this.culture = rel;
            var c = new CulturalDna();

            c.empTitle = this.empTitle;
            c.kingTitle = this.kingTitle;
            c.dukeTitle = this.dukeTitle;
            c.countTitle = this.countTitle;
            c.baronTitle = this.baronTitle;
            c.mayorTitle = this.mayorTitle;

            c.tribal = this.tribal;
            c.wordsForLand.AddRange(this.wordsForLand);
            //c.cons.AddRange(cons);
            //   c.vowels.AddRange(vowels);
            c.CommonStartNames.AddRange(this.CommonStartNames);
            c.CommonMiddleNames.AddRange(this.CommonMiddleNames);
            c.CommonEndNames.AddRange(this.CommonEndNames);
            c.WordFormats.AddRange(CommonWordFormats);
            c.portraitPool.AddRange(this.portraitPool);
            c.placeFormat = this.placeFormat;
            c.firstLetters.AddRange(this.firstLetters);
            c.wordLengthBias = this.wordLengthBias;
            c.from_dynasty_prefix = this.from_dynasty_prefix;
            c.count_titles_hidden = this.count_titles_hidden;
            c.baron_titles_hidden = this.baron_titles_hidden;
            c.allow_looting = this.allow_looting;
            c.female_patronym = this.female_patronym;
            c.male_patronym = this.male_patronym;
            c.founder_named_dynasties = this.founder_named_dynasties;
            c.dynasty_title_names = this.dynasty_title_names;
            c.horde = this.horde;
            c.culture = this.culture;
            c.seafarer = this.seafarer;
            for (int n = 0; n < numChanges; n++)
            {
                c.DoRandomChange(rel);
            }



            return c;
        }

        public void DoRandom()
        {
            this.from_dynasty_prefix = this.ConstructWord(2, 4).ToLower() + " ";
            this.female_patronym = this.ConstructWord(3, 4).ToLower();
            this.male_patronym = this.ConstructWord(3, 4).ToLower();

            this.allow_looting = RandomIntHelper.Next(2) == 0;
            if (this.allow_looting && GovernmentManager.instance.numNomadic < 10)
                this.horde = false;
            else
            {
                this.horde = false;
            }

            do
            {
                if (Simulation.SimulationManager.instance.AllowCustomTitles)
                {
                    this.empTitle = this.ConstructWord(2, 5);
                    this.kingTitle = this.ConstructWord(2, 5);
                    this.dukeTitle = this.ConstructWord(2, 5);
                    this.countTitle = this.ConstructWord(2, 5);
                    this.baronTitle = this.ConstructWord(2, 5);
                    this.mayorTitle = this.ConstructWord(2, 5);
                    this.empTitle = LanguageManager.instance.AddSafe(this.empTitle);
                    this.kingTitle = LanguageManager.instance.AddSafe(this.kingTitle);
                    this.dukeTitle = LanguageManager.instance.AddSafe(this.dukeTitle);
                    this.countTitle = LanguageManager.instance.AddSafe(this.countTitle);
                    this.baronTitle = LanguageManager.instance.AddSafe(this.baronTitle);
                    this.mayorTitle = LanguageManager.instance.AddSafe(this.mayorTitle);
                }
            } while (this.empTitle == null || this.kingTitle == null || this.dukeTitle == null || this.countTitle == null ||
                     this.baronTitle == null || this.mayorTitle == null ||
                     this.empTitle == "" || this.kingTitle == "" || this.dukeTitle == "" || this.countTitle == "" || this.baronTitle == "" ||
                     this.mayorTitle == "");
        }

        private void DoRandomChange(CultureParser rel)
        {
            switch (RandomIntHelper.Next(4))
            {
                case 0:
                    {
                        int count = this.CommonStartNames.Count / 2;
                        this.WordFormats.Clear();
                        this.ReplaceStartNames(count);
                        {
                            this.wordsForLand.Clear();
                           int create = 3;

                            for (int n = 0; n < create; n++)
                            {
                                string a = "";
                                a = this.ConstructWord(2 * this.wordLengthBias, 4 * this.wordLengthBias);

                                if (!this.wordsForLand.Contains(a))
                                {
                                    this.wordsForLand.Add(a);
                                    continue;
                                }
                            }
                        }
                    }

                    break;

                case 1:
                    {
                        int count = this.CommonEndNames.Count / 2;
                        this.ReplaceEndNames(count);
                        this.WordFormats.Clear();
                        {
                            this.wordsForLand.Clear();
                            int c = this.wordsForLand.Count / 2;
                            int create = 3;

                            for (int n = 0; n < create; n++)
                            {
                                string a = "";
                                a = this.ConstructWord(2 * this.wordLengthBias, 4 * this.wordLengthBias);

                                if (!this.wordsForLand.Contains(a))
                                {
                                    this.wordsForLand.Add(a);
                                    continue;
                                }
                            }
                        }
                    }

                    break;

                case 2:

                    // replace 1/3 of the words for land
                    {
                        this.WordFormats.Clear();
                        if (this.portraitPool.Count == 0)
                        {
                            return;
                        }

                        int x = RandomIntHelper.Next(this.portraitPool.Count);

                        var por = this.portraitPool[x];
                        this.portraitPool.RemoveAt(x);
                        this.portraitPool.Add(this.culture.GetRelatedCultureGfx(por));

                        if (RandomIntHelper.Next(2) == 0)
                        {
                            if (this.portraitPool.Count == 1)
                            {
                                this.portraitPool.Add(this.culture.GetRelatedCultureGfx(por));
                            }
                            else if (this.portraitPool.Count == 2)
                            {
                                this.portraitPool.RemoveAt(RandomIntHelper.Next(2));
                            }
                        }

                        if (RandomIntHelper.Next(2) == 0)
                        {
                            this.portraitPool.RemoveAt(0);
                            this.portraitPool.Add(CultureParser.GetRandomCultureGraphics());
                        }

                        while(this.portraitPool.Count > 1)
                        {
                            this.portraitPool.RemoveAt(0);
                        }
                    }

                    break;

                case 3:


                    {
                        if (Simulation.SimulationManager.instance.AllowCustomTitles)
                        {
                            //    case 0:
                            this.empTitle = this.ConstructWord(2, 5);
                            this.empTitle = LanguageManager.instance.AddSafe(this.empTitle);

                            //       break;
                            //    case 1:
                            this.kingTitle = this.ConstructWord(2, 5);
                            this.kingTitle = LanguageManager.instance.AddSafe(this.kingTitle);

                            //      break;
                            //  case 2:
                            this.dukeTitle = this.ConstructWord(2, 5);
                            this.dukeTitle = LanguageManager.instance.AddSafe(this.dukeTitle);

                            //       break;
                            //   case 3:
                            this.countTitle = this.ConstructWord(2, 5);
                            this.countTitle = LanguageManager.instance.AddSafe(this.countTitle);

                            //     break;
                            //    case 4:
                            this.baronTitle = this.ConstructWord(2, 5);
                            this.baronTitle = LanguageManager.instance.AddSafe(this.baronTitle);

                            //        break;
                            //    case 5:
                            this.mayorTitle = this.ConstructWord(2, 5);
                            this.mayorTitle = LanguageManager.instance.AddSafe(this.mayorTitle);
                        }

                        //       break;
                    }

                    break;
            }

            if (this.culture != null)
            {
                this.culture.DoDetailsForCulture();
            }
        }

        private void DoRandomSmallChange()
        {
            {
                switch (RandomIntHelper.Next(12))
                {
                    case 0:
                        this.founder_named_dynasties = false;//!founder_named_dynasties;
                        break;
                    case 1:
                        this.dynasty_title_names = false;//!dynasty_title_names;
                        break;
                    case 2:
                        this.baron_titles_hidden = !this.baron_titles_hidden;
                        break;
                    case 3:
                        this.count_titles_hidden = !this.count_titles_hidden;
                        break;
                    case 4:
                        this.dukes_called_kings = !this.dukes_called_kings;
                        break;
                    case 5:
                        this.from_dynasty_prefix = this.ConstructWord(2, 3);
                        break;
                    case 6:
                        this.female_patronym = this.ConstructWord(3, 4).ToLower();
                        this.male_patronym = this.ConstructWord(3, 4).ToLower();
                        if (RandomIntHelper.Next(10) == 0)
                        {
                            this.female_patronym = this.ConstructWord(3, 4);
                            this.male_patronym = this.ConstructWord(3, 4);
                        }

                        break;
                    case 7:
                        // Change place format
                        {
                            this.placeFormat = null;
                        }

                        break;
                    case 8:
                        this.WordFormats.RemoveAt(RandomIntHelper.Next(this.WordFormats.Count));
                        this.WordFormats.Add(CommonWordFormats[RandomIntHelper.Next(CommonWordFormats.Count)]);
                        break;
                    case 9:
                        this.tribal = !this.tribal;
                        break;
                    case 10:

                        if (Simulation.SimulationManager.instance.AllowCustomTitles)
                        {
                            //    for (int n = 0; n < 3; n++)
                                switch (RandomIntHelper.Next(6))
                            {
                                case 0:
                                    this.empTitle = this.ConstructWord(2, 5);
                                    this.empTitle = LanguageManager.instance.AddSafe(this.empTitle);

                                    break;
                                case 1:
                                    this.kingTitle = this.ConstructWord(2, 5);
                                    this.kingTitle = LanguageManager.instance.AddSafe(this.kingTitle);

                                    break;
                                case 2:
                                    this.dukeTitle = this.ConstructWord(2, 5);
                                    this.dukeTitle = LanguageManager.instance.AddSafe(this.dukeTitle);

                                    break;
                                case 3:
                                    this.countTitle = this.ConstructWord(2, 5);
                                    this.countTitle = LanguageManager.instance.AddSafe(this.countTitle);

                                    break;
                                case 4:
                                    this.baronTitle = this.ConstructWord(2, 5);
                                    this.baronTitle = LanguageManager.instance.AddSafe(this.baronTitle);

                                    break;
                                case 5:
                                    this.mayorTitle = this.ConstructWord(2, 8);
                                    this.mayorTitle = LanguageManager.instance.AddSafe(this.mayorTitle);

                                    break;
                            }
                        }

                        break;
                    case 11:
                        this.seafarer = !this.seafarer;
                        break;
                }
            }

            if (this.culture != null)
            {
                this.culture.DoDetailsForCulture();
            }
        }

        private void ReplaceStartNames(int count)
        {
            int i = this.CommonStartNames.Count;
            int c = count;
            int removed = c;
            for (int n = 0; n < c; n++)
            {
                this.CommonStartNames.RemoveAt(RandomIntHelper.Next(this.CommonStartNames.Count));
            }

            while (this.CommonStartNames.Count < i)
            {
                this.AddRandomStartNames(i - this.CommonStartNames.Count);
            }
        }


        private void ReplaceEndNames(int count)
        {
            int i = this.CommonEndNames.Count;
            int c = count;
            int removed = c;
            for (int n = 0; n < c; n++)
            {
                this.CommonEndNames.RemoveAt(RandomIntHelper.Next(this.CommonEndNames.Count));
            }

            while (this.CommonEndNames.Count < i)
            {
                this.AddRandomEndNames(i - this.CommonEndNames.Count);
            }
        }

        private int AddRandomStartNames(int count)
        {
            int c = count;
            CulturalDna dna = CulturalDnaManager.instance.GetVanillaCulture((string)null);
            List<string> choices = new List<string>();
            int added = 0;
            for (int n = 0; n < c; n++)
            {
                string str = dna.CommonStartNames[RandomIntHelper.Next(dna.CommonStartNames.Count)];
                if (this.CommonStartNames.Contains(str))
                {
                }
                else
                {
                    choices.Add(str);
                }
            }


            if (choices.Count > 0)
            {
                c = Math.Min(choices.Count, c);
                for (int n = 0; n < c; n++)
                {
                    var cc = choices[RandomIntHelper.Next(choices.Count)];
                    this.CommonStartNames.Add(cc);
                    choices.Remove(cc);
                    n--;
                    c = Math.Min(choices.Count, c);
                    added++;
                }
            }


            return added;
        }


        private int AddRandomEndNames(int count)
        {
            int c = count;
            CulturalDna dna = CulturalDnaManager.instance.GetVanillaCulture((string)null);
            List<string> choices = new List<string>();
            int added = 0;
            for (int n = 0; n < c; n++)
            {
                string str = dna.CommonEndNames[RandomIntHelper.Next(dna.CommonEndNames.Count)];
                if (this.CommonEndNames.Contains(str))
                {
                }
                else
                {
                    choices.Add(str);
                }
            }


            if (choices.Count > 0)
            {
                c = Math.Min(choices.Count, c);
                for (int n = 0; n < c; n++)
                {
                    var cc = choices[RandomIntHelper.Next(choices.Count)];
                    this.CommonEndNames.Add(cc);
                    choices.Remove(cc);
                    n--;
                    c = Math.Min(choices.Count, c);
                    added++;
                }
            }


            return added;
        }

        public string GetMaleNameBlock()
        {
            List<string> sts = new List<string>();
            StringBuilder b = new StringBuilder();
            for (int n = 0; n < 200; n++)
            {
                string s = this.GetMaleName();
                if (!sts.Contains(s))
                {
                    b.Append(s + " ");
                }
            }

            return b.ToString();
        }

        public string GetFemaleNameBlock()
        {
            List<string> sts = new List<string>();
            StringBuilder b = new StringBuilder();
            for (int n = 0; n < 200; n++)
            {
                var s = this.GetFemaleName();
                if (!sts.Contains(s))
                {
                    b.Append(s + " ");
                }
            }

            return b.ToString();
        }

        public List<string> GetMaleNameBlockCSV()
        {
            List<string> sts = new List<string>();
            StringBuilder b = new StringBuilder();
            for (int n = 0; n < 100; n++)
            {
                string s = this.GenMaleName();
                sts.Add(s);
            }

            return sts;
        }

        public List<string> GetFemaleNameBlockCSV()
        {
            List<string> sts = new List<string>();
            StringBuilder b = new StringBuilder();
            for (int n = 0; n < 100; n++)
            {
                string s = this.GenFemaleName();
                sts.Add(s);
            }

            return sts;
        }
    }
}