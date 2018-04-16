// <copyright file="NicknameManager.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Managers
{
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.Parsers;
    using CrusaderKingsStoryGen.ScriptHelpers;
    using System.Collections.Generic;

    class NicknameManager
    {
        public static NicknameManager instance = new NicknameManager();

        private bool init = false;
        List<string> nicks = new List<string>();

        public void Init()
        {
            Script s = ScriptLoader.instance.Load(Globals.GameDir + "common\\nicknames\\00_nicknames.txt");

            foreach (var o in s.Root.ChildrenMap)
            {
                string name = o.Key;
                this.nicks.Add(name);
                this.init = true;
            }

            this.nicks.Remove("nick_the_master_of_hungary");
            this.nicks.Remove("nick_the_master_of_hungary");
        }

        public string getNick(CharacterParser chr)
        {
            if (!this.init)
            {
                this.Init();
            }

            return this.nicks[RandomIntHelper.Next(this.nicks.Count)];
        }
    }
}
