// <copyright file="Halfedge.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace csDelaunay
{
    using System.Collections.Generic;

    public class Halfedge {
		#region Pool
		private static Queue<Halfedge> pool = new Queue<Halfedge>();

		public static Halfedge Create(Edge edge, LR lr) {
			if (pool.Count > 0) {
				return pool.Dequeue().Init(edge,lr);
			} else {
				return new Halfedge(edge,lr);
			}
		}

		public static Halfedge CreateDummy() {
			return Create(null, null);
		}
		#endregion

		#region Object
		public Halfedge edgeListLeftNeighbor;
		public Halfedge edgeListRightNeighbor;
		public Halfedge nextInPriorityQueue;

		public Edge edge;
		public LR leftRight;
		public Vertex vertex;

		// The vertex's y-coordinate in the transformed Voronoi space V
		public float ystar;

		public Halfedge(Edge edge, LR lr) {
			this.Init(edge, lr);
		}

		private Halfedge Init(Edge edge, LR lr) {
			this.edge = edge;
			this.leftRight = lr;
			this.nextInPriorityQueue = null;
			this.vertex = null;

			return this;
		}

		public override string ToString() {
			return "Halfedge (LeftRight: " + this.leftRight + "; vertex: " + this.vertex + ")";
		}

		public void Dispose() {
			if (this.edgeListLeftNeighbor != null || this.edgeListRightNeighbor != null) {
				// still in EdgeList
				return;
			}

			if (this.nextInPriorityQueue != null) {
				// still in PriorityQueue
				return;
			}

			this.edge = null;
			this.leftRight = null;
			this.vertex = null;
			pool.Enqueue(this);
		}

		public void ReallyDispose() {
			this.edgeListLeftNeighbor = null;
			this.edgeListRightNeighbor = null;
			this.nextInPriorityQueue = null;
			this.edge = null;
			this.leftRight = null;
			this.vertex = null;
			pool.Enqueue(this);
		}

		public bool IsLeftOf(Vector2f p) {
			Site topSite;
			bool rightOfSite, above, fast;
			float dxp, dyp, dxs, t1, t2, t3, y1;

			topSite = this.edge.RightSite;
			rightOfSite = p.x > topSite.x;
			if (rightOfSite && this.leftRight == LR.LEFT) {
				return true;
			}

			if (!rightOfSite && this.leftRight == LR.RIGHT) {
				return false;
			}

			if (this.edge.a == 1) {
				dyp = p.y - topSite.y;
				dxp = p.x - topSite.x;
				fast = false;
				if ((!rightOfSite && this.edge.b < 0) || (rightOfSite && this.edge.b >= 0)) {
					above = dyp >= this.edge.b * dxp;
					fast = above;
				} else {
					above = p.x + p.y * this.edge.b > this.edge.c;
					if (this.edge.b < 0) {
						above = !above;
					}

					if (!above) {
						fast = true;
					}
				}

				if (!fast) {
					dxs = topSite.x - this.edge.LeftSite.x;
					above = this.edge.b * (dxp * dxp - dyp * dyp) < dxs * dyp * (1 + 2 * dxp / dxs + this.edge.b * this.edge.b);
					if (this.edge.b < 0) {
						above = !above;
					}
				}
			} else {
				y1 = this.edge.c - this.edge.a * p.x;
				t1 = p.y - y1;
				t2 = p.x - topSite.x;
				t3 = y1 - topSite.y;
				above = t1 * t1 > t2 * t2 + t3 * t3;
			}

			return this.leftRight == LR.LEFT ? above : !above;
		}
		#endregion
	}
}
