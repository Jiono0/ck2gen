// <copyright file="Rectf.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

public struct Rectf {
	public static readonly Rectf zero = new Rectf(0,0,0,0);
	public static readonly Rectf one = new Rectf(1,1,1,1);

	public float x,y,width,height;

	public Rectf(float x, float y, float width, float height) {
		this.x = x;
		this.y = y;
		this.width = width;
		this.height = height;
	}

	public float left {
		get {
			return this.x;}
	}

	public float right {
		get {
			return this.x+this.width;
		}
	}

	public float top {
		get {
			return this.y;
		}
	}

	public float bottom {
		get {
			return this.y+this.height;
		}
	}

	public Vector2f topLeft {
		get {
			return new Vector2f(this.left, this.top);
		}
	}

	public Vector2f bottomRight {
		get {
			return new Vector2f(this.right, this.bottom);
		}
	}
}
