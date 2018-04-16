// <copyright file="Polygon.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace csDelaunay
{
    using System;
    using System.Collections.Generic;

    public class Polygon {
		private List<Vector2f> vertices;

		public Polygon(List<Vector2f> vertices) {
			this.vertices = vertices;
		}

		public float Area() {
			return Math.Abs(this.SignedDoubleArea() * 0.5f);
		}

		public Winding PolyWinding() {
			float signedDoubleArea = this.SignedDoubleArea();
			if (signedDoubleArea < 0) {
				return Winding.CLOCKWISE;
			}

			if (signedDoubleArea > 0) {
				return Winding.COUNTERCLOCKWISE;
			}

			return Winding.NONE;
		}

		private float SignedDoubleArea() {
			int index, nextIndex;
			int n = this.vertices.Count;
			Vector2f point, next;
			float signedDoubleArea = 0;

			for (index = 0; index < n; index++) {
				nextIndex = (index+1) % n;
				point = this.vertices[index];
				next = this.vertices[nextIndex];
				signedDoubleArea += point.x * next.y - next.x * point.y;
			}

			return signedDoubleArea;
		}
	}
}
