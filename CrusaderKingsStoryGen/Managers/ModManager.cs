// <copyright file="ModManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class ModManager
    {
        public static ModManager instance = new ModManager();

        public Dictionary<string, string> FileMap = new Dictionary<string, string>();
        public List<Mod> Mods = new List<Mod>();
        private string rootDir;

        public void LoadVanilla()
        {
            this.Mods.Clear();
            Mod mod = new Mod();
            mod.name = "vanilla";
            this.Mods.Add(mod);
            this.rootDir = Globals.GameDir;
            this.LoadDir(Globals.GameDir, mod);
        }

        public void Load(string modFile)
        {
            Mod mod = new Mod();
            mod.name = modFile.Substring(modFile.LastIndexOf("\\")+1).Replace(".mod", "").Trim();
            this.Mods.Add(mod);
            string usePath = "";
            using (System.IO.StreamReader file =
                new System.IO.StreamReader(modFile, Encoding.GetEncoding(1252)))
            {
                string line = "";


                while ((line = file.ReadLine()) != null)
                {
                    if (line.Trim().StartsWith("replace_path"))
                    {
                        string path = line.Substring(line.IndexOf('=') + 1).Replace("\"", "").Replace("/", "\\").Trim();
                        mod.replaceDirs.Add(path);
                        var toRemove = this.FileMap.Where(o => o.Key.StartsWith(path)).ToList();

                        toRemove.ForEach(p => this.FileMap.Remove(p.Key));
                    }

                    if (line.Trim().StartsWith("path"))
                    {
                        string path = line.Substring(line.IndexOf('=') + 1).Replace("\"", "").Replace("/", "\\").Trim();
                        usePath = path;
                    }

                    if (line.Trim().StartsWith("name"))
                    {
                        string path = line.Substring(line.IndexOf('=') + 1).Replace("\"", "").Replace("/", "\\").Trim();
                        if (path.Contains("#"))
                        {
                            path = path.Split('#')[0].Trim();
                        }

                        mod.name = path;
                    }
                }
            }

            this.rootDir = modFile.Substring(0, modFile.LastIndexOf("\\") + 1) + usePath.Replace("mod\\", "")+ "\\";
            this.LoadDir(this.rootDir, mod);
        }

        public string[] GetFiles(string path)
        {
            var list = this.FileMap.Where(f => f.Key.StartsWith(path)).ToList();
            List<string> str = new List<string>();
            list.ForEach(l => str.Add(l.Value));

            return str.ToArray();
        }

        public string[] GetFileKeys(string path)
        {
            var list = this.FileMap.Where(f => f.Key.StartsWith(path)).ToList();
            List<string> str = new List<string>();
            list.ForEach(l => str.Add(l.Key));

            return str.ToArray();
        }

        public void LoadDir(string dir, Mod mod)
        {
            string[] dirs = Directory.GetDirectories(dir);

            foreach (var s in dirs)
            {
                this.LoadDir(s, mod);
            }

            string[] files = Directory.GetFiles(dir);

            foreach (var file in files)
            {
                string stripped = file.Replace(this.rootDir, "");
                mod.FileMap[stripped] = file;
                this.FileMap[stripped] = file;
            }
        }


        public class Mod
        {
            public string name;
            public List<string> replaceDirs = new List<string>();
            public Dictionary<string, string> FileMap = new Dictionary<string, string>();
        }

        public void LoadMods()
        {
            this.LoadVanilla();
            foreach (var m in this.ModsToLoad)
            {
                this.Load(Globals.ModRootDir + m + ".mod");
            }
        }

        public string GetDependencies()
        {
            string depStr = "";

            foreach (var mod in this.Mods)
            {
                if(mod.name=="vanilla")
                {
                    continue;
                }

                depStr += "\"" + mod.name + "\"" + " ";
            }

            return "dependencies = { " + depStr.Trim() + " }";
        }

        public List<string> ModsToLoad = new List<string>();

        public void AddModsToLoad(string val)
        {
            this.ModsToLoad.Add(val);
        }

        public void Init()
        {
            foreach (var setting in Globals.Settings.Where(k => k.Key.StartsWith("Mod")).OrderBy(p=>p.Key).ToList())
            {
                ModManager.instance.ModsToLoad.Add(setting.Value);
            }
        }

        public bool IsVanilla(string str)
        {
            if (this.FileMap.ContainsKey(str))
            {
                if (this.FileMap[str].StartsWith(Globals.GameDir))
                {
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}
