// <copyright file="FloodFillRangeQueue.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace FloodFill2
{
    using System;

    /// <summary>A queue of FloodFillRanges.</summary>
	public class FloodFillRangeQueue
	{
        FloodFillRange[] array;
        int size;
        int head;

        /// <summary>
        /// Returns the number of items currently in the queue.
        /// </summary>
        public int Count
        {
            get { return this.size; }
        }

		public FloodFillRangeQueue():this(10000)
		{
		}

        public FloodFillRangeQueue(int initialSize)
        {
            this.array = new FloodFillRange[initialSize];
            this.head = 0;
            this.size = 0;
        }

        /// <summary>Gets the <see cref="FloodFillRange"/> at the beginning of the queue.</summary>
        public FloodFillRange First
		{
			get { return this.array[this.head]; }
		}

        /// <summary>Adds a <see cref="FloodFillRange"/> to the end of the queue.</summary>
        public void Enqueue(ref FloodFillRange r)
		{
			if (this.size+this.head == this.array.Length)
			{
                FloodFillRange[] newArray = new FloodFillRange[2 * this.array.Length];
                Array.Copy(this.array, this.head, newArray, 0, this.size);
				this.array = newArray;
                this.head = 0;
			}

            this.array[this.head+(this.size++)] = r;
		}

        /// <summary>Removes and returns the <see cref="FloodFillRange"/> at the beginning of the queue.</summary>
        public FloodFillRange Dequeue()
		{
            FloodFillRange range = new FloodFillRange();
            if (this.size>0)
            {
                range = this.array[this.head];
                this.array[this.head] = new FloodFillRange();
                this.head++;//advance head position
                this.size--;//update size to exclude dequeued item
            }

            return range;
		}

        /// <summary>Remove all FloodFillRanges from the queue.</summary>
		/*public void Clear()
		{
			if (size > 0)
				Array.Clear(array, 0, size);
			size = 0;
		}*/
	}
}
