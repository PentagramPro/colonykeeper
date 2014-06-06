
public struct MapPoint
{
	int x, z;
	public int X{
		get{
			return x;
		}
		set{
			x=value;
		}
	}
	public int Z{
		get{
			return z;
		}
		set{
			z=value;
		}
	}

	public MapPoint (int x, int z)
	{
		this.x=x;
		this.z=z;
	}
}


