
public struct MapPoint
{
	int x, y;
	public int X{
		get{
			return x;
		}
		set{
			x=value;
		}
	}
	public int Y{
		get{
			return y;
		}
		set{
			y=value;
		}
	}

	public MapPoint (int x, int y)
	{
		this.x=x;
		this.y=y;
	}
}


