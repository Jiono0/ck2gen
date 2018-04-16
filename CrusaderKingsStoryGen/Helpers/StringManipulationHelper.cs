// <copyright file="CultureDnaStringManip.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StringManipulationHelper
    {

        private List<string> firstLetters = new KingdomHelper().firstLetters;
        public string changeFirstLetter(string input)
        {
            string orig = input;
            string[] sub = input.Split(' ');
            //     if (bVowel)

            string output = "";
            foreach (var s in sub)
            {
                input = s;
                if (input.Trim().Length == 0)
                {
                    return "";
                }

                {
                    bool bVowel = false;
                    if (this.StartsWithVowel(input))
                    {
                        bVowel = true;
                    }

                    input = input.Substring(1);

                    if(RandomIntHelper.Next(2)==0)
                    while (input.Length > 8)
                    {
                        input = this.stripVowelsFromStart(input);
                        input = this.stripConsinentsFromStart(input);
                    }
                    else
                        while (input.Length > 8)
                        {
                            input = this.stripConsinentsFromStart(input);
                            input = this.stripVowelsFromStart(input);
                        }

                    if (bVowel)
                    {
                        input = this.vowels[RandomIntHelper.Next(this.vowels.Count)] + input;
                    }
                    else
                    {
                        if (RandomIntHelper.Next(2) == 0)
                        {
                            input = this.cons[RandomIntHelper.Next(this.cons.Count)] + input;
                        }
                    }

                    if (input.Length == 0)
                    {
                        return orig;
                    }

                    char firstLetter = char.ToUpper(input[0]);
                    input = firstLetter + input.Substring(1);
                }

                output += input + " ";
            }

            return output.Trim();
        }

        private bool EndsWithVowel(string input)
        {
            for (int n = 0; n < this.vowels.Count; n++)
            {
                var vowel = this.vowels[n];
                if (input.EndsWith(vowel))
                {
                    return true;
                }
            }

            return false;
        }

    public string ModifyName(string input)
        {
            string[] str = input.Split(' ');
            if (str.Length > 1)
            {
                input = str[str.Length - 1];
            }

            switch (RandomIntHelper.Next(2))
            {
                case 0:
                    input = this.changeRandomVowel(input);
                    break;
                case 1:
                    input = this.changeFirstLetter(input);
                    break;
            }

            return this.Capitalize(input);
        }

      static List<string> choices = new List<string>();

        private string changeRandomVowel(string input)
        {
            choices.Clear();
            for (int n = 0; n < this.vowels.Count; n++)
            {
                var vowel = this.vowels[n];
                if (input.Contains(vowel))
                {
                    choices.Add(vowel);
                }
            }

            if (choices.Count == 0)
            {
                return input;
            }

            var vowelc = choices[RandomIntHelper.Next(choices.Count)];
            //foreach (var vowel in choices)
            {
                input = input.Replace(vowelc, this.vowels[RandomIntHelper.Next(vowelc.Length)]);
                return input;
            }

            return input;
        }

        public string Capitalize(string toCap)
        {
            if (string.IsNullOrEmpty(toCap))
            {
                return "";
            }

            toCap = toCap.ToLower().Trim();

            int cons = 0;
            int vow = 0;
            string f = this.firstLetters[RandomIntHelper.Next(this.firstLetters.Count)];

            if(this.vowels.Contains(f))
            {
                toCap = f + this.stripVowelsFromStart(toCap);
            }
            else
            {
                toCap = f + this.stripConsinentsFromStart(toCap);
            }

            for (int n = 0; n < toCap.Length; n++)
            {
                if (toCap[n] == ' ' || toCap[n] == '-')
                {
                    continue;
                }

                if (this.vowels.Contains(toCap[n].ToString()) && toCap[n].ToString() != "y")
                {
                    vow++;
                    cons = 0;
                }
                else
                {
                    cons++;
                    vow = 0;
                }

                if (cons > 2)
                {
                    toCap = toCap.Substring(0, n) + toCap.Substring(n + 1);
                    cons--;
                }

                if (vow > 2)
                {
                    toCap = toCap.Substring(0, n) + toCap.Substring(n + 1);
                    vow--;
                }
            }

            string[] split = toCap.Split(new char[] {' ', '-'});

            string outp = "";

            if (split.Length == 1)
            {
                string ss = split[0].Trim();
                char firstLetter = char.ToUpper(ss[0]);
                ss = firstLetter + ss.Substring(1);

                return ss;
            }

            foreach (var s in split)
            {
                string ss = s;
                if (ss.Length == 0)
                {
                    continue;
                }

                char firstLetter = char.ToUpper(ss[0]);
                ss = firstLetter + ss.Substring(1);

                outp += ss;
                if (outp.Length < toCap.Length)
                {
                    if (toCap[outp.Length] == ' ')
                    {
                        outp += " ";
                    }
                    else
                    {
                        outp += "-";
                    }
                }
            }

            return outp.Trim();
        }

        private string changeRandomLetter(string input)
        {
            {
                int n = RandomIntHelper.Next(input.Length);

                input = input.Substring(0, n) + this.CommonStartNames[RandomIntHelper.Next(this.CommonStartNames.Count)] + input.Substring(n + 1);
            }

            return input;
        }

        private bool StartsWithVowel(string input)
        {
            for (int n = 0; n < this.vowels.Count; n++)
            {
                var vowel = this.vowels[n];
                if (input.StartsWith(vowel.ToUpper()) || input.StartsWith(vowel))
                {
                    return true;
                }
            }

            return false;
        }

        public string stripConsinantsFromEnd(string input)
        {
            for (int x = 0; x < input.Length; x++)
            {
                bool bDoIt = true;
                for (int n = 0; n < this.vowels.Count; n++)
                {
                    var vowel = this.vowels[n];
                    if (input.EndsWith(vowel))
                    {
                        n = 100;
                        bDoIt = false;
                        continue;
                    }
                }

                if (bDoIt)
                {
                    input = input.Substring(0, input.Length - 1);
                    return input;
                }
                else
                {
                    break;
                }
            }


            return input;
        }

        public string stripVowelsFromEnd(string input)
        {
            for (int n = 0; n < this.vowels.Count; n++)
            {
                var vowel = this.vowels[n];
                if (input.EndsWith(vowel))
                {
                    if (input.Length - vowel.Length > 0)
                    {
                        input = input.Substring(0, input.Length - vowel.Length);
                        n = -1;
                    }
                }
            }

            return input;
        }

        public string stripVowelsFromStart(string input)
        {
            for (int n = 0; n < this.vowels.Count; n++)
            {
                var vowel = this.vowels[n];
                if (input.StartsWith(vowel.ToUpper()) || input.StartsWith(vowel))
                {
                    input = input.Substring(1);
                    n = -1;
                }
            }

            return input;
        }

        public string stripConsinentsFromStart(string input)
        {
            for (int x = 0; x < input.Length; x++)
            {
                bool bDoIt = true;
                for (int n = 0; n < this.vowels.Count; n++)
                {
                    var vowel = this.vowels[n];
                    if (input.StartsWith(vowel.ToUpper()) || input.StartsWith(vowel))
                    {
                        n = 100;
                        bDoIt = false;
                        continue;
                    }
                }

                if (bDoIt)
                {
                    input = input.Substring(1);
                    x = -1;
                }
                else
                {
                    break;
                }
            }

            return input;
        }

        public string GetStart(string str, int min)
        {
            bool bDone = false;
            while (!bDone)
            {
                string last = str;

                if (this.EndsWithVowel(str))
                {
                    str = this.stripVowelsFromEnd(str);
                }
                else
                {
                    str = this.stripConsinantsFromEnd(str);
                    str = this.stripVowelsFromEnd(str);
                }

                if (str.Length < min)
                {
                    str = last;
                    bDone = true;
                }

                if (str == last)
                {
                    bDone = true;
                }
            }

            return str;
        }

        public string GetEnd(string str, int min)
        {
            string orig = str;
            bool bDone = false;
            while (!bDone)
            {
                string last = str;

                if (this.StartsWithVowel(str))
                {
                    str = this.stripVowelsFromStart(str);
                    str = this.stripConsinentsFromStart(str);
                }
                else
                {
                    str = this.stripConsinentsFromStart(str);
                }

                if (str.Length < min)
                {
                    str = last;
                    bDone = true;
                }
            }

            if (str == orig)
            {
                return "";
            }

            return str;
        }

        public string GetMiddle(string str, int min)
        {
            bool bDone = false;
            while (!bDone)
            {
                string last = str;

                if (this.EndsWithVowel(str))
                {
                    str = this.stripVowelsFromEnd(str);
                }
                else
                {
                    str = this.stripConsinantsFromEnd(str);
                }

                if (this.StartsWithVowel(str))
                {
                    str = this.stripVowelsFromStart(str);
                }
                else
                {
                    str = this.stripConsinentsFromStart(str);
                }

                if (str.Length < min)
                {
                    str = last;
                    bDone = true;
                }
            }

            return str;
        }

        public void Cannibalize(string str)
        {
            int min = (str.Length/2)-1;
            if (min < 3)
            {
                min = 3;
            }

            string start = this.GetStart(str, min-1);
            if (start.Trim().Length > 0 && !this.CommonStartNames.Contains(start))
            {
                this.CommonStartNames.Add(start);
            }

            string end = this.GetEnd(str, min);

            if (end.Length > 5)
            {
                string mid = end;
                mid = this.GetStart(mid, end.Length/3);
                end = end.Substring(mid.Length);
            }

            if (end.Trim().Length > 0 && !this.CommonEndNames.Contains(end))
            {
                this.CommonEndNames.Add(end);
            }
        }

        public string GetMaleCharacterName()
        {
            string word = this.ConstructWord(5, 10);
           return word;
        }

        public string ConstructWord(float min, float max)
        {
            string str = "";
            int timeout = 100;
            do
            {
                timeout--;
                string start = this.GetCommonStartName();
                string mid = this.GetCommonMiddleName();
                string end = this.GetCommonEndName();

                if (this.EndsWithVowel(start) && this.StartsWithVowel(mid))
                {
                    mid = this.stripVowelsFromStart(mid);
                }

                if (!this.EndsWithVowel(start) && !this.StartsWithVowel(mid))
                {
                    mid = this.stripConsinentsFromStart(mid);
                }

                int tot = start.Length + mid.Length + end.Length;

                if (tot > max)
                {
                    if (start.Length + end.Length < max && start.Length + end.Length > min)
                    {
                        if (this.EndsWithVowel(start) && this.StartsWithVowel(end))
                        {
                            end = this.stripVowelsFromStart(end);
                        }

                        if (!this.EndsWithVowel(start) && !this.StartsWithVowel(end))
                        {
                            end = this.stripConsinentsFromStart(end);
                        }
                    }
                    else while (!(start.Length + end.Length < max && start.Length + end.Length >= min) && start.Length > 0 && end.Length > 0)
                    {
                        int lastStartLength = start.Length;
                        if (start.Length > end.Length)
                        {
                            if (this.EndsWithVowel(start))
                            {
                                start = this.stripVowelsFromEnd(start);
                            }
                            else
                            {
                                start = this.stripConsinantsFromEnd(start);
                            }
                        }
                        else
                        {
                            if (this.StartsWithVowel(end))
                            {
                                end = this.stripVowelsFromStart(end);
                            }
                            else
                            {
                                end = this.stripConsinentsFromStart(end);
                            }
                        }

                        if (start.Length == lastStartLength)
                            {
                                break;
                            }
                        }

                    str = start  + end;
                }
                else
                {
                    if (this.EndsWithVowel(mid) && this.StartsWithVowel(end))
                    {
                        end = this.stripVowelsFromStart(end);
                    }

                    if (!this.EndsWithVowel(mid) && !this.StartsWithVowel(end))
                    {
                        end = this.stripConsinentsFromStart(end);
                    }

                    str = start + mid + end;
                }
            } while ((str.Length < min || str.Length >= max) && timeout>0);

            str = str.Replace('"'.ToString(), "");

            if (str.Length > min+2)
            {
                int rem = RandomIntHelper.Next(str.Length - (int)min);

                while (rem > 0)
                {
                    if (RandomIntHelper.Next(2) == 0)
                    {
                        if (this.StartsWithVowel(str))
                        {
                            str = this.stripVowelsFromStart(str);
                        }
                        else
                        {
                            str = this.stripConsinentsFromStart(str);
                        }
                    }
                    else
                    {
                        if (this.StartsWithVowel(str))
                        {
                            str = this.stripVowelsFromEnd(str);
                        }
                        else
                        {
                            str = this.stripConsinantsFromEnd(str);
                        }
                    }

                    rem--;
                }
            }

            if (str.Trim().Length <= 1)
            {
                var w = this.ConstructWord(min, max);
                return w;
            }

            return this.Capitalize(str);
        }

        private string GetCommonStartName()
        {
            return this.CommonStartNames[RandomIntHelper.Next(this.CommonStartNames.Count)];
        }

        private string GetCommonMiddleName()
        {
            if (this.CommonMiddleNames.Count == 0)
            {
                return "";
            }

            return this.CommonMiddleNames[RandomIntHelper.Next(this.CommonMiddleNames.Count)];
        }

        private string GetCommonEndName()
        {
            return this.CommonEndNames[RandomIntHelper.Next(this.CommonEndNames.Count)];
        }

        public string GetPlaceName()
        {
            if(RandomIntHelper.Next(3)!=0)
            {
                return this.ConstructWord(5, 8);
            }

            if (this.WordFormats.Count == 0)
            {
                for (int n = 0; n < 1; n++)
                {
                    this.WordFormats.Add(CommonWordFormats[RandomIntHelper.Next(CommonWordFormats.Count)]);
                }
            }

            if (this.wordsForLand.Count==0)
            {
                string s = this.ConstructWord(2*this.wordLengthBias, 4*this.wordLengthBias);
                this.wordsForLand.Add(s);
            }

            if (this.placeFormat == null)
            {
                this.placeFormat = placeFormatOptions[RandomIntHelper.Next(placeFormatOptions.Count())];
            }

            string format = this.WordFormats[RandomIntHelper.Next(this.WordFormats.Count)];

            int nWords = 1;

            if (RandomIntHelper.Next(3) == 0)
            {
                nWords++;
            }

            if (RandomIntHelper.Next(3) == 0)
            {
                nWords++;
            }

            if (RandomIntHelper.Next(3) != 0)
            {
                format = "{0}";
                nWords = 1;
            }

            int index = 0;

            if (nWords == 1)
            {
                index = format.IndexOf("{0}") + 3;
                format = format.Substring(0, index);
            }

            if (nWords == 2)
            {
                index = format.IndexOf("{1}") + 3;
                format = format.Substring(0, index);
            }

            string[] strs = new string[nWords];
            for (int n = 0; n < nWords; n++)
            {
                if(nWords==1)
                    strs[n] = this.ConstructWord(5*this.wordLengthBias, 8*this.wordLengthBias);
                else if (nWords == 2)
                {
                    strs[n] = this.ConstructWord(3 * this.wordLengthBias, 5 * this.wordLengthBias);
                } else if (nWords == 3)
                {
                    strs[n] = this.ConstructWord(3 * this.wordLengthBias, 4 * this.wordLengthBias);
                }
            }

            if (nWords == 1)
            {
                var s= this.Capitalize(string.Format(format, strs[0]));
                if (LanguageManager.instance.Get(StarHelper.SafeName(s)) != null)
                {
                    return this.GetPlaceName();
                }

                return s;
            }
            else if (nWords == 2)
            {
                var s = this.Capitalize(string.Format(format, strs[0], strs[1]));
                if (LanguageManager.instance.Get(StarHelper.SafeName(s)) != null)
                {
                    return this.GetPlaceName();
                }

                return s;
            }
            else
            {
                var s = this.Capitalize(string.Format(format, strs[0], strs[1], strs[2]));
                if (LanguageManager.instance.Get(StarHelper.SafeName(s)) != null)
                {
                    return this.GetPlaceName();
                }

                return s;
            }
        }

        public List<string> maleNameBlockSet= new List<string>();
        public List<string> femaleNameBlockSet = new List<string>();

        public string GenMaleName()
        {
            var name = this.ConstructWord(3 * this.wordLengthBias, 8 * this.wordLengthBias);

            name = this.stripVowelsFromEnd(name);
            return name;
        }

        public string GetMaleName()
        {
            if (this.maleNameBlockSet.Count == 0)
            {
                this.maleNameBlockSet = this.GetMaleNameBlockCSV();
            }

            return this.maleNameBlockSet[RandomIntHelper.Next(this.maleNameBlockSet.Count)];
        }

        public string GetUniqueMaleName()
        {
            var name = this.ConstructWord(3 * this.wordLengthBias, 8 * this.wordLengthBias);

            name = this.stripVowelsFromEnd(name);
            if (LanguageManager.instance.Get(StarHelper.SafeName(name)) != null)
            {
                return this.GetUniqueMaleName();
            }

            return name;
        }

        public string GetDynastyName()
        {
            var name = this.ConstructWord(6 * this.wordLengthBias, 8 * this.wordLengthBias);
            if(RandomIntHelper.Next(3)==0)
            {
                this.ConstructWord(6 * this.wordLengthBias, 10 * this.wordLengthBias);
            }

            return name;
        }

        public string GenFemaleName()
        {
            var name = this.ConstructWord(3 * this.wordLengthBias, 8 * this.wordLengthBias);

            name = this.stripConsinantsFromEnd(name);

            return name;
        }

        public string GetFemaleName()
        {
            if (this.femaleNameBlockSet.Count == 0)
            {
                this.femaleNameBlockSet = this.GetFemaleNameBlockCSV();
            }

            return this.femaleNameBlockSet[RandomIntHelper.Next(this.femaleNameBlockSet.Count)];
        }

        public string GetGodName()
        {
            var name = this.ConstructWord(3 * this.wordLengthBias, 5 * this.wordLengthBias);

            if(RandomIntHelper.Next(3)==0)
            {
                name = this.stripConsinantsFromEnd(name);
            }

            if (name.Trim().Length==0)
            {
                return this.GetGodName();
            }

            return name;
        }

        public void ShortenWordCounts()
        {
            int targ = this.CommonStartNames.Count/4;
            int targend = this.CommonEndNames.Count/4;

            targ = Math.Min(20, targ);
            targend = Math.Min(20, targend);

            while (this.CommonStartNames.Count > targ)
            {
                this.CommonStartNames.RemoveAt(RandomIntHelper.Next(this.CommonStartNames.Count));
            }

            while (this.CommonEndNames.Count > targend)
            {
                this.CommonEndNames.RemoveAt(RandomIntHelper.Next(this.CommonEndNames.Count));
            }
        }
    }
}
