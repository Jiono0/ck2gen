// <copyright file="AbstractFloodFiller.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace FloodFill2
{
    using System.Diagnostics;
    using System.Drawing;
    using LibNoise.Modfiers;

    /// <summary>
    ///
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public delegate void UpdateScreenDelegate(ref int x, ref int y);

    /// <summary>
    /// The base class that the flood fill algorithms inherit from. Implements the
    /// basic flood filler functionality that is the same across all algorithms.
    /// </summary>
    public abstract class AbstractFloodFiller
    {
        protected LockBitmap bitmap;
        protected byte[] tolerance = new byte[] { 0, 0, 0 };
        protected Color fillColor = Color.Magenta;
        protected bool fillDiagonally = false;
        protected bool slow = false;

        //cached bitmap properties
        protected int bitmapWidth = 0;
        protected int bitmapHeight = 0;
        protected int bitmapStride = 0;
        protected int bitmapPixelFormatSize = 0;
        protected byte[] bitmapBits = null;

        //internal int timeBenchmark = 0;
        internal Stopwatch watch = new Stopwatch();
        internal UpdateScreenDelegate UpdateScreen;

        //internal, initialized per fill
        //protected BitArray pixelsChecked;
        protected bool[] pixelsChecked;
        protected byte[] byteFillColor;
        protected byte[] startColor;
        //protected int stride;

        public AbstractFloodFiller()
        {
        }

        public AbstractFloodFiller(AbstractFloodFiller configSource)
        {
            if (configSource != null)
            {
                this.Bitmap = configSource.Bitmap;
                this.FillColor = configSource.FillColor;
                this.FillDiagonally = configSource.FillDiagonally;
                this.Slow = configSource.Slow;
                this.Tolerance = configSource.Tolerance;
            }
        }

        public bool Slow
        {
            get { return this.slow; }
            set { this.slow = value; }
        }

        public Color FillColor
        {
            get { return this.fillColor; }
            set { this.fillColor = value; }
        }

        public bool FillDiagonally
        {
            get { return this.fillDiagonally; }
            set { this.fillDiagonally = value; }
        }

        public byte[] Tolerance
        {
            get { return this.tolerance; }
            set { this.tolerance = value; }
        }

        public LockBitmap Bitmap
        {
            get { return this.bitmap; }

            set
            {
                this.bitmap = value;
            }
        }

        public abstract void FloodFill(Point pt);

        protected void PrepareForFloodFill(Point pt)
        {
            //cache data in member variables to decrease overhead of property calls
            //this is especially important with Width and Height, as they call
            //GdipGetImageWidth() and GdipGetImageHeight() respectively in gdiplus.dll -
            //which means major overhead.
            this.byteFillColor = new byte[] { this.fillColor.B, this.fillColor.G, this.fillColor.R };
            this.bitmapStride=this.bitmap.Stride;
            this.bitmapPixelFormatSize=this.bitmap.Depth/8;

            this.bitmapWidth = this.bitmap.Width;
            this.bitmapHeight = this.bitmap.Height;

            this.pixelsChecked = new bool[(this.bitmap.Width* this.bitmap.Height)];
        }
    }
}
