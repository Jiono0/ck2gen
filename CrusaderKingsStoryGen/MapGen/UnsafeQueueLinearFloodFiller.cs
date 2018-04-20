// <copyright file="UnsafeQueueLinearFloodFiller.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace FloodFill2
{
    using System.Collections.Generic;
    using System.Drawing;

    /// <summary>
    /// Implements the QueueLinear flood fill algorithm using pointer-based pixel manipulation.
    /// </summary>
    public class UnsafeQueueLinearFloodFiller : AbstractFloodFiller
    {
        protected unsafe byte* scan0;
        FloodFillRangeQueue ranges = new FloodFillRangeQueue();
        public List<Point> pts = new List<Point>(100000);

        public UnsafeQueueLinearFloodFiller(AbstractFloodFiller configSource) : base(configSource) { }

        public override void FloodFill(System.Drawing.Point pt)
        {
            this.pts.Clear();
            this.watch.Reset();
            this.watch.Start();
            this.PrepareForFloodFill(pt);

            unsafe
            {
                this.bitmapStride = this.bitmap.Stride;
                this.scan0 = (byte*)this.bitmap.Iptr;
                int x = pt.X; int y = pt.Y;
                int loc = this.CoordsToIndex(ref x, ref y);
                byte* colorPtr = ((byte*)(this.scan0 + loc));
                this.startColor = new byte[] { colorPtr[0], colorPtr[1], colorPtr[2] };
                this.LinearFloodFill4(ref x, ref y);

                bool[] pixelsChecked = this.pixelsChecked;

                while (this.ranges.Count > 0)
                {
                    FloodFillRange range = this.ranges.Dequeue();

                    //START THE LOOP UPWARDS AND DOWNWARDS
                    int upY = range.Y - 1; //so we can pass the y coord by ref
                    int downY = range.Y + 1;
                    byte* upPtr = (byte*)(this.scan0 + this.CoordsToIndex(ref range.StartX, ref upY));
                    byte* downPtr = (byte*)(this.scan0 + this.CoordsToIndex(ref range.StartX, ref downY));
                    int downPxIdx = (this.bitmapWidth * (range.Y + 1)) + range.StartX; //CoordsToPixelIndex(range.StartX,range.Y+1);
                    int upPxIdx = (this.bitmapWidth * (range.Y - 1)) + range.StartX; //CoordsToPixelIndex(range.StartX, range.Y - 1);
                    for (int i = range.StartX; i <= range.EndX; i++)
                    {
                        //START LOOP UPWARDS
                        //if we're not above the top of the bitmap and the pixel above this one is within the color tolerance
                        if (range.Y > 0 && this.CheckPixel(ref upPtr) && (!(pixelsChecked[upPxIdx])))
                        {
                            this.LinearFloodFill4(ref i, ref upY);
                        }
                        //START LOOP DOWNWARDS
                        if (range.Y < (this.bitmapHeight - 1) && this.CheckPixel(ref downPtr) && (!(pixelsChecked[downPxIdx])))
                        {
                            this.LinearFloodFill4(ref i, ref downY);
                        }

                        upPtr += this.bitmapPixelFormatSize;
                        downPtr += this.bitmapPixelFormatSize;
                        downPxIdx++;
                        upPxIdx++;
                    }
                }
            }

            this.watch.Stop();
        }

        unsafe void LinearFloodFill4(ref int x, ref int y)
        {
            //offset the pointer to the point passed in
            byte* p = (byte*)(this.scan0 + (this.CoordsToIndex(ref x, ref y)));

            //cache some bitmap and fill info in local variables for a little extra speed
            bool[] pixelsChecked = this.pixelsChecked;
            byte[] byteFillColor = this.byteFillColor;
            int bitmapPixelFormatSize = this.bitmapPixelFormatSize;
            int bitmapWidth = this.bitmapWidth;

            //FIND LEFT EDGE OF COLOR AREA
            int lFillLoc = x; //the location to check/fill on the left
            byte* ptr = p; //the pointer to the current location
            int pxIdx = (bitmapWidth * y) + x;
            while (true)
            {
                ptr[0] = byteFillColor[0]; 	 //fill with the color
                ptr[1] = byteFillColor[1];
                ptr[2] = byteFillColor[2];
                pixelsChecked[pxIdx] = true;
                lFillLoc--; 		 	 //de-increment counter
                ptr -= bitmapPixelFormatSize;				 	 //de-increment pointer
                pxIdx--;
                if (lFillLoc <= 0 || !this.CheckPixel(ref ptr) || (pixelsChecked[pxIdx]))
                {
                    break;               //exit loop if we're at edge of bitmap or color area
                }
            }

            lFillLoc++;

            //FIND RIGHT EDGE OF COLOR AREA
            int rFillLoc = x; //the location to check/fill on the left
            ptr = p;
            pxIdx = (bitmapWidth * y) + x;
            while (true)
            {
                ptr[0] = byteFillColor[0]; 	 //fill with the color
                ptr[1] = byteFillColor[1];
                ptr[2] = byteFillColor[2];
                pixelsChecked[pxIdx] = true;
                rFillLoc++; 		 //increment counter
                ptr += bitmapPixelFormatSize;				 //increment pointer
                pxIdx++;
                if (rFillLoc >= bitmapWidth || !this.CheckPixel(ref ptr) || (pixelsChecked[pxIdx]))
                {
                    break;           //exit loop if we're at edge of bitmap or color area
                }
            }

            rFillLoc--;
            for (int xx = lFillLoc; xx <= rFillLoc; xx++)
            {
                this.pts.Add(new Point(xx, y));
            }

            FloodFillRange r = new FloodFillRange(lFillLoc, rFillLoc, y);
            this.ranges.Enqueue(ref r);
        }

        private unsafe bool CheckPixel(ref byte* px)
        {
            return
                px[0] >= (this.startColor[0] - this.tolerance[0]) && px[0] <= (this.startColor[0] + this.tolerance[0]) &&
                px[1] >= (this.startColor[1] - this.tolerance[1]) && px[1] <= (this.startColor[1] + this.tolerance[1]) &&
                px[2] >= (this.startColor[2] - this.tolerance[2]) && px[2] <= (this.startColor[2] + this.tolerance[2]);
        }

        private int CoordsToIndex(ref int x, ref int y)
        {
            return (this.bitmapStride * y) + (x * this.bitmapPixelFormatSize);
        }
    }
}
