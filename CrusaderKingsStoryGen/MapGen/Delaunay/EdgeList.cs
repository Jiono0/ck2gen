// <copyright file="EdgeList.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace csDelaunay
{
    public class EdgeList {
		private float deltaX;
		private float xmin;

		private int hashSize;
		private Halfedge[] hash;
		private Halfedge leftEnd;

		public Halfedge LeftEnd {get{return this.leftEnd;}}

		private Halfedge rightEnd;

		public Halfedge RightEnd {get{return this.rightEnd;}}

		public void Dispose() {
			Halfedge halfedge = this.leftEnd;
			Halfedge prevHe;
			while (halfedge != this.rightEnd) {
				prevHe = halfedge;
				halfedge = halfedge.edgeListRightNeighbor;
				prevHe.Dispose();
			}

			this.leftEnd = null;
			this.rightEnd.Dispose();
			this.rightEnd = null;

			this.hash = null;
		}

		public EdgeList(float xmin, float deltaX, int sqrtSitesNb) {
			this.xmin = xmin;
			this.deltaX = deltaX;
			this.hashSize = 2 * sqrtSitesNb;

			this.hash = new Halfedge[this.hashSize];

			// Two dummy Halfedges:
			this.leftEnd = Halfedge.CreateDummy();
			this.rightEnd = Halfedge.CreateDummy();
			this.leftEnd.edgeListLeftNeighbor = null;
			this.leftEnd.edgeListRightNeighbor = this.rightEnd;
			this.rightEnd.edgeListLeftNeighbor = this.leftEnd;
			this.rightEnd.edgeListRightNeighbor = null;
			this.hash[0] = this.leftEnd;
			this.hash[this.hashSize - 1] = this.rightEnd;
		}

		/*
		 * Insert newHalfedge to the right of lb
		 * @param lb
		 * @param newHalfedge
		 */
		public void Insert(Halfedge lb, Halfedge newHalfedge) {
			newHalfedge.edgeListLeftNeighbor = lb;
			newHalfedge.edgeListRightNeighbor = lb.edgeListRightNeighbor;
			lb.edgeListRightNeighbor.edgeListLeftNeighbor = newHalfedge;
			lb.edgeListRightNeighbor = newHalfedge;
		}

		/*
		 * This function only removes the Halfedge from the left-right list.
		 * We cannot dispose it yet because we are still using it.
		 * @param halfEdge
		 */
		public void Remove(Halfedge halfedge) {
			halfedge.edgeListLeftNeighbor.edgeListRightNeighbor = halfedge.edgeListRightNeighbor;
			halfedge.edgeListRightNeighbor.edgeListLeftNeighbor = halfedge.edgeListLeftNeighbor;
			halfedge.edge = Edge.DELETED;
			halfedge.edgeListLeftNeighbor = halfedge.edgeListRightNeighbor = null;
		}

		/*
		 * Find the rightmost Halfedge that is still elft of p
		 * @param p
		 * @return
		 */
		public Halfedge EdgeListLeftNeighbor(Vector2f p) {
			int bucket;
			Halfedge halfedge;

			// Use hash table to get close to desired halfedge
			bucket = (int)((p.x - this.xmin)/this.deltaX * this.hashSize);
			if (bucket < 0) {
				bucket = 0;
			}

			if (bucket >= this.hashSize) {
				bucket = this.hashSize - 1;
			}

			halfedge = this.GetHash(bucket);
			if (halfedge == null) {
				for (int i = 0; true; i++) {
					if ((halfedge = this.GetHash(bucket - i)) != null)
                    {
                        break;
                    }

                    if ((halfedge = this.GetHash(bucket + i)) != null)
                    {
                        break;
                    }
                }
			}

			// Now search linear list of haledges for the correct one
			if (halfedge == this.leftEnd || (halfedge != this.rightEnd && halfedge.IsLeftOf(p))) {
				do {
					halfedge = halfedge.edgeListRightNeighbor;
				} while (halfedge != this.rightEnd && halfedge.IsLeftOf(p));
				halfedge = halfedge.edgeListLeftNeighbor;
			} else {
				do {
					halfedge = halfedge.edgeListLeftNeighbor;
				} while (halfedge != this.leftEnd && !halfedge.IsLeftOf(p));
			}

			// Update hash table and reference counts
			if (bucket > 0 && bucket < this.hashSize - 1) {
				this.hash[bucket] = halfedge;
			}

			return halfedge;
		}

		// Get entry from the has table, pruning any deleted nodes
		private Halfedge GetHash(int b) {
			Halfedge halfedge;

			if (b < 0 || b >= this.hashSize) {
				return null;
			}

			halfedge = this.hash[b];
			if (halfedge != null && halfedge.edge == Edge.DELETED) {
				// Hash table points to deleted halfedge. Patch as necessary
				this.hash[b] = null;
				// Still can't dispose halfedge yet!
				return null;
			} else {
				return halfedge;
			}
		}
	}
}
