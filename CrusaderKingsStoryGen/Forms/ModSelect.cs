// <copyright file="ModSelect.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;

    public partial class ModSelect : Form
    {
        public ModSelect()
        {
            this.InitializeComponent();

            string[] mods = Directory.GetFiles(Globals.ModRootDir);

            List<string> modList = new List<string>(mods);

            modList = modList.Where(m => m.EndsWith(".mod")).ToList();
            var used = Globals.Settings.Where(k => k.Key.StartsWith("Mod")).ToList();

            foreach (var m in modList)
            {
                string modN = m.Substring(m.LastIndexOf("\\") + 1).Replace(".mod", "");
                bool found = false;
                foreach (var keyValuePair in used)
                {
                    if (keyValuePair.Value == modN)
                    {
                        this.activeMods.Items.Add(modN);
                        found = true;
                    }
                }

                if (!found)
                {
                    this.inactiveMods.Items.Add(modN);
                }
            }
        }

        private void inactiveMods_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void inactiveMods_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                this.Add(this.inactiveMods.SelectedItem.ToString());
            }
            catch (NullReferenceException ex)
            {
                // Silently fail to allow continued selection of mods
                return;
            }
        }

        public void Add(string str)
        {
            this.inactiveMods.Items.Remove(str);
            this.activeMods.Items.Add(str);
        }

        public void Remove(string str)
        {
            this.activeMods.Items.Remove(str);
            this.inactiveMods.Items.Add(str);
        }

        private void add_Click(object sender, EventArgs e)
        {
            this.Add(this.inactiveMods.SelectedItem.ToString());
        }

        private void remove_Click(object sender, EventArgs e)
        {
            this.Remove(this.activeMods.SelectedItem.ToString());
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            this.Remove(this.activeMods.SelectedItem.ToString());
        }

        private void moveUp_Click(object sender, EventArgs e)
        {
            if (this.activeMods.SelectedItem != null)
            {
                var item = this.activeMods.SelectedItem;
                int pos = this.activeMods.SelectedIndex;
                if (pos == 0)
                {
                    return;
                }

                this.activeMods.Items.Remove(this.activeMods.SelectedItem);

                this.activeMods.Items.Insert(pos - 1, item.ToString());
                this.activeMods.SelectedItem = item;
            }
        }

        private void moveDown_Click(object sender, EventArgs e)
        {
            if (this.activeMods.SelectedItem != null)
            {
                var item = this.activeMods.SelectedItem;
                int pos = this.activeMods.SelectedIndex;
                if (pos == this.activeMods.Items.Count - 1)
                {
                    return;
                }

                this.activeMods.Items.Remove(this.activeMods.SelectedItem);

                this.activeMods.Items.Insert(pos + 1, item.ToString());
                this.activeMods.SelectedItem = item;
            }
        }
    }
}
