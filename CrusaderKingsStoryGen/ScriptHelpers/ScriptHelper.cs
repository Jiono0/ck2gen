using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrusaderKingsStoryGen.ScriptHelpers
{
    class Script
    {
        public string Name { get; set; }

        public ScriptScope Root { get; set; }

        public override string ToString()
        {
            return this.Name;
        }

        public void Save()
        {
            Directory.CreateDirectory(Globals.ModDir);
            Directory.CreateDirectory(Globals.ModDir + "common\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\cultures\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\governments\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\dynasties\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\disease\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\landed_titles\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\province_setup\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\religions\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\religious_titles\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\scripted_triggers\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\on_actions\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\societies\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\trade_routes\\");
            Directory.CreateDirectory(Globals.ModDir + "common\\traits\\");
            Directory.CreateDirectory(Globals.ModDir + "history\\");
            Directory.CreateDirectory(Globals.ModDir + "history\\characters\\");
            Directory.CreateDirectory(Globals.ModDir + "history\\provinces\\");
            Directory.CreateDirectory(Globals.ModDir + "history\\titles\\");
            Directory.CreateDirectory(Globals.ModDir + "history\\wars\\");
            Directory.CreateDirectory(Globals.ModDir + "decisions\\");
            Directory.CreateDirectory(Globals.ModDir + "events\\");
            Directory.CreateDirectory(Globals.ModDir + "gfx\\");
            Directory.CreateDirectory(Globals.ModDir + "gfx\\traits\\");
            Directory.CreateDirectory(Globals.ModDir + "gfx\\flags\\");
            Directory.CreateDirectory(Globals.ModDir + "interface\\");
            Directory.CreateDirectory(Globals.ModDir + "localisation\\");
            Directory.CreateDirectory(Globals.ModDir + "map\\");


            var filename = ConvertFileName(this.Name, this.filenameIsKey);
            try
            {
                using (System.IO.StreamWriter file =
              new System.IO.StreamWriter(filename, false, Encoding.GetEncoding(1252)))
                {
                    this.Root.Save(file, 0);

                    file.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Failed to save " + filename, "Error");
                throw;
            }
        }

        internal bool filenameIsKey = false;

        public static string ConvertFileName(string filename, bool isKey = false)
        {
            if (!isKey)
            {
                filename = filename.Replace(Globals.GameDir, "");
                filename = filename.Replace("storygen\\", Globals.ModName + "\\");
                filename = filename.Replace(Globals.MapDir, "");
                filename = filename.Replace(Directory.GetCurrentDirectory() + "\\data\\decisiontemplates\\", "");
                filename = filename.Replace(Globals.ModDir, "");
                filename = filename.Replace(Globals.OModDir, "");
            }

            filename = Globals.ModDir + filename;

            filename = filename.Replace("\\", "/");
            return filename;
        }
    }
}
