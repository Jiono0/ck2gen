// <copyright file="TitleProxy.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.PropertyPageProxies
{
    using System.ComponentModel;
    using System.Drawing;
    using CrusaderKingsStoryGen.Forms;
    using CrusaderKingsStoryGen.Parsers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    class TitleProxy
    {
        private TitleParser title;


        public TitleProxy(TitleParser title)
        {
            this.title = title;
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
                return this.title.color;
            }

            set
            {
                this.title.color = value;
                this.title.color2 = value;
                this.title.SetProperty("color", this.title.color);
                this.title.SetProperty("color2", this.title.color);
                foreach (var provinceParser in this.title.GetAllProvinces())
                {
                    provinceParser.SetProperty("color", this.title.color);
                    provinceParser.SetProperty("color2", this.title.color);
                }
            }
        }

        [Category("Title Details"),
            DisplayName("Name")]

        public string Name
        {
            get
            {
                return this.title.LangName;
            }

            set
            {
                this.title.RenameSoft(value);
                MainForm.instance.RefreshTree(this.title);
            }
        }

        [Category("Title Details"),
                DisplayName("Tag")]

        public string TitleName
        {
            get
            {
                return this.title.Name;
            }
        }

        [Category("Ruler Details"),
            DisplayName("Name")]
        public string LeaderName
        {
            get
            {
                return this.title.Holder.ChrName;
            }

            set { this.title.Holder.ChrName = value; }
        }

        [Category("Ruler Details"),
            DisplayName("Dynasty")]
        public string LeaderDynasty
        {
            get
            {
                return (this.title.Holder.Dynasty.NameScope as ScriptCommand).Value.ToString();
            }

            set { this.title.Holder.Dynasty.Name = value; }
        }

        [Category("Liege Details"),
              DisplayName("Liege")]
        public string Liege
        {
            get
            {
                if (this.title.Liege != null)
                {
                    return this.title.Liege.Name;
                }

                return "<Independent>";
            }
        }
    }
}
