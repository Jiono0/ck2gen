// <copyright file="SpriteManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.ScriptHelpers;
    using System.IO;

    internal class SpriteManager
    {
        public static SpriteManager instance = new SpriteManager();
        public Script script = new Script();

        public ScriptScope spriteTypes = new ScriptScope();

        public void Init()
        {
            if (!Directory.Exists(Globals.ModDir + "interface"))
            {
                Directory.CreateDirectory(Globals.ModDir + "interface");
            }

            var files = Directory.GetFiles(Globals.ModDir + "interface");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            this.script.Root = new ScriptScope();
            var s = new ScriptScope();
            s.Name = "spriteTypes";
            this.spriteTypes = s;
            this.script.Root.Add(s);
            this.script.Name = Globals.ModDir + "interface\\genGraphics.gfx";
        }

        public void AddTraitSprite(string name, string relFilename)
        {
            var scope = new ScriptScope();

            scope.Name = "spriteType";

            scope.Do(@"
                	        name = ""GFX_trait_" + name + @"
		                    texturefile = " + relFilename + @"
		                    noOfFrames = 1
		                    norefcount = yes
		                    effectFile = ""gfx/FX/buttonstate.lua""");

            this.spriteTypes.Add(scope);
        }

        public void Save()
        {
            this.script.Save();
        }

        public void AddGovernment(GovermentPolicyHelper government)
        {
            var scope = new ScriptScope();

            scope.Name = "spriteType";

            scope.Do(@"
                	        name = ""GFX_icon_" + government.name + @"
		                    texturefile = gfx\\interface\\government_icon_" + government.name + ".dds");

            this.spriteTypes.Add(scope);
        }
    }
}
