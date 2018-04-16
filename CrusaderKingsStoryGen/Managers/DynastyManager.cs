// <copyright file="DynastyManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using CrusaderKingsStoryGen.Parsers;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.ScriptHelpers;

    public class Dynasty
    {
        public int ID;
        public List<CharacterParser> Members = new List<CharacterParser>();
        public ScriptScope Scope;
        public TitleParser palace;
        private string _name;

        public Color Color { get; set; }

        public string Name
        {
            get { return this._name; }

            set
            {
                this._name = value;
                (this.NameScope as ScriptCommand).Value = value;
            }
        }

        public ScriptCommand NameScope { get; set; }
    }

    class DynastyManager
    {
        public static DynastyManager instance = new DynastyManager();
        public int ID = 1;

        public void Init()
        {
            Script s = new Script();
            this.script = s;
            s.Name = Globals.ModDir + "common\\dynasties\\dynasties.txt";
            s.Root = new ScriptScope();
        }

        public Dictionary<int, Dynasty> DynastyMap = new Dictionary<int, Dynasty>();

        public void Save()
        {
            this.script.Save();
        }

        public Dynasty GetDynasty(int id, string name, string culture, string religion, ScriptScope scope)
        {
            this.ID = id;
            this.Name = name;
            var nameScope = scope.ChildrenMap["name"] as ScriptCommand;

            this.script.Root.Add(scope);
            var d = new Dynasty() { ID = this.ID, Scope = scope, NameScope = nameScope };
            d.Color = Color.FromArgb(255, RandomIntHelper.Next(200) + 55, RandomIntHelper.Next(200) + 55, RandomIntHelper.Next(200) + 55);
            this.DynastyMap[this.ID] = d;
            return d;
        }

        public Dynasty GetDynasty(CultureParser culture)
        {
            ScriptScope scope = new ScriptScope();
            scope.Name = this.ID.ToString();
            this.ID++;
            do
            {
                this.Name = culture.dna.GetDynastyName();
            } while (this.Name == null || this.Name.Trim().Length==0);

            var nameScope = new ScriptCommand("name", this.Name, scope);

            scope.Add(nameScope);
            scope.Add(new ScriptCommand("culture", culture.Name, scope));
            this.script.Root.Add(scope);
            var d = new Dynasty() {ID = this.ID - 1, Scope = scope, NameScope = nameScope};
            d.Color = Color.FromArgb(255, RandomIntHelper.Next(200) + 55, RandomIntHelper.Next(200) + 55, RandomIntHelper.Next(200) + 55);
            this.DynastyMap[this.ID - 1] = d;
            culture.Dynasties.Add(d);
            return d;
        }

        public string Name { get; set; }

        public Script script { get; set; }

        public void LoadVanilla()
        {
            var files = ModManager.instance.GetFileKeys("common\\dynasties");
            foreach (var file in files)
            {
                Script s = ScriptLoader.instance.LoadKey(file);
                foreach (var rootChild in s.Root.Children)
                {
                    var scope = rootChild as ScriptScope;

                    int id = Convert.ToInt32(scope.Name);

                    string name = scope.GetString("name");
                    string culture = scope.GetString("culture");
                    string religion = scope.GetString("religion");
                    Dynasty d = this.GetDynasty(id, name, culture, religion, scope);
                }
            }
        }
    }
}
