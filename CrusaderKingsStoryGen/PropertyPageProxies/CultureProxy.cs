// <copyright file="CultureProxy.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.PropertyPageProxies
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using CrusaderKingsStoryGen.Forms;
    using CrusaderKingsStoryGen.Parsers;

    class CultureProxy
    {
        private CultureParser culture;

        public CultureProxy(CultureParser title)
        {
            this.culture = title;
        }

        [Category("Culture Details"),
         DisplayName("Name")]
        public string Name
        {
            get
            {
                return this.culture.LanguageName;
            }

            set
            {
                this.culture.LanguageName = value;
                LanguageManager.instance.Add(this.culture.Scope.Name, value);

                MainForm.instance.RefreshTree();
            }
        }

        [Category("Graphical Details"),
        DisplayName("Portrait")]
        public CultureParser.gfxStyles Portrait
        {
            get {
                    CultureParser.gfxStyles v = CultureParser.gfxStyles.africangfx;

                    Enum.TryParse<CultureParser.gfxStyles>(this.culture.dna.portraitPool[0], out v);
                    return v;
                }

            set
            {
                this.culture.dna.portraitPool.Clear();
                this.culture.dna.portraitPool.Add(value.ToString());
                this.culture.ScopeCultureDetails();
            }
        }

        [Category("Character Names"),
         DisplayName("Male Names")]
        [Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
            "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(CsvConverter))]

        public List<string> MaleNames
        {
            get { return this.culture.dna.maleNameBlockSet; }
        }

        [Category("Character Names"),
         DisplayName("Female Names")]
        [Editor(@"System.Windows.Forms.Design.StringCollectionEditor," +
            "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(CsvConverter))]
        public List<string> FemaleNames
        {
            get { return this.culture.dna.femaleNameBlockSet; }
        }

        [Category("Title Language"),
        DisplayName("Emperor Title")]
        public string EmperorTitle
        {
            get
            {
                return this.culture.dna.empTitle.Lang();
            }

            set
            {
                this.culture.dna.empTitle = value.AddSafe();

                MainForm.instance.RefreshTree();
            }
        }

        [Category("Appearance"),
          DisplayName("Color")]
        [Editor(@"CrusaderKingsStoryGen.PropertyPageProxies.MyColorEditor",
              typeof(System.Drawing.Design.UITypeEditor)),
               TypeConverter(typeof(MyColorConverter))]
        public Color Color
        {
            get
            {
                return Color.FromArgb(255, this.culture.r, this.culture.g, this.culture.b);
            }

            set
            {
                this.culture.r = value.R;
                this.culture.g = value.G;
                this.culture.b = value.B;
            }
        }

        [Category("Title Language"),
        DisplayName("King Title")]
        public string KingTitle
        {
            get
            {
                return this.culture.dna.kingTitle.Lang();
            }

            set
            {
                this.culture.dna.kingTitle = value.AddSafe();

                MainForm.instance.RefreshTree();
            }
        }

        [Category("Title Language"),
             DisplayName("Duke Title")]
        public string DukeTitle
        {
            get
            {
                return this.culture.dna.dukeTitle.Lang();
            }

            set
            {
                this.culture.dna.dukeTitle = value.AddSafe();


                MainForm.instance.RefreshTree();
            }
        }

        [Category("Title Language"),
                   DisplayName("Count Title")]
        public string CountTitle
        {
            get
            {
                return this.culture.dna.countTitle.Lang();
            }

            set
            {
                this.culture.dna.countTitle= value.AddSafe();
                MainForm.instance.RefreshTree();
            }
        }

        [Category("Title Language"),
                      DisplayName("Baron Title")]
        public string BaronTitle
        {
            get
            {
                return this.culture.dna.baronTitle.Lang();
            }

            set
            {
                this.culture.dna.baronTitle= value.AddSafe();

                MainForm.instance.RefreshTree();
            }
        }

        [Category("Title Language"),
                          DisplayName("Mayor Title")]
        public string MayorTitle
        {
            get
            {
                return this.culture.dna.mayorTitle.Lang();
            }

            set
            {
                this.culture.dna.mayorTitle = value.AddSafe();

                MainForm.instance.RefreshTree();
            }
        }
    }
}
