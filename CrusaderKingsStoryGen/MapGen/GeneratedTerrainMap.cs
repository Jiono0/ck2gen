// <copyright file="GeneratedTerrainMap.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.MapGen
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using CrusaderKingsStoryGen.Helpers;
    using LibNoise.Modfiers;

    public class GeneratedTerrainMap
    {
        private float DivNoise = 1.0f;
        private int heightSeed1;
        private int heightSeed2;
        bool preview;

        public void Init(int width, int height, int seed = 0, bool preview = false)
        {
            this.preview = preview;
            //            this.preview = false;
            if (seed != 0)
            {
                RandomIntHelper.SetSeed(seed);
            }
            else
            {
                RandomIntHelper.SetSeed(new Random().Next(1000000));
            }

            if (!preview)
            {
                this.Width = width / 4;
                this.Height = height / 4;
            }
            else
            {
                this.Width = width;
                this.Height = height;
            }

            this.DivNoise = 1200.0f * (this.Height / 2048.0f);
            Bitmap terrainMap = null;
            if (preview)
            {
                terrainMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            }
            else
            {
                terrainMap = new Bitmap(this.Width * 4, this.Height * 4, PixelFormat.Format24bppRgb);
            }

            Bitmap moistureMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            Bitmap heatMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            Bitmap trees = new Bitmap((int)((this.Width * 4) / 8.04188481675f), (int)((this.Height * 4) / 8.04188481675f));
            this.Trees = new LockBitmap(trees);
            this.MoistureMap = new LockBitmap(moistureMap);
            this.HeatMap = new LockBitmap(heatMap);

            this.TerrainMap = new LockBitmap(terrainMap);
            this.heightSeed1 = RandomIntHelper.Next(10000000);
            this.heightSeed2 = RandomIntHelper.Next(10000000);
            this.CreateInitialHeightMap();
            this.CreateInitialTerrainMap();
            //     Save();
        }

        public void InitFromHeightMap(int width, int height, LockBitmap heightMap = null, bool preview = true, int seed = 0)
        {
            Bitmap terrainMap = null;

            if (seed != 0)
            {
                RandomIntHelper.SetSeed(seed);
            }
            else
            {
                RandomIntHelper.SetSeed(new Random().Next(1000000));
            }

            if (!preview)
            {
                this.Width = width / 4;
                this.Height = height / 4;
            }
            else
            {
                this.Width = width;
                this.Height = height;
            }

            if (preview)
            {
                terrainMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            }
            else
            {
                terrainMap = new Bitmap(this.Width * 4, this.Height * 4, PixelFormat.Format24bppRgb);
            }

            Bitmap moistureMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            Bitmap heatMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            Bitmap trees = new Bitmap((int)((this.Width * 4) / 8.04188481675f), (int)((this.Height * 4) / 8.04188481675f));
            this.Trees = new LockBitmap(trees);
            this.MoistureMap = new LockBitmap(moistureMap);
            this.HeatMap = new LockBitmap(heatMap);
            this.TerrainMap = new LockBitmap(terrainMap);

            this.heightSeed1 = RandomIntHelper.Next(10000000);
            this.heightSeed2 = RandomIntHelper.Next(10000000);
            this.Map = heightMap;

            this.CreateInitialTerrainMap();
        }

        public void Init(int width, int height, LockBitmap landBitmap = null, LockBitmap hillBitmap = null, LockBitmap mountainBitmap = null, int seed = 0, bool preview = false)
        {
            this.preview = preview;

            if (seed != 0)
            {
                RandomIntHelper.SetSeed(seed);
            }
            else
            {
                RandomIntHelper.SetSeed(new Random().Next(1000000));
            }

            if (!preview)
            {
                this.Width = width / 4;
                this.Height = height / 4;
            }
            else
            {
                this.Width = width;
                this.Height = height;
            }

            this.DivNoise = 1200.0f * (this.Height / 2048.0f);
            Bitmap terrainMap = null;
            if (preview)
            {
                terrainMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            }
            else
            {
                terrainMap = new Bitmap(this.Width * 4, this.Height * 4, PixelFormat.Format24bppRgb);
            }

            Bitmap moistureMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            Bitmap heatMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            Bitmap trees = new Bitmap((int)((this.Width * 4) / 8.04188481675f), (int)((this.Height * 4) / 8.04188481675f));
            this.Trees = new LockBitmap(trees);
            this.MoistureMap = new LockBitmap(moistureMap);
            this.HeatMap = new LockBitmap(heatMap);

            this.TerrainMap = new LockBitmap(terrainMap);
            this.heightSeed1 = RandomIntHelper.Next(10000000);
            this.heightSeed2 = RandomIntHelper.Next(10000000);
            this.CreateInitialHeightMap(landBitmap, hillBitmap, mountainBitmap);
            this.CreateInitialTerrainMap();


            //     Save();
        }

        public LockBitmap Trees { get; set; }

        public LockBitmap HeightModifier { get; set; }

        public void Refine()
        {
            var provinceMap = MapGenManager.instance.ProvinceMap;
            this.CreateHeightMap(provinceMap);
            this.CreateTerrainMap();
            MapGenManager.instance.ProvinceMap = null;
            provinceMap.Source.Dispose();
            provinceMap = null;
           ConvertTo24bpp(this.Map.Source).Save(Globals.MapOutputTotalDir + "map\\topology.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            var normalMap = NormalMapGenerator.instance.Create(this.Map);

            ConvertTo24bpp(normalMap.Source).Save(Globals.MapOutputTotalDir + "map\\world_normal_height.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            this.Map.Source.Dispose();
            this.Map.Source = null;
            ProvinceBitmapManager.instance = new ProvinceBitmapManager();
            this.CreateColorMap();

            this.UpdateHeightMap();
        }

        public static Color SeaColor = Color.FromArgb(255, 69, 91, 186);

        public static Color DesertSandColor = Color.FromArgb(255, 206, 169, 99);

        public static Color SandyGrassColor = Color.FromArgb(255, 130, 158, 75);

        public static Color TemperateForestColor = Color.FromArgb(255, 0, 86, 6);

        public static Color NorthernGrassColor = Color.FromArgb(255, 13, 96, 62);

        public static Color TemperateGrassColor = Color.FromArgb(255, 86, 124, 27);

        public static Color SteppesColor = Color.FromArgb(255, 255, 186, 0);

        public static Color HillsDesertColor = Color.FromArgb(255, 112, 74, 31);

        public static Color SandMountainColor = Color.FromArgb(255, 86, 46, 0);

        public static Color SnowMountainTopColor = Color.FromArgb(255, 255, 255, 255);

        public static Color MountainRockColor = Color.FromArgb(255, 65, 42, 17);

        public void CreateColorMap()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Bitmap ocean = ConvertTo24bpp(new Bitmap(Directory.GetCurrentDirectory() + "\\data\\ocean.png"));
            Bitmap grass = ConvertTo24bpp(new Bitmap(Directory.GetCurrentDirectory() + "\\data\\grass.png"));
            Bitmap desert = ConvertTo24bpp(new Bitmap(Directory.GetCurrentDirectory() + "\\data\\desert.png"));
            Bitmap mountain = ConvertTo24bpp(new Bitmap(Directory.GetCurrentDirectory() + "\\data\\mountain.png"));

            Bitmap mountainAlpha = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);
            Bitmap grassAlpha = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);
            Bitmap desertAlpha = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);
            Bitmap finalMap = new Bitmap(this.Width, this.Height, PixelFormat.Format24bppRgb);
            LockBitmap lOcean = new LockBitmap(ocean);
            LockBitmap lMountain = new LockBitmap(mountain);
            LockBitmap lGrass = new LockBitmap(grass);
            LockBitmap lDesert = new LockBitmap(desert);
            LockBitmap lGrassAlpha = new LockBitmap(grassAlpha);
            LockBitmap lDesertAlpha = new LockBitmap(desertAlpha);
            LockBitmap lMountainAlpha = new LockBitmap(mountainAlpha);

            LockBitmap lFinal = new LockBitmap(finalMap);
            lGrassAlpha.LockBits();
            lDesertAlpha.LockBits();
            lMountainAlpha.LockBits();
            this.TerrainMap.LockBits();
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    Color col = this.TerrainMap.GetPixel(x, y);

                    if (col != SeaColor)
                    {
                        lGrassAlpha.SetPixel(x, y, Color.White);

                        if (col == SandyGrassColor)
                        {
                            lDesertAlpha.SetPixel(x, y, Color.LightGray);
                            lMountainAlpha.SetPixel(x, y, Color.FromArgb(255, 0x50, 0x50, 0x50));
                        }
                        else if (col == SteppesColor)
                            lMountainAlpha.SetPixel(x, y, Color.Gray);
                        else if (col == DesertSandColor)
                            lDesertAlpha.SetPixel(x, y, Color.White);
                        else if (col == MountainRockColor|| col == HillsDesertColor || col == SandMountainColor || col == SnowMountainTopColor)
                            lMountainAlpha.SetPixel(x, y, Color.White);
                    }
                }
            }

            this.TerrainMap.UnlockBits();


            lGrassAlpha.Blur(1);

            lDesertAlpha.Blur(1);

            lMountainAlpha.Blur(1);

            lOcean.LockBits();
            lGrass.LockBits();
            lDesert.LockBits();
            lMountain.LockBits();
            lFinal.LockBits();

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    int sx = x;
                    if (sx >= 3072)
                    {
                        sx -= 3072;
                    }

                    float grassAlphaVal = lGrassAlpha.GetPixel(x, y).R / 255.0f;
                    float desertAlphaVal = lDesertAlpha.GetPixel(x, y).R / 255.0f;
                    float mountainAlphaValue = lMountainAlpha.GetPixel(x, y).R / 255.0f;

                    Color seaCol = lOcean.GetPixel(sx, y);
                    Color grassCol = lGrass.GetPixel(sx, y);
                    Color mountainCol = lMountain.GetPixel(sx, y);
                    Color desCol = lDesert.GetPixel(sx, y);

                    float r = seaCol.R/255.0f;
                    float g = seaCol.G / 255.0f;
                    float b = seaCol.B / 255.0f;

                    r = this.Lerp(r, grassCol.R / 255.0f, grassAlphaVal);
                    g = this.Lerp(g, grassCol.G / 255.0f, grassAlphaVal);
                    b = this.Lerp(b, grassCol.B / 255.0f, grassAlphaVal);

                    r = this.Lerp(r, desCol.R / 255.0f, desertAlphaVal);
                    g = this.Lerp(g, desCol.G / 255.0f, desertAlphaVal);
                    b = this.Lerp(b, desCol.B / 255.0f, desertAlphaVal);

                    r = this.Lerp(r, mountainCol.R / 255.0f, mountainAlphaValue);
                    g = this.Lerp(g, mountainCol.G / 255.0f, mountainAlphaValue);
                    b = this.Lerp(b, mountainCol.B / 255.0f, mountainAlphaValue);

                    Color final = Color.FromArgb(255, (int) (r*255), (int) (g*255), (int) (b*255));

                    lFinal.SetPixel(x, y, final);
                }
            }


            lGrassAlpha.UnlockBits();
            lDesertAlpha.UnlockBits();

            lMountainAlpha.UnlockBits();
            lGrassAlpha.Source.Dispose();
            lMountainAlpha.Source.Dispose();
            lDesertAlpha.Source.Dispose();

            lGrassAlpha = null;
            lDesertAlpha = null;
            lMountainAlpha = null;
            lOcean.UnlockBits();
            lGrass.UnlockBits();
            lDesert.UnlockBits();
            lMountain.UnlockBits();
            lFinal.UnlockBits();
            if (!Directory.Exists(Globals.MapOutputTotalDir))
            {
                Directory.CreateDirectory(Globals.MapOutputTotalDir);
            }

            if (!Directory.Exists(Globals.MapOutputTotalDir + "map\\"))
            {
                Directory.CreateDirectory(Globals.MapOutputTotalDir + "map\\");
            }

            if (!Directory.Exists(Globals.MapOutputTotalDir + "map\\terrain\\"))
            {
                Directory.CreateDirectory(Globals.MapOutputTotalDir + "map\\terrain\\");
            }

            if (File.Exists(Globals.MapOutputTotalDir + "map\\terrain\\colormap.dds"))
            {
                File.Delete(Globals.MapOutputTotalDir + "map\\terrain\\colormap.dds");
            }

            lOcean.Source.Dispose();
            lGrass.Source.Dispose();
            lDesert.Source.Dispose();
            lMountain.Source.Dispose();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            DevIL.DevIL.SaveBitmap(Globals.MapOutputTotalDir + "map\\terrain\\colormap.dds", lFinal.Source);
            lFinal.Source.Dispose();
            lFinal = null;
        }

        private void UpdateHeightMap()
        {
        }

        public LockBitmap MoistureMap { get; set; }

        public LockBitmap HeatMap { get; set; }


        public static Bitmap ConvertTo24bpp(Image img)
        {
            var bmp = new Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            }

            return bmp;
        }


        private void CreateTerrainMap()
        {
            this.MoistureMap.ResizeImage(this.Map.Width, this.Map.Height);
            this.HeatMap.LockBits();
            this.MoistureMap.LockBits();
            this.Map.LockBits();
            this.Trees.LockBits();
            this.TerrainMap.LockBits();
            MapGenManager.instance.ProvinceMap.LockBits();
            NoiseGenerator noiseH = new NoiseGenerator();
            NoiseGenerator noiseM = new NoiseGenerator();
            noiseM.Octaves = 4;
            for (int x = 0; x < this.Map.Width; x++)
                for (int y = 0; y < this.Map.Height; y++)
                {
                    Color col = Color.AliceBlue;
                    float heat = this.GetHeat(x, y);// + (Rand.Next(-30, 30) / 8000.0f);
                    float moisture = this.GetMoisture(x*4, y*4);// + (Rand.Next(-30, 30) / 8000.0f);
                    float no = (float)((float)((noiseH.Noise(x / this.DivNoise / 8, y / this.DivNoise / 8)) - 0.5f) * 0.2);
                    float tree = (float)((float)((noiseM.Noise(x / this.DivNoise, y / this.DivNoise)) - 0.5f));

                    tree += no * 12.0f;
                    heat += no * 3.0f;
                    moisture -= no * 3.0f;

                    float height = this.Map.GetHeight(x, y) / 255.0f;

                    float desertLevel = 0.48f;
                    float shyDesertLevel = 0.43f;
                    float desertDry = 0.6f;
                    float shyDry = 0.62f;
                    float coastalDesertLevel = 0.7f;
                    float tundraWetLevel = 0.25f;

                    float jungleLevel = 0.8f;
                    float arcticLevel = 0.13f;

                    float hillLevel = 0.55f;
                    float mountainLevel = 0.60f;
                    float highmountainLevel = 0.8f;

                    heat += height / 8.0f;
                    moisture -= height / 8.0f;
                    float sealevel = (98 / 255.0f);
                    tree += (height - sealevel) * 2.3f;
                    tree += moisture * 1.9f;

                    bool hot = heat > desertLevel;
                    bool slightlyHot = !hot && heat > shyDesertLevel;
                    bool cold = heat < arcticLevel;
                    bool wet = moisture > coastalDesertLevel;
                    bool dry = moisture < desertDry;
                    bool nearlyDry = moisture < desertDry;
                    bool wetCold = moisture > tundraWetLevel;
                    bool sea = Color.FromArgb(255, 69, 91, 186) == this.TerrainMap.GetPixel(x, y);
                    bool trees = tree > 0.99f;
                    bool temptrees = tree > 0.99f;
                    if (sea)
                    {
                        col = SeaColor;
                    }
                    else
                    {
                        if (hot || slightlyHot)
                        {
                            if (wet)
                            {
                                col = Color.FromArgb(255, 40, 180, 149);

                                if (trees && wet)
                                {
                                    float dx = x / (float)this.Width;
                                    float dy = y / (float)this.Height;
                                    int tx = (int)(this.Trees.Width * dx);
                                    int ty = (int)(this.Trees.Height * dy);
                                    if (RandomIntHelper.Next(2) == 0)
                                        if (height > 107 / 255.0f)
                                        {
                                            this.Trees.SetPixel(tx, ty, Color.FromArgb(255, 154, 156, 51));
                                        }
                                }
                            }
                            else if (hot && dry)
                                col = DesertSandColor;
                            else if (slightlyHot && dry)
                                col = SandyGrassColor;
                            else
                            {
                                if(nearlyDry)
                                {
                                    col = DesertSandColor;
                                }
                                else
                                {
                                    col = TemperateGrassColor;
                                }
                            }
                        }
                        else if (cold)
                        {
                            col = NorthernGrassColor;

                            if (temptrees)
                            {
                                col = Color.FromArgb(255, 0, 86, 6);

                                float dx = x / (float)this.Width;
                                float dy = y / (float)this.Height;
                                int tx = (int)(this.Trees.Width * dx);
                                int ty = (int)(this.Trees.Height * dy);
                                if (RandomIntHelper.Next(4) == 0)
                                    if (height > 107 / 255.0f)
                                    {
                                        this.Trees.SetPixel(tx, ty, Color.FromArgb(255, 30, 139, 109));
                                    }
                            }
                            else
                            {
                                if (RandomIntHelper.Next(18) == 0)
                                {
                                    float dx = x / (float)this.Width;
                                    float dy = y / (float)this.Height;
                                    int tx = (int)(this.Trees.Width * dx);
                                    int ty = (int)(this.Trees.Height * dy);
                                    if (height > 107 / 255.0f)
                                    {
                                        this.Trees.SetPixel(tx, ty, Color.FromArgb(255, 30, 139, 109));
                                    }
                                }
                            }
                        }
                        else
                        {
                            col = TemperateGrassColor;

                            if (temptrees)
                            {
                                col = TemperateForestColor;
                                float dx = x / (float)this.Width;
                                float dy = y / (float)(this.Height);
                                int tx = (int)(this.Trees.Width * dx);
                                int ty = (int)(this.Trees.Height * dy);
                                if (RandomIntHelper.Next(4) == 0)
                                    if (height > 107 / 255.0f)
                                    {
                                        this.Trees.SetPixel(tx, ty, Color.FromArgb(255, 76, 156, 51));
                                    }
                            }
                            else
                            {
                                if (RandomIntHelper.Next(18) == 0)
                                {
                                    float dx = x / (float)this.Width;
                                    float dy = y / (float)this.Height;
                                    int tx = (int)(this.Trees.Width * dx);
                                    int ty = (int)(this.Trees.Height * dy);
                                    if (height > 107 / 255.0f)
                                    {
                                        this.Trees.SetPixel(tx, ty, Color.FromArgb(255, 76, 156, 51));
                                    }
                                }
                            }
                        }


                        //   height += no * 2.0f;
                        //     height -= sealevel;

                        if (height > highmountainLevel)
                        {
                            if (hot)
                            {
                                col = SandMountainColor;
                            }
                            else
                            {
                                col = SnowMountainTopColor;
                            }
                        }

                        else if (height > mountainLevel)
                        {
                            if (hot)
                            {
                                col = HillsDesertColor;
                            }
                            else
                            {
                                col = MountainRockColor;
                            }
                        }
                        else if (height > hillLevel)
                        {
                            if (hot)
                            {
                            }
                            else
                            {
                                col = SteppesColor;
                            }
                        }
                        else
                        {
                            var ccol = MapGenManager.instance.ProvinceMap.GetPixel(x, y);

                            if (ccol.R == 0 && ccol.G == 0 && ccol.B == 0)
                            {
                                if (!hot && !slightlyHot)
                                {
                                    col = SteppesColor;
                                }
                                else
                                {
                                    col = DesertSandColor;
                                    float dx = x / (float)this.Width;
                                    float dy = y / (float)(this.Height);
                                    int tx = (int)(this.Trees.Width * dx);
                                    int ty = (int)(this.Trees.Height * dy);
                                    this.Trees.SetPixel(tx, ty, Color.FromArgb(255, 0, 0, 0));
                                }
                            }
                        }
                    }

                    this.TerrainMap.SetPixel(x, y, col);
                }

            MapGenManager.instance.ProvinceMap.UnlockBits();
            this.TerrainMap.UnlockBits();
            this.HeatMap.UnlockBits();
            this.MoistureMap.UnlockBits();

            this.MoistureMap.Source.Dispose();
          //  HeatMap.Source.Dispose();
          //  HeatMap = null;
            this.MoistureMap = null;
            this.HeatMap.Source.Dispose();
            this.HeatMap = null;
            this.Map.UnlockBits();
            this.Trees.UnlockBits();
        }



        private void CreateInitialTerrainMap()
        {
            this.Map.LockBits();
            this.TerrainMap.LockBits();
              for (int x = 0; x < this.Map.Width; x++)
                for (int y = 0; y < this.Map.Height; y++)
                {
                    Color col = Color.AliceBlue;

                    float height = this.Map.GetHeight(x, y) / 255.0f;

                    float sealevel = (96 / 255.0f);
                    bool sea = height < sealevel;
                    if (sea)
                    {
                        col = Color.FromArgb(255, 69, 91, 186);
                    }
                    else
                    {
                        col = Color.FromArgb(255, 130, 158, 75);
                    }

                    this.TerrainMap.SetPixel(x, y, col);
                }

            this.TerrainMap.UnlockBits();
            this.Map.UnlockBits();
        }

        private float GetHeat(int x, int y)
        {
            return this.HeatMap.GetPixel(x, y).R / 255.0f;
        }

        private float GetMoisture(int x, int y)
        {
            return this.MoistureMap.GetPixel(x/4, y/4).R / 255.0f;
        }

        public void CreateHeatMap()
        {
            NoiseGenerator noise = new NoiseGenerator();
            this.HeatMap.LockBits();
            for (int x = 0; x < this.HeatMap.Width; x++)
                for (int y = 0; y < this.HeatMap.Height; y++)
                {
                    float orig = (float)noise.Noise(x / this.DivNoise / 4, y / this.DivNoise / 4);

                    float ydel = y / (float)(this.HeatMap.Height);

                    if (Globals.Climate == 0)
                    {
                        if (ydel > 0.5f)
                        {
                            ydel -= 0.5f;
                            ydel *= 2.0f;
                        }
                        else
                            ydel = (0.5f - ydel) * 2.0f;

                        ydel = 1.0f - ydel;
                    }

                    if (Globals.Climate == 1)
                    {
                        ydel *= 1.3f;
                        if (ydel > 1)
                        {
                            ydel = 1;
                        }
                    }

                    if (Globals.Climate == 2)
                    {
                        ydel *= 1.3f;
                        if (ydel > 1)
                        {
                            ydel = 1;
                        }

                        ydel = 1.0f - ydel;
                    }

                    if (Globals.Climate == 3)
                    {
                        ydel = 0.4f;
                    }

                    if (Globals.Climate == 4)
                    {
                        ydel = 1f;
                    }

                    float o = orig;
                    orig *= ydel;
                    //  ydel *= ydel;

                    orig *= ydel;
                    orig = (orig + orig + ydel) / 3.0f;


                    orig = (orig + orig + o) / 3.0f;
                    orig *= orig;
                    if (Globals.Climate == 0)
                    {
                        orig = (orig + orig + orig + orig + orig + ydel)/6.0f;
                    }

                    if (orig > 1)
                    {
                        orig = 1;
                    }

                    this.HeatMap.SetPixel(x, y, Color.FromArgb(255, (int)(255 * orig), (int)(255 * orig), (int)(255 * orig)));
                }

            this.HeatMap.UnlockBits();
            this.HeatMap.ResizeImage(this.Map.Source.Width / 16, this.Map.Source.Height / 16);
            this.HeatMap.ResizeImage(this.Map.Source.Width, this.Map.Source.Height);

            this.HeatMap.Save24(Globals.MapOutputTotalDir + "heatmap.bmp");
        }


        public void AdjustMoistureMap()
        {
            this.MoistureMap.LockBits();
            this.Map.LockBits();
            for (int x = 0; x < this.MoistureMap.Width; x++)
                for (int y = 0; y < this.MoistureMap.Height; y++)
                {
                    float orig = (float)this.MoistureMap.GetPixel(x, y).R / 255.0f;

                    float height = this.Map.GetHeight(x*4, y * 4) / 255.0f;
                    if (height*255.0f < 102)
                    {
                        orig = 1.0f;
                    }

                    if (orig > 1.0f)
                    {
                        orig = 1.0f;
                    }

                    height = 1.0f - height;
                    if(height > 0.54f)
                    {
                        orig = (orig + orig + 1) / 3.0f;
                    }

                    this.MoistureMap.SetPixel(x, y, Color.FromArgb(255, (int)(255 * orig), (int)(255 * orig), (int)(255 * orig)));
                }

            this.Map.UnlockBits();
            this.MoistureMap.UnlockBits();
            this.MoistureMap.ResizeImage(this.MoistureMap.Source.Width / 16, this.MoistureMap.Source.Height / 16);
            this.MoistureMap.ResizeImage(this.Width, this.Height);

        //    MoistureMap.Save24(Globals.MapOutputTotalDir + "moisture.bmp");
        }

        public void CreateMoistureMap()
        {
            NoiseGenerator noise = new NoiseGenerator();
            this.MoistureMap.LockBits();
            for (int x = 0; x < this.MoistureMap.Width; x++)
                for (int y = 0; y < this.MoistureMap.Height; y++)
                {
                    float orig = (float)noise.Noise(x / this.DivNoise / 4, y / this.DivNoise / 4);
                    orig *= orig;
                    float ydel = y / (float)this.MoistureMap.Height;
                    if (ydel > 0.5f)
                    {
                        ydel -= 0.5f;
                        ydel *= 2.0f;
                    }
                    else
                        ydel = (0.5f - ydel) * 2.0f;

                    orig = (orig + orig +  ydel) / 3.0f;

                    this.MoistureMap.SetPixel(x, y, Color.FromArgb(255, (int)(255 * orig), (int)(255 * orig), (int)(255 * orig)));
                }

            this.MoistureMap.UnlockBits();
            this.MoistureMap.ResizeImage(this.Map.Source.Width / 32, this.Map.Source.Height / 32);
            this.MoistureMap.ResizeImage(this.Width, this.Height);
        }

        private void CreateHeightMap(LockBitmap provinceMap)
        {
            this.Map.LockBits();
            provinceMap.LockBits();
            //   NewMap.LockBits();
            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                   float height = this.Map.GetHeight(x, y) / 255.0f;

                    if (height < 76 / 255.0f)
                    {
                        height = 76 / 255.0f;
                    }

                    if (height >= 96 &&
                        ProvinceBitmapManager.instance.colorProvinceMap[provinceMap.GetPixel(x, y)].isSea)
                    {
                        height = 94;
                    }

                    this.Map.SetPixel(x, y, Color.FromArgb(255, (int)(255 * height), (int)(255 * height), (int)(255 * height)));
                }
            }

            provinceMap.UnlockBits();

            ConvertTo24bpp(provinceMap.Source).Save(Globals.MapOutputTotalDir + "map\\provinces.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            provinceMap = null;
            ProvinceBitmapManager.instance.colorProvinceMap.Clear();
            ProvinceBitmapManager.instance.colorMap.Clear();
            this.Map.UnlockBits();
        }

        private float Lerp(float a, float b, float d)
        {
            float dif = b - a;

            dif *= d;

            return a + dif;
        }

        private TerrainMap noiseTerrain;

        private void CreateInitialHeightMap(LockBitmap landBitmap = null, LockBitmap hillBitmap = null, LockBitmap mountainBitmap = null)
        {
            NoiseGenerator noise = new NoiseGenerator(this.heightSeed1);
            NoiseGenerator noise2 = new NoiseGenerator(this.heightSeed2);
            this.noiseTerrain= new TerrainMap(landBitmap, hillBitmap, mountainBitmap);
            if(landBitmap != null)
                this.noiseTerrain.Init(RandomIntHelper.Next(1000000), this.Width*4, this.Height * 4);
            else
            {
                this.noiseTerrain.InitGen(RandomIntHelper.Next(1000000), this.Width * 4, this.Height * 4);
            }

            this.Map = this.noiseTerrain.ResultBitmap;
        }

        public LockBitmap TerrainMap { get; set; }

        public LockBitmap Map { get; set; }


        public int Height { get; set; }

        public int Width { get; set; }

        public void UpdateFromProvinces()
        {
            this.Width *= 4;
            this.Height *= 4;
            this.TerrainMap.LockBits();

            this.Refine();

            File.Delete(Globals.MapOutputTotalDir + "map\\terrain.bmp");
            GrayBMP_File.CreateGrayBitmapFile(this.TerrainMap.Source, Globals.MapOutputTotalDir + "map\\terrain.bmp");
            this.TerrainMap.Source.Dispose();
            this.TerrainMap = null;
            GrayBMP_File.CreateGrayBitmapFile(this.Trees.Source, Globals.MapOutputTotalDir + "map\\trees.bmp", true);
            LockBitmap rivers = new LockBitmap(this.Width, this.Height);
            rivers.LockBits();
            for (int x = 0; x < this.Width; x++)
                for (int y = 0; y < this.Height; y++)
                {
                    rivers.SetPixel(x, y, Color.FromArgb(255, 255, 0, 128));
                }

            rivers.UnlockBits();


            GrayBMP_File.CreateGrayBitmapFile(rivers.Source, Globals.MapOutputTotalDir + "map\\rivers.bmp", false, true);
        }
    }
}
