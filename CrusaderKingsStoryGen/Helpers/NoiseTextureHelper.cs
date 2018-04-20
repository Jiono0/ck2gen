// <copyright file="NoiseTexture.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace CrusaderKingsStoryGen.Helpers
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using LibNoise;
    using LibNoise.Modfiers;

    public class NoiseTextureHelper : LockBitmap
    {
        private double[,] heights;
        public double minRange = 100000;
        public double maxRange = -100000;
        public float DefaultFrequencyDiv = 150.0f;
        private float capMin = -1;
        private float capMax = -1;

        private NoiseTextureHelper(Bitmap source) : base(source)
        {
        }


        public static implicit operator Bitmap(NoiseTextureHelper tex)
        {
            return tex.Source;
        }

        public NoiseTextureHelper(int width, int height, IModule source, float delta = 1.0f, float capMax = -1, float capMin = -1)
        {
            this.Source = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette pal = this.Source.Palette;

            for (int i = 0; i < 256; i++)
            {
                pal.Entries[i] = Color.FromArgb(255, i, i, i);
            }

            this.Source.Palette = pal;

            this.capMax = capMax;
            this.capMin = capMin;
            this.DefaultFrequencyDiv = this.DefaultFrequencyDiv * delta;
            this.heights = new double[width,height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    this.SetHeight(x, y, (float)((source.GetValue(x / this.DefaultFrequencyDiv, 0, y / this.DefaultFrequencyDiv))));
                }
            }

            this.ApplyNoise();
        }

        public float GetHeightDelta(int x, int y)
        {
            return (float)((this.heights[x, y] - this.minRange) / this.Range);
        }

        private void ApplyNoise()
        {
            this.LockBits();
            if (this.capMax != -1)
            {
                this.maxRange = this.capMax;
                this.minRange = this.capMin;
            }

            double range = this.Range;

            for (int x = 0; x < this.Width; x++)
            {
                for (int y = 0; y < this.Height; y++)
                {
                    double h = this.heights[x, y];


                    double adj = h - this.minRange;

                    adj = adj / range;
                    if (range == 0 || double.IsNaN(range) || double.IsInfinity(range))
                    {
                        adj = this.minRange;
                    }

                    this.SetPixel(x, y, (float)adj);
                }
            }

            this.UnlockBits();

            this.heights = null;
        }

        public double Range
        {
            get { return this.maxRange - this.minRange; }
        }

        private void SetHeight(int x, int y, double h)
        {
            if (h > this.maxRange)
            {
                this.maxRange = h;
            }

            if (h < this.minRange)
            {
                this.minRange = h;
            }

            this.heights[x, y] = h;
        }
    }
}
