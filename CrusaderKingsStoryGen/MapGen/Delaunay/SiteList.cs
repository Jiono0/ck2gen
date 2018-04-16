// <copyright file="SiteList.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace csDelaunay
{
    using System;
    using System.Collections.Generic;

    public class SiteList {
		private List<Site> sites;
		private int currentIndex;

		private bool sorted;

		public SiteList() {
			this.sites = new List<Site>();
			this.sorted = false;
		}

		public void Dispose() {
			this.sites.Clear();
		}

		public int Add(Site site) {
			this.sorted = false;
			this.sites.Add(site);
			return this.sites.Count;
		}

		public int Count() {
			return this.sites.Count;
		}

		public Site Next() {
			if (!this.sorted) {
				throw new Exception("SiteList.Next(): sites have not been sorted");
			}

			if (this.currentIndex < this.sites.Count) {
				return this.sites[this.currentIndex++];
			} else {
				return null;
			}
		}

		public Rectf GetSitesBounds() {
			if (!this.sorted) {
				this.SortList();
				this.ResetListIndex();
			}

			float xmin, xmax, ymin, ymax;
			if (this.sites.Count == 0) {
				return Rectf.zero;
			}

			xmin = float.MaxValue;
			xmax = float.MinValue;
			foreach (Site site in this.sites) {
				if (site.x < xmin)
                {
                    xmin = site.x;
                }

                if (site.x > xmax)
                {
                    xmax = site.x;
                }
            }

			// here's where we assume that the sites have been sorted on y:
			ymin = this.sites[0].y;
			ymax = this.sites[this.sites.Count-1].y;

			return new Rectf(xmin, ymin, xmax - xmin, ymax - ymin);
		}

		public List<Vector2f> SiteCoords() {
			List<Vector2f> coords = new List<Vector2f>();
			foreach (Site site in this.sites) {
				coords.Add(site.Coord);
			}

			return coords;
		}

		/*
		 *
		 * @return the largest circle centered at each site that fits in its region;
		 * if the region is infinite, return a circle of radius 0.
		 */
		public List<Circle> Circles() {
			List<Circle> circles = new List<Circle>();
			foreach (Site site in this.sites) {
				float radius = 0;
				Edge nearestEdge = site.NearestEdge();

				if (!nearestEdge.IsPartOfConvexHull())
                {
                    radius = nearestEdge.SitesDistance() * 0.5f;
                }

                circles.Add(new Circle(site.x,site.y, radius));
			}

			return circles;
		}

		public List<List<Vector2f>> Regions(Rectf plotBounds) {
			List<List<Vector2f>> regions = new List<List<Vector2f>>();
			foreach (Site site in this.sites) {
				regions.Add(site.Region(plotBounds));
			}

			return regions;
		}

		public void ResetListIndex() {
			this.currentIndex = 0;
		}

		public void SortList() {
			Site.SortSites(this.sites);
			this.sorted = true;
		}
	}
}
