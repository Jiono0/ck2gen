// <copyright file="MapGenerator.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Forms;
    using CrusaderKingsStoryGen.Helpers;
    using CrusaderKingsStoryGen.MapGen;
    using LibNoise.Modfiers;

    public partial class MapGenerator : Form
    {
        GeneratedTerrainMap map;
        int usedSeed = 0;

        public MapGenerator()
        {
            this.InitializeComponent();
            if (this.randomize.Checked)
            {
                RandomIntHelper.SetSeed(new Random().Next(1000000));
                this.seedBox.Value = RandomIntHelper.Next(10000000);
            }

            this.climate.SelectedIndex = Globals.Climate = 0;
            this.land.Checked = true;
            this.mediumBrush.Checked = true;
            //Color.FromArgb(255, 69, 91, 186)
            this.randHigh.Checked = true;
            this.landDrawBitmap = new LockBitmap(new Bitmap(this.preview.Width, this.preview.Height));
            this.mountainDrawBitmap = new LockBitmap(new Bitmap(this.preview.Width, this.preview.Height));
            this.landProxyBitmap = new LockBitmap(new Bitmap(3072 / 2, 2048 / 2));
            this.mountainProxyBitmap = new LockBitmap(new Bitmap(3072 / 2, 2048 / 2));
            var a = new SolidBrush(Color.FromArgb(255, 130, 158, 75));
            var b = new SolidBrush(Color.FromArgb(255, 130 + 40, 158 + 40, 75 + 40));
            var c = new SolidBrush(Color.FromArgb(255, 65, 42, 17));
            var d = new SolidBrush(Color.FromArgb(255, 69, 91, 186));

            using (Graphics gg = Graphics.FromImage(this.landDrawBitmap.Source))
            {
                gg.Clear(Color.FromArgb(255, 69, 91, 186));
            }

            using (Graphics gg = Graphics.FromImage(this.mountainDrawBitmap.Source))
            {
                gg.Clear(Color.Transparent);
            }
        }

        private void generateLandmass_Click(object sender, EventArgs e)
        {
            this.map = new GeneratedTerrainMap();

            if (this.randomize.Checked)
            {
                this.seedBox.Value = RandomIntHelper.Next(10000000);
            }

            int seed = (int) this.seedBox.Value;
            this.usedSeed = seed;
            int w = 3072;
            if (this.mapvlarge.Checked)
            {
                w = 4096;
            }

            if (this.maplarge.Checked)
            {
                w = 3200;
            }

            if (this.mapnorm.Checked)
            {
                w = 3072;
            }

            if (this.mapsmall.Checked)
            {
                w = 2048;
            }

            this.map.Init(w / 10, 2048 / 10, this.usedSeed);
            this.preview.Image = this.map.Map.Source;
            this.exportButton.Enabled = true;
        }

        private void randomize_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void generateDrawn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to generate?", "Are you sure?", MessageBoxButtons.YesNo) !=
                DialogResult.Yes)
            {
                return;
            }

            if (Globals.GameDir.Trim().Length == 0)
            {
                MessageBox.Show(
                    "You need to set the CK2 game directory in the main interface Configuration tab before generating maps",
                    "Error");
                return;
            }

            MainForm.instance.clear();
            if (Globals.MapOutputDir == null)
            {
                FolderBrowserDialog d = new FolderBrowserDialog();
                d.Description = "Choose root folder you would like to store your custom maps.";
                if (d.ShowDialog() == DialogResult.OK)
                {
                    Globals.MapOutputDir = d.SelectedPath;
                }
            }

            Globals.MapName = this.mapOutputDrawn.Text;

            if (!Directory.Exists(Globals.MapOutputTotalDir))
            {
                Directory.CreateDirectory(Globals.MapOutputTotalDir);
            }

            if (!Directory.Exists(Globals.MapOutputTotalDir + "map\\"))
            {
                Directory.CreateDirectory(Globals.MapOutputTotalDir + "map\\");
            }

            //   MapGenManager.instance.Width /= 4;
            //   MapGenManager.instance.Height /= 4;
            this.CopyDir(Directory.GetCurrentDirectory() + "\\data\\mapstuff", Globals.MapOutputTotalDir + "map");
            this.CopyDir(Directory.GetCurrentDirectory() + "\\data\\common", Globals.MapOutputTotalDir + "common");

            float delta = 1.0f;

            if (this.vhigh.Checked)
            {
                delta = 2.5f;
            }

            if (this.high.Checked)
            {
                delta = 1.75f;
            }

            if (this.normal.Checked)
            {
                delta = 1.2f;
            }

            if (this.low.Checked)
            {
                delta = 0.85f;
            }

            if (this.vLow.Checked)
            {
                delta = 0.6f;
            }

            if (this.mapvlarge.Checked)
            {
                MapGenManager.instance.Width = 4096;
            }

            if (this.maplarge.Checked)
            {
                MapGenManager.instance.Width = 3200;
            }

            if (this.mapnorm.Checked)
            {
                MapGenManager.instance.Width = 3072;
            }

            if (this.mapsmall.Checked)
            {
                MapGenManager.instance.Width = 2048;
            }

            //    MapGenManager.instance.Width /= 4;
            //   MapGenManager.instance.Height /= 4;

            LockBitmap lBit = new LockBitmap(this.landBitmapOut);
            LockBitmap mBit = new LockBitmap(this.mountainBitmap);

            MapGenManager.instance.Create(false, this.DrawnSeed, 1500, delta, lBit, mBit);
            lBit.UnlockBits();
            mBit.UnlockBits();
            //  MapGenManager.instance.Create(usedSeed, 1300);
            // MapGenManager.instance.Create(usedSeed, 600);
            LockBitmap waterColorMap =
                new LockBitmap(
                    DevIL.DevIL.LoadBitmap(Directory.GetCurrentDirectory() +
                                           "\\data\\mapstuff\\terrain\\colormap_water.dds"));
            waterColorMap.ResizeImage(MapGenManager.instance.Width, MapGenManager.instance.Height);
            DevIL.DevIL.SaveBitmap(Globals.MapOutputTotalDir + "map\\terrain\\colormap_water.dds", waterColorMap.Source);
            //  LockBitmap normalMap = new LockBitmap(new Bitmap((Directory.GetCurrentDirectory() + "\\data\\mapstuff\\world_normal_height.bmp")));
            //  normalMap.ResizeImage(MapGenManager.instance.Width, MapGenManager.instance.Height);

            //    normalMap.Save24(Globals.MapOutputTotalDir + "map\\world_normal_height.bmp");

            //preview.Image = DevIL.DevIL.LoadBitmap(Globals.MapOutputTotalDir + "map\\terrain\\colormap.dds");
            this.landBitmap = DevIL.DevIL.LoadBitmap(Globals.MapOutputTotalDir + "map\\terrain\\colormap.dds");
            this.preview.Invalidate();
            //   map = null;
            this.exportButton.Enabled = false;

            if (
                MessageBox.Show("Would you like to load this map to generate a world history on?", "Load Map?",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MainForm.instance.SetMap(Globals.MapOutputTotalDir);
                this.map = null;
                ProvinceBitmapManager.instance = new ProvinceBitmapManager();
                MapGenManager.instance = new MapGenManager();
                this.Close();
            }

            ProvinceBitmapManager.instance = new ProvinceBitmapManager();
            MapGenManager.instance = new MapGenManager();
            this.map = null;
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (Globals.GameDir.Trim().Length == 0)
            {
                MessageBox.Show(
                    "You need to set the CK2 game directory in the main interface Configuration tab before generating maps",
                    "Error");
                return;
            }

            MainForm.instance.clear();
            if (Globals.MapOutputDir == null)
            {
                FolderBrowserDialog d = new FolderBrowserDialog();
                d.Description = "Choose root folder you would like to store your custom maps.";
                if (d.ShowDialog() == DialogResult.OK)
                {
                    Globals.MapOutputDir = d.SelectedPath;
                }
            }

            Globals.MapName = this.mapName.Text;

            if (!Directory.Exists(Globals.MapOutputTotalDir))
            {
                Directory.CreateDirectory(Globals.MapOutputTotalDir);
            }

            if (!Directory.Exists(Globals.MapOutputTotalDir + "map\\"))
            {
                Directory.CreateDirectory(Globals.MapOutputTotalDir + "map\\");
            }

            this.CopyDir(Directory.GetCurrentDirectory() + "\\data\\mapstuff", Globals.MapOutputTotalDir + "map");
            this.CopyDir(Directory.GetCurrentDirectory() + "\\data\\common", Globals.MapOutputTotalDir + "common");

            float delta = 1.0f;

            if (this.vhigh.Checked)
            {
                delta = 2.5f;
            }

            if (this.high.Checked)
            {
                delta = 1.75f;
            }

            if (this.normal.Checked)
            {
                delta = 1.2f;
            }

            if (this.low.Checked)
            {
                delta = 0.85f;
            }

            if (this.vLow.Checked)
            {
                delta = 0.6f;
            }

            if (this.mapvlarge.Checked)
            {
                MapGenManager.instance.Width = 4096;
            }

            if (this.maplarge.Checked)
            {
                MapGenManager.instance.Width = 3200;
            }

            if (this.mapnorm.Checked)
            {
                MapGenManager.instance.Width = 3072;
            }

            if (this.mapsmall.Checked)
            {
                MapGenManager.instance.Width = 2048;
            }

            var h = this.adjustedHeight;
            this.adjustedHeight = null;

            if (this.bFromHeightMap)
            {
                MapGenManager.instance.Create(this.bFromHeightMap, this.usedSeed, 1500, delta, h);
            }
            else
            {
                MapGenManager.instance.Create(false, this.usedSeed, 1500, delta);
            }
            //  MapGenManager.instance.Create(usedSeed, 1300);
            // MapGenManager.instance.Create(usedSeed, 600);

            LockBitmap waterColorMap =
                new LockBitmap(
                    DevIL.DevIL.LoadBitmap(Directory.GetCurrentDirectory() +
                                           "\\data\\mapstuff\\terrain\\colormap_water.dds"));
            waterColorMap.ResizeImage(MapGenManager.instance.Width, MapGenManager.instance.Height);
            DevIL.DevIL.SaveBitmap(Globals.MapOutputTotalDir + "map\\terrain\\colormap_water.dds", waterColorMap.Source);
            // LockBitmap normalMap = new LockBitmap(new Bitmap((Directory.GetCurrentDirectory() + "\\data\\mapstuff\\world_normal_height.bmp")));
            //   normalMap.ResizeImage(MapGenManager.instance.Width, MapGenManager.instance.Height);

            //     normalMap.Save24(Globals.MapOutputTotalDir + "map\\world_normal_height.bmp");

            this.preview.Image = DevIL.DevIL.LoadBitmap(Globals.MapOutputTotalDir + "map\\terrain\\colormap.dds");

            this.preview.Invalidate();
            //   map = null;
            this.exportButton.Enabled = false;

            if (
                MessageBox.Show("Would you like to load this map to generate a world history on?", "Load Map?",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MainForm.instance.SetMap(Globals.MapOutputTotalDir);
                this.map = null;
                ProvinceBitmapManager.instance = new ProvinceBitmapManager();
                MapGenManager.instance = new MapGenManager();
                this.Close();
            }

            ProvinceBitmapManager.instance = new ProvinceBitmapManager();
            MapGenManager.instance = new MapGenManager();
            this.map = null;
        }

        public void CopyDir(string from, string to)
        {
            if (!Directory.Exists(to))
            {
                Directory.CreateDirectory(to);
            }

            var files = Directory.GetFiles(to);
            for (int index = 0; index < files.Length; index++)
            {
                var file = files[index];
                File.Delete(file);
            }

            if (Directory.Exists(from))
            {
                files = Directory.GetFiles(from);
                foreach (var file in files)
                {
                    File.Copy(file, to + file.Substring(file.LastIndexOf('\\')));
                }

                var dirs = Directory.GetDirectories(from);

                foreach (var dir in dirs)
                {
                    this.CopyDir(dir, to + dir.Substring(dir.LastIndexOf('\\')));
                }
            }
        }

        private void loadHeight_Click(object sender, EventArgs e)
        {
        }

        private void vhigh_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void high_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void normal_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void low_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void vLow_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void newMap_Click(object sender, EventArgs e)
        {
            this.SeaCommands.Clear();
            this.LandCommands.Clear();
            this.MountainCommands.Clear();
            this.landProxyBitmap = new LockBitmap(new Bitmap(3072 / 2, 2048 / 2));
            this.mountainProxyBitmap = new LockBitmap(new Bitmap(3072 / 2, 2048 / 2));
            var a = new SolidBrush(Color.FromArgb(255, 130, 158, 75));
            var b = new SolidBrush(Color.FromArgb(255, 130 + 40, 158 + 40, 75 + 40));
            var c = new SolidBrush(Color.FromArgb(255, 65, 42, 17));
            var d = new SolidBrush(Color.FromArgb(255, 69, 91, 186));

            using (Graphics gg = Graphics.FromImage(this.landProxyBitmap.Source))
            {
                gg.Clear(Color.FromArgb(255, 69, 91, 186));
            }

            using (Graphics gg = Graphics.FromImage(this.mountainProxyBitmap.Source))
            {
                gg.Clear(Color.Transparent);
            }

            this.preview.Invalidate();
        }

        private void sea_Click(object sender, EventArgs e)
        {
            //     sea.Checked = true;
            this.land.Checked = false;
            //    hill.Checked = false;
            this.mountain.Checked = false;
        }

        private void water_Click(object sender, EventArgs e)
        {
            this.land.Checked = false;
            this.water.Checked = true;
            //  hill.Checked = false;
            this.mountain.Checked = false;
        }

        private void land_Click(object sender, EventArgs e)
        {
            //     sea.Checked = false;
            this.water.Checked = false;
            this.land.Checked = true;
            //  hill.Checked = false;
            this.mountain.Checked = false;
        }

        private void hill_Click(object sender, EventArgs e)
        {
            //     sea.Checked = false;
            this.land.Checked = false;
            //  hill.Checked = true;
            this.mountain.Checked = false;
        }

        private void mountain_Click(object sender, EventArgs e)
        {
            this.water.Checked = false;
            //      sea.Checked = false;
            this.land.Checked = false;
            //   hill.Checked = false;
            this.mountain.Checked = true;
        }

        public struct DrawCommand
        {
            public Point Point { get; set; }

            public int Radius { get; set; }

            public DrawCommand(Point p, int radius)
            {
                this.Radius = radius;
                this.Point = p;
            }
        }

        public List<DrawCommand> LandCommands = new List<DrawCommand>();
        public List<DrawCommand> HillCommands = new List<DrawCommand>();
        public List<DrawCommand> SeaCommands = new List<DrawCommand>();
        public List<DrawCommand> MountainCommands = new List<DrawCommand>();

        private int drawRadius = 32;
        Point lastP = new Point();

        public void Draw(int x, int y)
        {
            if (new Point(x, y) == this.lastP)
            {
                return;
            }

            this.lastP = new Point(x, y);
            if (this.land.Checked)
            {
                this.LandCommands.Add(new DrawCommand(new Point(x, y), this.drawRadius));
                var r = new Rectangle(x - this.drawRadius, y - this.drawRadius, x + this.drawRadius, y + this.drawRadius);

                float delStartX = r.X / (float) this.preview.Width;
                float delStartY = r.Y / (float) this.preview.Height;
                float delEndX = r.Right / (float) this.preview.Width;
                float delEndY = r.Bottom / (float) this.preview.Height;


                Rectangle src = new Rectangle((int) (this.mountainProxyBitmap.Source.Width * delStartX),
                    (int) (this.mountainProxyBitmap.Source.Height * delStartY),
                    (int)
                    ((this.mountainProxyBitmap.Source.Width * delEndX) - (this.mountainProxyBitmap.Source.Width * delStartX)),
                    (int)
                    ((this.mountainProxyBitmap.Source.Height * delEndY) - (this.mountainProxyBitmap.Source.Height * delStartY)));

                delStartX = src.X / (float) this.mountainProxyBitmap.Source.Width;
                delStartY = src.Y / (float) this.mountainProxyBitmap.Source.Height;
                delEndX = src.Right / (float) this.mountainProxyBitmap.Source.Width;
                delEndY = src.Bottom / (float) this.mountainProxyBitmap.Source.Height;


                src = new Rectangle((int) (this.preview.Width * delStartX), (int) (this.preview.Height * delStartY),
                    (int) ((this.preview.Width * delEndX) - (this.preview.Width * delStartX)),
                    (int) ((this.preview.Height * delEndY) - (this.preview.Height * delStartY)));

                this.preview.Invalidate(src);
            }

            if (this.water.Checked)
            {
                this.SeaCommands.Add(new DrawCommand(new Point(x, y), (int) (this.drawRadius / 2.5)));
                this.preview.Invalidate(new Rectangle(x - this.drawRadius, y - this.drawRadius, x + this.drawRadius, y + this.drawRadius));
            }

            if (this.mountain.Checked)
            {
                this.MountainCommands.Add(new DrawCommand(new Point(x, y), (int) (this.drawRadius / 2.5)));
                this.preview.Invalidate(new Rectangle(x - this.drawRadius, y - this.drawRadius, x + this.drawRadius, y + this.drawRadius));
            }
        }

        private bool drawing = false;

        private void preview_MouseDown(object sender, MouseEventArgs e)
        {
            this.last = new Point(e.X, e.Y);
            this.preview.Capture = true;
            this.drawing = true;
            if (this.drawing)
            {
                this.Draw(e.X, e.Y);
            }

            this.landBitmap = null;
            this.preview.Invalidate();
        }

        private void preview_MouseUp(object sender, MouseEventArgs e)
        {
            this.preview.Capture = false;
            this.drawing = false;
            this.preview.Invalidate();
            var a = new SolidBrush(Color.FromArgb(255, 130, 158, 75));
            var b = new SolidBrush(Color.FromArgb(255, 130 + 40, 158 + 40, 75 + 40));
            var c = new SolidBrush(Color.FromArgb(255, 65, 42, 17));
            var d = new SolidBrush(Color.FromArgb(255, 0, 0, 0));

            if (this.land.Checked || this.water.Checked)
            {
                using (Graphics gg = Graphics.FromImage(this.landProxyBitmap.Source))
                {
                    using (Graphics gg2 = Graphics.FromImage(this.mountainProxyBitmap.Source))
                    {
                        foreach (var drawCommand in this.LandCommands)
                        {
                            var rect = new Rectangle(drawCommand.Point.X - drawCommand.Radius,
                                drawCommand.Point.Y - drawCommand.Radius, drawCommand.Radius * 2, drawCommand.Radius * 2);

                            float deltaX = this.mountainProxyBitmap.Source.Width / (float) this.preview.Width;
                            float deltaY = this.mountainProxyBitmap.Source.Height / (float) this.preview.Height;

                            Point ap = new Point((int) (rect.X * deltaX), (int) (rect.Y * deltaY));
                            Point bp = new Point((int) (rect.Right * deltaX), (int) (rect.Bottom * deltaY));
                            rect = new Rectangle(ap.X, ap.Y, bp.X - ap.X, bp.Y - ap.Y);
                            gg.FillEllipse(a, rect);
                            gg2.FillEllipse(d, rect);
                        }

                        foreach (var drawCommand in this.SeaCommands)
                        {
                            var rect = new Rectangle(drawCommand.Point.X - drawCommand.Radius,
                                drawCommand.Point.Y - drawCommand.Radius, drawCommand.Radius * 2, drawCommand.Radius * 2);

                            float deltaX = this.mountainProxyBitmap.Source.Width / (float) this.preview.Width;
                            float deltaY = this.mountainProxyBitmap.Source.Height / (float) this.preview.Height;

                            Point ap = new Point((int) (rect.X * deltaX), (int) (rect.Y * deltaY));
                            Point bp = new Point((int) (rect.Right * deltaX), (int) (rect.Bottom * deltaY));
                            rect = new Rectangle(ap.X, ap.Y, bp.X - ap.X, bp.Y - ap.Y);

                            gg.FillEllipse(d, rect);
                            gg2.FillEllipse(d, rect);
                        }
                    }

                    this.landProxyBitmap.Source.MakeTransparent(Color.Black);
                    this.mountainProxyBitmap.Source.MakeTransparent(Color.Black);
                    this.LandCommands.Clear();
                    this.SeaCommands.Clear();
                }
            }

            if (this.mountain.Checked)
            {
                using (Graphics gg = Graphics.FromImage(this.mountainProxyBitmap.Source))
                {
                    foreach (var drawCommand in this.MountainCommands)
                    {
                        var rect = new Rectangle(drawCommand.Point.X - drawCommand.Radius,
                            drawCommand.Point.Y - drawCommand.Radius, drawCommand.Radius * 2, drawCommand.Radius * 2);

                        float deltaX = this.mountainProxyBitmap.Source.Width / (float) this.preview.Width;
                        float deltaY = this.mountainProxyBitmap.Source.Height / (float) this.preview.Height;

                        Point ap = new Point((int) (rect.X * deltaX), (int) (rect.Y * deltaY));
                        Point bp = new Point((int) (rect.Right * deltaX), (int) (rect.Bottom * deltaY));
                        rect = new Rectangle(ap.X, ap.Y, bp.X - ap.X, bp.Y - ap.Y);

                        gg.FillEllipse(c, rect);
                    }

                    this.MountainCommands.Clear();
                }
            }

            this.MapGenerator_ResizeEnd(null, new EventArgs());
        }

        private Point last;

        private void preview_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.drawing)
            {
                float difx = (e.X - this.last.X);
                float dify = (e.Y - this.last.Y);
                if (difx != 0)
                {
                    difx = 1 / difx;
                }

                if (dify != 0)
                {
                    dify = 1 / dify;
                }

                float xx = this.last.X;
                float yy = this.last.Y;
                while (Math.Abs(Math.Round(xx) - e.X) > 0.001f && Math.Abs(Math.Round(yy) - e.Y) > 0.001f)
                {
                    this.Draw((int) Math.Round(xx), (int) Math.Round(yy));
                    xx += difx;
                    yy += dify;
                }

                this.last.X = e.X;
                this.last.Y = e.Y;

                this.Draw(e.X, e.Y);
            }
        }

        private void preview_Paint(object sender, PaintEventArgs e)
        {
            if (this.tabPage1.Visible)
            {
                return;
            }

            if (this.tabPage3.Visible)
            {
                return;
            }

            e.Graphics.PageUnit = GraphicsUnit.Pixel;

            var a = new SolidBrush(Color.FromArgb(255, 130, 158, 75));
            var b = new SolidBrush(Color.FromArgb(255, 130 + 40, 158 + 40, 75 + 40));
            var c = new SolidBrush(Color.FromArgb(255, 65, 42, 17));
            var d = new SolidBrush(Color.FromArgb(255, 69, 91, 186));
            e.Graphics.Clear(Color.FromArgb(255, 69, 91, 186));

            ColorMatrix cm = new ColorMatrix();
            ImageAttributes ia = new ImageAttributes();


            cm.Matrix00 = (130 / 255.0f); // * 0.7f;
            cm.Matrix11 = (158 / 255.0f); // * 0.7f;
            cm.Matrix22 = (75 / 255.0f); // * 0.7f;
            cm.Matrix33 = 1.0f;
            cm.Matrix44 = 1.0f;
            //   cm.Matrix33 = 0.4f;

            ia.SetColorMatrix(cm);

            var totRect = e.ClipRectangle;


            e.Graphics.DrawImage(this.landDrawBitmap.Source, totRect, totRect, e.Graphics.PageUnit);
            e.Graphics.DrawImage(this.mountainDrawBitmap.Source, totRect, totRect, e.Graphics.PageUnit);
            foreach (var drawCommand in this.LandCommands)
            {
                e.Graphics.FillEllipse(a,
                    new Rectangle(drawCommand.Point.X - drawCommand.Radius, drawCommand.Point.Y - drawCommand.Radius,
                        drawCommand.Radius * 2, drawCommand.Radius * 2));
            }

            foreach (var drawCommand in this.SeaCommands)
            {
                e.Graphics.FillEllipse(d,
                    new Rectangle(drawCommand.Point.X - drawCommand.Radius, drawCommand.Point.Y - drawCommand.Radius,
                        drawCommand.Radius * 2, drawCommand.Radius * 2));
            }

            foreach (var drawCommand in this.HillCommands)
            {
                e.Graphics.FillEllipse(b,
                    new Rectangle(drawCommand.Point.X - drawCommand.Radius, drawCommand.Point.Y - drawCommand.Radius,
                        drawCommand.Radius * 2, drawCommand.Radius * 2));
            }

            foreach (var drawCommand in this.MountainCommands)
            {
                e.Graphics.FillEllipse(c,
                    new Rectangle(drawCommand.Point.X - drawCommand.Radius, drawCommand.Point.Y - drawCommand.Radius,
                        drawCommand.Radius * 2, drawCommand.Radius * 2));
            }

            if (this.landBitmap != null)
            {
                e.Graphics.DrawImage(this.landBitmap, new Rectangle(0, 0, this.preview.Width, this.preview.Height));
            }
        }

        private Bitmap landBitmap = null;
        private Bitmap hillBitmap = null;
        private Bitmap mountainBitmap = null;
        private Bitmap landBitmapOut;

        private LockBitmap landDrawBitmap = null;
        private LockBitmap mountainDrawBitmap = null;
        private LockBitmap landProxyBitmap = null;
        private LockBitmap mountainProxyBitmap = null;

        private void generateFromDraw_Click(object sender, EventArgs e)
        {
            this.generateDrawn.Enabled = true;
            int ow = 1;
            int oh = 1;
            int w = 3072;
            int h = 2048;
            if (this.mapvlarge.Checked)
            {
                w = 4096;
            }

            if (this.maplarge.Checked)
            {
                w = 3200;
            }

            if (this.mapnorm.Checked)
            {
                w = 3072;
            }

            if (this.mapsmall.Checked)
            {
                w = 2048;
            }

            {
                Bitmap bmp = new Bitmap(w / 2, h / 2, PixelFormat.Format24bppRgb);
                Bitmap bmp2 = new Bitmap(w / 2, h / 2, PixelFormat.Format24bppRgb);
                LockBitmap lbmp2 = new LockBitmap(bmp2);

                LockBitmap lbmp = new LockBitmap(bmp);

                float deltaX = w / (float) this.preview.Width;
                float deltaY = h / (float) this.preview.Height;

                lbmp.LockBits();
                lbmp2.LockBits();
                this.landProxyBitmap.LockBits();
                this.mountainProxyBitmap.LockBits();
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        float dx = this.mountainProxyBitmap.Width / (float) bmp.Width;
                        float dy = this.mountainProxyBitmap.Height / (float) bmp.Height;

                        var col = this.mountainProxyBitmap.GetPixel((int) (x * dx), (int) (y * dy));

                        var col2 = this.landProxyBitmap.GetPixel((int) (x * dx), (int) (y * dy));

                        int xx = x;
                        int yy = y;

                        if (col.R > 0 && col.A > 0)
                        {
                            lbmp.SetPixel(xx, yy, Color.White);
                        }
                        else
                        {
                            lbmp.SetPixel(xx, yy, Color.Black);
                        }

                        if (col2.R != 69 && col2.A > 0)
                        {
                            lbmp2.SetPixel(xx, yy, Color.White);
                        }
                        else
                        {
                            lbmp2.SetPixel(xx, yy, Color.Black);
                        }
                    }
                }

                lbmp2.UnlockBits();
                lbmp.UnlockBits();
                this.landProxyBitmap.UnlockBits();
                this.mountainProxyBitmap.UnlockBits();
                /*   Graphics g = Graphics.FromImage(bmp);

                   foreach (var drawCommand in MountainCommands)
                   {
                       var r = new Rectangle(drawCommand.Point.X - drawCommand.Radius, drawCommand.Point.Y - drawCommand.Radius,
                           drawCommand.Radius * 2, drawCommand.Radius * 2);

                       Rectangle rect = new Rectangle((int)(r.X * deltaX), (int)(r.Y * deltaY), (int)((r.Right * deltaX) - (r.Left * deltaX)), (int)((r.Bottom * deltaY) - r.Top * deltaY));

                       g.FillEllipse(a, rect);
                   }
                   */
                int rand = 32;
                if (this.randMed.Checked)
                {
                    rand = 32;
                }

                if (this.randHigh.Checked)
                {
                    rand = 32;
                }

                ow = bmp.Width;
                oh = bmp.Height;
                lbmp.ResizeImage(ow / rand, oh / rand);
                lbmp.ResizeImage(ow * 2, oh * 2);
                this.mountainBitmap = lbmp.Source;

                rand = 8;
                BitmapSelect.Randomness = 0.75f;

                if (this.randMed.Checked)
                {
                    rand = 16;
                    BitmapSelect.Randomness = 1f;
                }

                if (this.randHigh.Checked)
                {
                    rand = 64;
                    BitmapSelect.Randomness = 1f;
                }

                if (this.randMin.Checked)
                {
                    rand = 2;
                    BitmapSelect.Randomness = 0.5f;
                }

                ow = bmp2.Width;
                oh = bmp2.Height;
                lbmp2.ResizeImage(ow / rand, oh / rand);
                lbmp2.ResizeImage(ow * 2, oh * 2);
                this.landBitmap = lbmp2.Source;
            }

            this.map = new GeneratedTerrainMap();

            int seed = RandomIntHelper.Next(1000000);
            LockBitmap lBit = new LockBitmap(this.landBitmap);
            LockBitmap mBit = new LockBitmap(this.mountainBitmap);
            this.map.Init(ow / 4, oh / 4, lBit, lBit, mBit, seed);
            lBit.UnlockBits();
            mBit.UnlockBits();
            this.DrawnSeed = seed;
            this.landBitmap = this.map.Map.Source;
            this.landBitmapOut = lBit.Source;
            this.mountainBitmap = mBit.Source;
            this.preview.Invalidate();
        }

        public int DrawnSeed { get; set; }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            this.preview.Invalidate();
        }

        private void smallBrush_Click(object sender, EventArgs e)
        {
            this.smallBrush.Checked = true;
            this.mediumBrush.Checked = false;
            this.largeBrush.Checked = false;
            this.drawRadius = 16;
        }

        private void mediumBrush_Click(object sender, EventArgs e)
        {
            this.smallBrush.Checked = false;
            this.mediumBrush.Checked = true;
            this.largeBrush.Checked = false;
            this.drawRadius = 32;
        }

        private void largeBrush_Click(object sender, EventArgs e)
        {
            this.smallBrush.Checked = false;
            this.mediumBrush.Checked = false;
            this.largeBrush.Checked = true;
            this.drawRadius = 64;
        }

        private void randLow_Click(object sender, EventArgs e)
        {
            this.randLow.Checked = true;
            this.randMed.Checked = false;
            this.randHigh.Checked = false;
            this.randMin.Checked = false;
        }

        private void randMed_Click(object sender, EventArgs e)
        {
            this.randLow.Checked = false;
            this.randMed.Checked = true;
            this.randHigh.Checked = false;
            this.randMin.Checked = false;
        }

        private void randHigh_Click(object sender, EventArgs e)
        {
            this.randLow.Checked = false;
            this.randMed.Checked = false;
            this.randHigh.Checked = true;
            this.randMin.Checked = false;
        }

        private void randMin_Click(object sender, EventArgs e)
        {
            this.randLow.Checked = false;
            this.randMed.Checked = false;
            this.randHigh.Checked = false;
            this.randMin.Checked = true;
        }

        private void MapGenerator_ResizeEnd(object sender, EventArgs e)
        {
            if (this.landDrawBitmap == null)
            {
                return;
            }

            var oldLand = this.landProxyBitmap;
            var oldMount = this.mountainProxyBitmap;
            if (this.landDrawBitmap.Source.Width != this.preview.Width || this.landDrawBitmap.Source.Height != this.preview.Height)
            {
                this.landDrawBitmap = new LockBitmap(new Bitmap(this.preview.Width, this.preview.Height));
                this.mountainDrawBitmap = new LockBitmap(new Bitmap(this.preview.Width, this.preview.Height));
            }

            using (Graphics gg = Graphics.FromImage(this.landDrawBitmap.Source))
            {
                gg.Clear(Color.Transparent);
                gg.SmoothingMode = SmoothingMode.Default;
                gg.DrawImage(oldLand.Source, new Rectangle(0, 0, this.preview.Width, this.preview.Height));
            }

            using (Graphics gg = Graphics.FromImage(this.mountainDrawBitmap.Source))
            {
                gg.Clear(Color.Transparent);
                gg.SmoothingMode = SmoothingMode.Default;
                gg.DrawImage(oldMount.Source, new Rectangle(0, 0, this.preview.Width, this.preview.Height));
            }
        }

        private void climate_SelectedIndexChanged(object sender, EventArgs e)
        {
            Globals.Climate = this.climate.SelectedIndex;
        }

        private void loadHeightMap_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "All Graphics Types|*.bmp;*.png;*.jpg";
            openFileDialog1.FilterIndex = 0;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap i = (Bitmap)Bitmap.FromFile(openFileDialog1.FileName);

                this.HeightMapBitmap = new LockBitmap(i);
                this.HeightMapBitmapPreview = new LockBitmap(new Bitmap(i));
                {
                    this.HeightMapBitmapPreview.LockBits();

                    if ((this.HeightMapBitmapPreview.Width * this.HeightMapBitmapPreview.Height) > 4096 * 2048)
                    {
                        MessageBox.Show(this, "Error: Height map too large. Max size 4096x2048");
                        this.HeightMapBitmapPreview.UnlockBits();
                        return;
                    }

                    for (int x = 0; x < this.HeightMapBitmapPreview.Width; x++)
                    {
                        for (int y = 0; y < this.HeightMapBitmapPreview.Height; y++)
                        {
                            byte h = this.HeightMapBitmapPreview.GetHeight(x, y);
                            if (h < this.seaLevel.Value )
                            {
                                this.HeightMapBitmapPreview.SetPixel(x, y, Color.FromArgb(255, 69, 91, 186));
                            }
                        }
                    }

                    this.HeightMapBitmapPreview.UnlockBits();
                    this.preview.Image = this.HeightMapBitmapPreview.Source;
                    this.preview.Invalidate();
                    //using (Graphics gg = Graphics.FromImage(landDrawBitmap.Source))
                    {
                      //  gg.Clear(Color.Transparent);
                    //    gg.SmoothingMode = SmoothingMode.Default;
                  //      gg.DrawImage(i, new Rectangle(0, 0, preview.Width, preview.Height));
                    }
                }
            }
        }

        public LockBitmap HeightMapBitmapPreview { get; set; }

        public LockBitmap HeightMapBitmap { get; set; }

        private void seaLevel_ValueChanged(object sender, EventArgs e)
        {
            this.HeightMapBitmapPreview = new LockBitmap(new Bitmap(this.HeightMapBitmap.Source));
            {
                this.HeightMapBitmapPreview.LockBits();
                for (int x = 0; x < this.HeightMapBitmapPreview.Width; x++)
                {
                    for (int y = 0; y < this.HeightMapBitmapPreview.Height; y++)
                    {
                        byte h = this.HeightMapBitmapPreview.GetHeight(x, y);
                        if (h < this.seaLevel.Value)
                        {
                            this.HeightMapBitmapPreview.SetPixel(x, y, Color.FromArgb(255, 69, 91, 186));
                        }
                    }
                }

                this.HeightMapBitmapPreview.UnlockBits();
                this.preview.Image = this.HeightMapBitmapPreview.Source;
                this.preview.Invalidate();
            }
        }

        private LockBitmap adjustedHeight;
        private bool bFromHeightMap;

        private void generateFromHeightMap_Click(object sender, EventArgs e)
        {
            if (this.mapvlarge.Checked)
            {
                MapGenManager.instance.Width = 4096;
            }

            if (this.maplarge.Checked)
            {
                MapGenManager.instance.Width = 3200;
            }

            if (this.mapnorm.Checked)
            {
                MapGenManager.instance.Width = 3072;
            }

            if (this.mapsmall.Checked)
            {
                MapGenManager.instance.Width = 2048;
            }

            this.adjustedHeight = new LockBitmap(new Bitmap(MapGenManager.instance.Width, 2048, this.HeightMapBitmap.Source.PixelFormat));
            this.HeightMapBitmap.LockBits();
            this.bFromHeightMap = true;
            this.adjustedHeight.LockBits();
            float seaLevel = (float) this.seaLevel.Value;

            for (int x = 0; x < this.adjustedHeight.Width; x++)
            {
                for (int y = 0; y < this.adjustedHeight.Height; y++)
                {
                    this.adjustedHeight.SetPixel(x, y, 89 / 255.0f);
                }
            }

            for (int x = 0; x < this.HeightMapBitmap.Width; x++)
            {
                for (int y = 0; y < this.HeightMapBitmap.Height; y++)
                {
                    byte h = this.HeightMapBitmap.GetHeight(x, y);

                    int xa = x;
                    int ya = y;

                    int halfo = this.HeightMapBitmap.Width / 2;
                    int halfa = this.adjustedHeight.Width / 2;
                    int startX = halfa - halfo;
                    xa += startX;

                    if (this.HeightMapBitmap.Height < this.adjustedHeight.Height)
                    {
                        halfo = this.HeightMapBitmap.Height / 2;
                        halfa = this.adjustedHeight.Height / 2;
                        ya += halfa - halfo;
                    }

                    if (h < seaLevel)
                    {
                        float below = 89;

                        this.adjustedHeight.SetPixel(xa, ya, below / 255.0f);
                    }
                    else
                    {
                        h -= (byte)seaLevel;
                        float delta = h / (255.0f - seaLevel);

                        delta *= (float)(this.reliefScale.Value) / 100.0f;
                        float below = delta * (255.0f - 98.0f);
                        below += 98;

                        this.adjustedHeight.SetPixel(xa, ya, below / 255.0f);
                    }
                }
            }

            this.HeightMapBitmap.UnlockBits();
            this.adjustedHeight.UnlockBits();

         //   adjustedHeight.ResizeImage(adjustedHeight.Width / 2, adjustedHeight.Height / 2);
        //    adjustedHeight.ResizeImage(adjustedHeight.Width, adjustedHeight.Height);
            this.HeightMapBitmap.Source.Dispose();
            this.HeightMapBitmap = null;
            GC.Collect();
            this.exportButton_Click(null, new EventArgs());
            /*

                        {
                            map = new GeneratedTerrainMap();

                            int ow = 1;
                            int oh = 1;
                            int w = 3072;
                            int h = 2048;
                            if (mapvlarge.Checked)
                                w = 4096;

                            if (maplarge.Checked)
                                w = 3200;

                            if (mapnorm.Checked)
                                w = 3072;

                            if (mapsmall.Checked)
                                w = 2048;


                            int seed = Rand.Next(1000000);

                            map.InitFromHeightMap(w / 4, h / 4, adjustedHeight);

                            DrawnSeed = seed;
                            landBitmap = map.Map.Source;

                            preview.Image = landBitmap;
                            preview.Invalidate();
                        }
                        */
        }
    }
}

