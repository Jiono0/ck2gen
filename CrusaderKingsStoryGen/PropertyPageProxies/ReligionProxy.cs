// <copyright file="ReligionProxy.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.PropertyPageProxies
{
    using System.ComponentModel;
    using System.Drawing;
    using CrusaderKingsStoryGen.Forms;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Managers;
    using CrusaderKingsStoryGen.Parsers;

    class ReligionProxy
    {
        private ReligionParser religion;

        public ReligionProxy(ReligionParser title)
        {
            this.religion = title;
        }

        [Category("Religion Details"),
         DisplayName("Name")]
        public string Name
        {
            get
            {
                return this.religion.LanguageName;
            }

            set
            {
                this.religion.LanguageName = value;
                LanguageManager.instance.Add(this.religion.Scope.Name, value);

                MainForm.instance.RefreshTree();
            }
        }

        void test()
        {
        }

        [Category("Religion Head"),
         DisplayName("Head Name")]
        public string PopeName
        {
            get { return this.religion.PopeName.Lang(); }

            set
            {
                this.religion.PopeName = value.AddSafe();
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Religion Head"),
              DisplayName("Has Head")]
        public bool HasHead
        {
            get { return this.religion.hasLeader; }

            set
            {
                this.religion.hasLeader = value;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Religion Traditions"),
                    DisplayName("Heir Designation")]
        public bool HeirDes
        {
            get { return this.religion.has_heir_designation; }

            set
            {
                this.religion.has_heir_designation = value;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Religion Traditions"),
                    DisplayName("Concubines")]
        public int Concubines
        {
            get { return this.religion.max_consorts; }

            set
            {
                this.religion.max_consorts = value;
                this.religion.max_wives = 1;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Marriage Rules"),
           DisplayName("Max Wives")]
        public int Wives
        {
            get { return this.religion.max_wives; }

            set
            {
                this.religion.max_wives = value;
                this.religion.max_consorts = 0;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Marriage Rules"),
               DisplayName("Sibling Marriage")]
        public bool SiblingMarriage
        {
            get { return this.religion.bs_marriage; }

            set
            {
                this.religion.bs_marriage = value;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Marriage Rules"),
                DisplayName("Parent Child Marriage")]
        public bool ParentChildMarriage
        {
            get { return this.religion.pc_marriage; }

            set
            {
                this.religion.pc_marriage = value;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Marriage Rules"),
                    DisplayName("Uncle-Niece/Aunty-Nephew Marriage")]
        public bool UncleMarriage
        {
            get { return !this.religion.psc_marriage; }

            set
            {
                this.religion.psc_marriage = !value;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Marriage Rules"),
                            DisplayName("Cousin Marriage")]
        public bool CousinMarriage
        {
            get { return !this.religion.cousin_marriage; }

            set
            {
                this.religion.cousin_marriage = !value;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Marriage Rules"),
        DisplayName("Matrilineal Marriage")]
        public bool Matrilineal
        {
            get { return !this.religion.matrilineal_marriages; }

            set
            {
                this.religion.matrilineal_marriages = !value;
                this.religion.ScopeReligionDetails();
            }
        }

        [Category("Marriage Rules"),
             DisplayName("Religious Intermarrying")]
        public bool Intermarry
        {
            get { return !this.religion.intermarry; }

            set
            {
                this.religion.intermarry = !value;
                this.religion.ScopeReligionDetails();
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
                return Color.FromArgb(255, this.religion.r, this.religion.g, this.religion.b);
            }

            set
            {
                this.religion.r = value.R;
                this.religion.g = value.G;
                this.religion.b = value.B;
            }
        }
    }
}
