// <copyright file="Triangle.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace csDelaunay
{
    using System.Collections.Generic;

    public class Triangle {
		private List<Site> sites;

		public List<Site> Sites {get{return this.sites;}}

		public Triangle(Site a, Site b, Site c) {
			this.sites = new List<Site>();
			this.sites.Add(a);
			this.sites.Add(b);
			this.sites.Add(c);
		}

		public void Dispose() {
			this.sites.Clear();
		}
	}
}