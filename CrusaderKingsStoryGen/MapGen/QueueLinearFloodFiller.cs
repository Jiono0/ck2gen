// <copyright file="QueueLinearFloodFiller.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace FloodFill2
{
    /// <summary>
    /// Implements the QueueLinear flood fill algorithm using array-based pixel manipulation.
    /// </summary>
    public class QueueLinearFloodFiller : AbstractFloodFiller
    {
        //Queue of floodfill ranges. We use our own class to increase performance.
        //To use .NET Queue class, change this to:
        //<FloodFillRange> ranges = new Queue<FloodFillRange>();
        FloodFillRangeQueue ranges = new FloodFillRangeQueue();

        public QueueLinearFloodFiller(AbstractFloodFiller configSource) : base(configSource) { }

        /// <summary>
        /// Fills the specified point on the bitmap with the currently selected fill color.
        /// </summary>
        /// <param name="pt">The starting point for the fill.</param>
        public override void FloodFill(System.Drawing.Point pt)
        {
            this.watch.Reset();
            this.watch.Start();

            //***Prepare for fill.
            this.PrepareForFloodFill(pt);

            this.ranges = new FloodFillRangeQueue(((this.bitmapWidth+this.bitmapHeight)/2)*5);//new Queue<FloodFillRange>();

            //***Get starting color.
            int x = pt.X; int y = pt.Y;
            int idx = this.CoordsToByteIndex(ref x, ref y);
            this.startColor = new byte[] { this.bitmap.Pixels[idx], this.bitmap.Pixels[idx + 1], this.bitmap.Pixels[idx + 2] };

            bool[] pixelsChecked=this.pixelsChecked;

            //***Do first call to floodfill.
            this.LinearFill(ref x, ref y);

            //***Call floodfill routine while floodfill ranges still exist on the queue
            while (this.ranges.Count > 0)
            {
                //**Get Next Range Off the Queue
                FloodFillRange range = this.ranges.Dequeue();

	            //**Check Above and Below Each Pixel in the Floodfill Range
                int downPxIdx = (this.bitmapWidth * (range.Y + 1)) + range.StartX;//CoordsToPixelIndex(lFillLoc,y+1);
                int upPxIdx = (this.bitmapWidth * (range.Y - 1)) + range.StartX;//CoordsToPixelIndex(lFillLoc, y - 1);
                int upY=range.Y - 1;//so we can pass the y coord by ref
                int downY = range.Y + 1;
                int tempIdx;
                for (int i = range.StartX; i <= range.EndX; i++)
                {
                    //*Start Fill Upwards
                    //if we're not above the top of the bitmap and the pixel above this one is within the color tolerance
                    tempIdx = this.CoordsToByteIndex(ref i, ref upY);
                    if (range.Y > 0 && (!pixelsChecked[upPxIdx]) && this.CheckPixel(ref tempIdx))
                    {
                        this.LinearFill(ref i, ref upY);
                    }

                    //*Start Fill Downwards
                    //if we're not below the bottom of the bitmap and the pixel below this one is within the color tolerance
                    tempIdx = this.CoordsToByteIndex(ref i, ref downY);
                    if (range.Y < (this.bitmapHeight - 1) && (!pixelsChecked[downPxIdx]) && this.CheckPixel(ref tempIdx))
                    {
                        this.LinearFill(ref i, ref downY);
                    }

                    downPxIdx++;
                    upPxIdx++;
                }
            }

            this.watch.Stop();
        }

       /// <summary>
       /// Finds the furthermost left and right boundaries of the fill area
       /// on a given y coordinate, starting from a given x coordinate, filling as it goes.
       /// Adds the resulting horizontal range to the queue of floodfill ranges,
       /// to be processed in the main loop.
       /// </summary>
       /// <param name="x">The x coordinate to start from.</param>
       /// <param name="y">The y coordinate to check at.</param>
       void LinearFill(ref int x, ref int y)
        {
           //cache some bitmap and fill info in local variables for a little extra speed
           byte[] bitmapBits=this.bitmapBits;
           bool[] pixelsChecked=this.pixelsChecked;
           byte[] byteFillColor= this.byteFillColor;
           int bitmapPixelFormatSize=this.bitmapPixelFormatSize;
           int bitmapWidth=this.bitmapWidth;

            //***Find Left Edge of Color Area
            int lFillLoc = x; //the location to check/fill on the left
            int idx = this.CoordsToByteIndex(ref x, ref y); //the byte index of the current location
            int pxIdx = (bitmapWidth * y) + x;//CoordsToPixelIndex(x,y);
            while (true)
            {
                //**fill with the color
                bitmapBits[idx] = byteFillColor[0];
                bitmapBits[idx+1] = byteFillColor[1];
                bitmapBits[idx+2] = byteFillColor[2];
                //**indicate that this pixel has already been checked and filled
                pixelsChecked[pxIdx] = true;
                //**screen update for 'slow' fill
                if (this.slow)
                {
                    this.UpdateScreen(ref lFillLoc, ref y);
                }
                //**de-increment
                lFillLoc--;     //de-increment counter
                pxIdx--;        //de-increment pixel index
                idx -= bitmapPixelFormatSize;//de-increment byte index
                //**exit loop if we're at edge of bitmap or color area
                if (lFillLoc <= 0 || (pixelsChecked[pxIdx]) || !this.CheckPixel(ref idx))
                {
                    break;
                }
            }

            lFillLoc++;

            //***Find Right Edge of Color Area
            int rFillLoc = x; //the location to check/fill on the left
            idx = this.CoordsToByteIndex(ref x, ref y);
            pxIdx = (bitmapWidth * y) + x;
            while (true)
            {
                //**fill with the color
                bitmapBits[idx] = byteFillColor[0];
                bitmapBits[idx + 1] = byteFillColor[1];
                bitmapBits[idx + 2] = byteFillColor[2];
                //**indicate that this pixel has already been checked and filled
                pixelsChecked[pxIdx] = true;
                //**screen update for 'slow' fill
                if (this.slow)
                {
                    this.UpdateScreen(ref rFillLoc, ref y);
                }
                //**increment
                rFillLoc++;     //increment counter
                pxIdx++;        //increment pixel index
                idx += bitmapPixelFormatSize;//increment byte index
                //**exit loop if we're at edge of bitmap or color area
                if (rFillLoc >= bitmapWidth || pixelsChecked[pxIdx] || !this.CheckPixel(ref idx))
                {
                    break;
                }
            }

            rFillLoc--;

           //add range to queue
           FloodFillRange r = new FloodFillRange(lFillLoc, rFillLoc, y);
           this.ranges.Enqueue(ref r);
        }

        ///<summary>Sees if a pixel is within the color tolerance range.</summary>
        ///<param name="px">The byte index of the pixel to check, passed by reference to increase performance.</param>
        protected bool CheckPixel(ref int px)
        {
            //tried a 'for' loop but it adds an 8% overhead to the floodfill process
            /*bool ret = true;
            for (byte i = 0; i < 3; i++)
            {
                ret &= (bitmap.Bits[px] >= (startColor[i] - tolerance[i])) && bitmap.Bits[px] <= (startColor[i] + tolerance[i]);
                px++;
            }
            return ret;*/

            return (this.bitmapBits[px] >= (this.startColor[0] - this.tolerance[0])) && this.bitmapBits[px] <= (this.startColor[0] + this.tolerance[0]) &&
                (this.bitmapBits[px + 1] >= (this.startColor[1] - this.tolerance[1])) && this.bitmapBits[px + 1] <= (this.startColor[1] + this.tolerance[1]) &&
                (this.bitmapBits[px + 2] >= (this.startColor[2] - this.tolerance[2])) && this.bitmapBits[px + 2] <= (this.startColor[2] + this.tolerance[2]);
        }

        ///<summary>Calculates and returns the byte index for the pixel (x,y).</summary>
        ///<param name="x">The x coordinate of the pixel whose byte index should be returned.</param>
        ///<param name="y">The y coordinate of the pixel whose byte index should be returned.</param>
        protected int CoordsToByteIndex(ref int x, ref int y)
        {
            return (this.bitmapStride * y) + (x * this.bitmapPixelFormatSize);
        }

        /// <summary>
        /// Returns the linear index for a pixel, given its x and y coordinates.
        /// </summary>
        /// <param name="x">The x coordinate of the pixel.</param>
        /// <param name="y">The y coordinate of the pixel.</param>
        /// <returns></returns>
        protected int CoordsToPixelIndex(int x, int y)
        {
            return (this.bitmapWidth * y) + x;
        }
    }

    /// <summary>
    /// Represents a linear range to be filled and branched from.
    /// </summary>
    public struct FloodFillRange
    {
        public int StartX;
        public int EndX;
        public int Y;

        public FloodFillRange(int startX, int endX, int y)
        {
            this.StartX=startX;
            this.EndX = endX;
            this.Y = y;
        }
    }
}
