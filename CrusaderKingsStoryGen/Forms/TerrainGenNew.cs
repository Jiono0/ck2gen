// <copyright file="TerrainGenNew.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Forms
{
    using System;
    using System.Windows.Forms;
    using CrusaderKingsStoryGen.MapGen;

    public partial class TerrainGenNew : Form
    {
        public TerrainMap noise = new TerrainMap();

        public TerrainGenNew()
        {
            this.InitializeComponent();
            this.noise.Init(new Random().Next(100000), 3072/6, 2048/6);
            //preview.Image = noise.ResultBitmap2;
            this.pictureBox1.Image = this.noise.ResultBitmap;
        }
    }
}
