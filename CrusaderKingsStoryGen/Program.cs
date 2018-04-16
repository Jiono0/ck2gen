// <copyright file="Program.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen
{
    using System;
    using System.Windows.Forms;
    using CrusaderKingsStoryGen.Forms;

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //    GeneratedTerrainMap map = new GeneratedTerrainMap();
            //   map.Init(3072, 2048);
            //      return;
         /*   Rand.SetSeed();
            Globals.GameDir = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Crusader Kings II\\";

            CulturalDnaManager.instance.Init();
            CultureManager.instance.Init();

            for (int x = 0; x < 1000; x++)
            {
                String str = CultureManager.instance.AllCultures[0].dna.GetPlaceName();
                System.Console.Out.WriteLine(str);
            }
            */
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
         //   Application.Run(new ScriptBlueprint());
        }
    }
}
