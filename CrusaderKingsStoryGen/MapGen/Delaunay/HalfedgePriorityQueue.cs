// <copyright file="HalfedgePriorityQueue.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace csDelaunay
{
    // Also know as heap
    public class HalfedgePriorityQueue {
		private Halfedge[] hash;
		private int count;
		private int minBucked;
		private int hashSize;

		private float ymin;
		private float deltaY;

		public HalfedgePriorityQueue(float ymin, float deltaY, int sqrtSitesNb) {
			this.ymin = ymin;
			this.deltaY = deltaY;
			this.hashSize = 4 * sqrtSitesNb;
			this.Init();
		}

		public void Dispose() {
			// Get rid of dummies
			for (int i = 0; i < this.hashSize; i++) {
				this.hash[i].Dispose();
			}

			this.hash = null;
		}

		public void Init() {
			this.count = 0;
			this.minBucked = 0;
			this.hash = new Halfedge[this.hashSize];
			// Dummy Halfedge at the top of each hash
			for (int i = 0; i < this.hashSize; i++) {
				this.hash[i] = Halfedge.CreateDummy();
				this.hash[i].nextInPriorityQueue = null;
			}
		}

		public void Insert(Halfedge halfedge) {
			Halfedge previous, next;

			int insertionBucket = this.Bucket(halfedge);
			if (insertionBucket < this.minBucked) {
				this.minBucked = insertionBucket;
			}

			previous = this.hash[insertionBucket];
			while ((next = previous.nextInPriorityQueue) != null &&
			       (halfedge.ystar > next.ystar || (halfedge.ystar == next.ystar && halfedge.vertex.x > next.vertex.x))) {
				previous = next;
			}

			halfedge.nextInPriorityQueue = previous.nextInPriorityQueue;
			previous.nextInPriorityQueue = halfedge;
			this.count++;
		}

		public void Remove(Halfedge halfedge) {
			Halfedge previous;
			int removalBucket = this.Bucket(halfedge);

			if (halfedge.vertex != null) {
				previous = this.hash[removalBucket];
				while (previous.nextInPriorityQueue != halfedge) {
					previous = previous.nextInPriorityQueue;
				}

				previous.nextInPriorityQueue = halfedge.nextInPriorityQueue;
				this.count--;
				halfedge.vertex = null;
				halfedge.nextInPriorityQueue = null;
				halfedge.Dispose();
			}
		}

		private int Bucket(Halfedge halfedge) {
			int theBucket = (int)((halfedge.ystar - this.ymin) / this.deltaY * this.hashSize);
			if (theBucket < 0)
            {
                theBucket = 0;
            }

            if (theBucket >= this.hashSize)
            {
                theBucket = this.hashSize - 1;
            }

            return theBucket;
		}

		private bool IsEmpty(int bucket) {
			return (this.hash[bucket].nextInPriorityQueue == null);
		}

		/*
		 * move minBucket until it contains an actual Halfedge (not just the dummy at the top);
		 */
		private void AdjustMinBucket() {
			while (this.minBucked < this.hashSize - 1 && this.IsEmpty(this.minBucked)) {
				this.minBucked++;
			}
		}

		public bool Empty() {
			return this.count == 0;
		}

		/*
		 * @return coordinates of the Halfedge's vertex in V*, the transformed Voronoi diagram
		 */
		public Vector2f Min() {
			this.AdjustMinBucket();
			Halfedge answer = this.hash[this.minBucked].nextInPriorityQueue;
			return new Vector2f(answer.vertex.x, answer.ystar);
		}

		/*
		 * Remove and return the min Halfedge
		 */
		public Halfedge ExtractMin() {
			Halfedge answer;

			// Get the first real Halfedge in minBucket
			answer = this.hash[this.minBucked].nextInPriorityQueue;

			this.hash[this.minBucked].nextInPriorityQueue = answer.nextInPriorityQueue;
			this.count--;
			answer.nextInPriorityQueue = null;

			return answer;
		}
	}
}