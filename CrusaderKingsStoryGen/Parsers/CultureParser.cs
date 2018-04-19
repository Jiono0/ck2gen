namespace CrusaderKingsStoryGen.Parsers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class CultureParser : Parser
    {
        public CultureGroupParser Group
        {
            get { return this.group; }
            set { this.group = value; }
        }

        public string LanguageName { get; set; }

        public KingdomHelper dna { get; set; }

        public List<Dynasty> Dynasties = new List<Dynasty>();
        private bool dirty = true;
        private Rectangle _bounds = new Rectangle();

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

        public CultureParser(ScriptScope scope, CultureGroupParser group) : base(scope)
        {
            this.group = group;
        }

        public void Init()
        {
            if (this.Name == "kedaras")
            {
            }

            //        Scope.Clear();

            string fx = this.Group.chosenGfx;
            if (this.Group.chosenGfx == null)
            {
                fx = this.Group.Scope.Scopes[0].Data;
            }

            int r = RandomIntHelper.Next(255);
            int g = RandomIntHelper.Next(255);
            int b = RandomIntHelper.Next(255);

            r = this.Group.r;
            g = this.Group.g;
            b = this.Group.b;
            switch (RandomIntHelper.Next(3))
            {
                case 0:
                    r += RandomIntHelper.Next(-45, 45);
                    g += RandomIntHelper.Next(-25, 25);
                    b += RandomIntHelper.Next(-15, 15);

                    break;

                case 1:
                    g += RandomIntHelper.Next(-45, 45);
                    r += RandomIntHelper.Next(-25, 25);
                    b += RandomIntHelper.Next(-15, 15);

                    break;

                case 2:
                    b += RandomIntHelper.Next(-45, 45);
                    g += RandomIntHelper.Next(-25, 25);
                    r += RandomIntHelper.Next(-15, 15);

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

            this.Scope.Do(@"

		         color = { " + r + " " + g + " " + b + @" }
        ");
        }

        public void DoDetailsForCulture()
        {
            dna.culture = this;
            if (this.dna.portraitPool.Count == 0)
            {
                int c = 1;
                string cul = GetRandomCultureGraphics();
                for (int i = 0; i < c; i++)
                {
                    this.dna.portraitPool.Add(cul);
                    cul = this.GetRelatedCultureGfx(cul);
                }
            }

            string portrait = "";

            foreach (var p in this.dna.portraitPool)
            {
                portrait += p + " ";
            }

            int r = RandomIntHelper.Next(255);
            int g = RandomIntHelper.Next(255);
            int b = RandomIntHelper.Next(255);

            r = this.Group.r;
            g = this.Group.g;
            b = this.Group.b;
            switch (RandomIntHelper.Next(3))
            {
                case 0:
                    r += RandomIntHelper.Next(-45, 45);
                    g += RandomIntHelper.Next(-25, 25);
                    b += RandomIntHelper.Next(-15, 15);

                    break;

                case 1:
                    g += RandomIntHelper.Next(-45, 45);
                    r += RandomIntHelper.Next(-25, 25);
                    b += RandomIntHelper.Next(-15, 15);

                    break;

                case 2:
                    b += RandomIntHelper.Next(-45, 45);
                    g += RandomIntHelper.Next(-25, 25);
                    r += RandomIntHelper.Next(-15, 15);

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

            this.r = r;
            this.g = g;
            this.b = b;

            this.ScopeCultureDetails();
        }

        public int r = 0;
        public int g = 0;
        public int b = 0;

        public void ScopeCultureDetails()
        {
            List<string> maleNameBlockSet;//= new List<string>();
            List<string> femaleNameBlockSet;//= new List<string>();

            maleNameBlockSet = dna.maleNameBlockSet;
            femaleNameBlockSet = dna.femaleNameBlockSet;

            this.Scope.Clear();
            var portrait = this.dna.portraitPool[0];
            this.Scope.Do(@"

               color = { " + (this.r) + " " + (this.g) + " " + (this.b) + @" }

               graphical_cultures = {
                    " + portrait + @"
                }

		        male_names = {
			        " + string.Join(" ", maleNameBlockSet.ToArray()) + @"
		        }
		        female_names = {
			        " + string.Join(" ", femaleNameBlockSet.ToArray()) + @"
		        }

		        dukes_called_kings =  " + (this.dna.dukes_called_kings ? "yes" : "no") + @"
		        baron_titles_hidden =  " + (this.dna.baron_titles_hidden ? "yes" : "no") + @"
		        count_titles_hidden =  " + (this.dna.count_titles_hidden ? "yes" : "no") + @"
		        horde = " + (this.dna.horde ? "yes" : "no") + @"
                founder_named_dynasties = " + (this.dna.founder_named_dynasties ? "yes" : "no") + @"
                dynasty_title_names = " + (this.dna.dynasty_title_names ? "yes" : "no") + @"

		        from_dynasty_prefix = [" + '"' + this.dna.from_dynasty_prefix + '"' + @"

		        male_patronym = " + (this.dna.male_patronym) + @"
		        female_patronym =  " + (this.dna.female_patronym) + @"
		        prefix =  " + (this.dna.patronym_prefix ? "yes" : "no") + @" # The patronym is added as a suffix
		        # Chance of male children being named after their paternal or maternal grandfather, or their father. Sum must not exceed 100.
		        pat_grf_name_chance = 25
		        mat_grf_name_chance = 0
		        father_name_chance = 25

		        # Chance of female children being named after their paternal or maternal grandmother, or their mother. Sum must not exceed 100.
		        pat_grm_name_chance = 10
		        mat_grm_name_chance = 25
		        mother_name_chance = 25

		        modifier = default_culture_modifier

		        allow_looting =  " + (this.dna.allow_looting ? "yes" : "no") + @"
		        seafarer =  " + (this.dna.seafarer ? "yes" : "no") + @"
        ");
        }

        public string GetRelatedCultureGfx(string cul)
        {
            if (wh.Contains(cul))
            {
                return wh[RandomIntHelper.Next(wh.Count)];
            }
            else if (!wh.Contains(cul))
            {
                return bl[RandomIntHelper.Next(bl.Count)];
            }

            return null;
        }

        public List<string> male_names = new List<string>();
        public List<string> female_names = new List<string>();

        public enum gfxStyles
        {
            norsegfx,
            germangfx,
            frankishgfx,
            westerngfx,
            saxongfx,
            italiangfx,
            southerngfx,
            occitangfx,
            easterngfx,
            byzantinegfx,
            easternslavicgfx,
            westernslavicgfx,
            celticgfx,
            ugricgfx,
            turkishgfx,
            mongolgfx,
            muslimgfx,
            persiangfx,
            cumangfx,
            arabicgfx,
            andalusiangfx,
            africangfx,
            mesoamericangfx,
            indiangfx
        }

        public static string[] gfx =
        {
            "norsegfx", "germangfx", "frankishgfx", "westerngfx", "saxongfx", "italiangfx", "southerngfx", "occitangfx",
            "easterngfx", "byzantinegfx", "easternslavicgfx", "westernslavicgfx",
            "celticgfx", "ugricgfx", "turkishgfx", "mongolgfx", "muslimgfx", "persiangfx", "cumangfx", "arabicgfx",
            "andalusiangfx", "africangfx", "mesoamericangfx", "indiangfx"
        };

        private static List<string> wh = new List<string>()
        {
            "norsegfx",
            "germangfx",
            "frankishgfx",
            "westerngfx",
            "saxongfx",
            "italiangfx",
            "southerngfx",
            "occitangfx",
            "easterngfx",
            "byzantinegfx",
            "easternslavicgfx",
            "westernslavicgfx",
            "celticgfx"
        };

        private static List<string> bl = new List<string>()
        {
            "ugricgfx",
            "turkishgfx",
            "mongolgfx",
            "muslimgfx",
            "persiangfx",
            "cumangfx",
            "arabicgfx",
            "andalusiangfx",
            "africangfx",
            "mesoamericangfx",
            "indiangfx"
        };

        internal CultureGroupParser group;

        public Color color
        {
            get { return Color.FromArgb(255, this.r, this.g, this.b); }

            set
            {
                this.r = value.R;
                this.g = value.G;
                this.b = value.B;
            }
        }

        //   public String government = "tribal";

        internal static string GetRandomCultureGraphics(CultureGroupParser group = null)
        {
            if (group != null)
            {
                if (RandomIntHelper.Next(3) == 0)
                {
                    switch (group.chosenGfx)
                    {
                        case "norsegfx":
                        case "germangfx":
                        case "frankishgfx":
                        case "westerngfx":
                        case "saxongfx":
                        case "italiangfx":
                        case "celticgfx":
                        case "mongolgfx":
                            return wh[RandomIntHelper.Next(gfx.Count())];
                            break;

                        case "ugricgfx":
                        case "turkishgfx":
                        case "muslimgfx":
                        case "persiangfx":
                        case "cumangfx":
                        case "arabicgfx":
                        case "andalusiangfx":
                        case "africangfx":
                        case "mesoamericangfx":
                        case "indiangfx":
                            return bl[RandomIntHelper.Next(gfx.Count())];
                            break;
                    }
                }
                else
                {
                    switch (group.chosenGfx)
                    {
                        case "norsegfx":
                        case "germangfx":
                        case "frankishgfx":
                        case "westerngfx":
                        case "saxongfx":
                        case "italiangfx":
                        case "celticgfx":
                        case "mongolgfx":
                            return bl[RandomIntHelper.Next(gfx.Count())];
                            break;

                        case "ugricgfx":
                        case "turkishgfx":
                        case "muslimgfx":
                        case "persiangfx":
                        case "cumangfx":
                        case "arabicgfx":
                        case "andalusiangfx":
                        case "africangfx":
                        case "mesoamericangfx":
                        case "indiangfx":
                            return wh[RandomIntHelper.Next(gfx.Count())];
                            break;
                    }
                }
            }

            return gfx[RandomIntHelper.Next(gfx.Count())];
        }

        public string PickCharacterName()
        {
            return this.dna.GetMaleName();
        }

        public string PickCharacterName(bool isFemale)
        {
            string str = "";
            do
            {
                str = this.DoPickCharacterName(isFemale);
            } while (str.Trim().Length <= 1);

            return str;
        }

        public string DoPickCharacterName(bool isFemale)
        {
            if (isFemale)
            {
                return this.dna.GetFemaleName();
            }

            return this.dna.GetMaleName();
        }

        public void RemoveProvince(ProvinceParser provinceParser)
        {
            this.Group.RemoveProvince(provinceParser);

            this.Provinces.Remove(provinceParser);
            this.dirty = true;
        }

        public void AddProvince(ProvinceParser provinceParser)
        {
            if (provinceParser.Culture != null)
            {
                provinceParser.Culture.RemoveProvince(provinceParser);
            }

            if (!this.Group.Provinces.Contains(provinceParser))
            {
                this.Group.AddProvince(provinceParser);
            }

            if (!this.Provinces.Contains(provinceParser))
            {
                this.Provinces.Add(provinceParser);
            }

            this.dirty = true;
        }

        public void AddProvinces(List<ProvinceParser> instanceSelectedProvinces)
        {
            foreach (var provinceParser in instanceSelectedProvinces)
            {
                this.AddProvince(provinceParser);
                provinceParser.Culture = this;
            }
        }

        public List<ProvinceParser> Provinces = new List<ProvinceParser>();
        private Point _textPos;
    }
}
