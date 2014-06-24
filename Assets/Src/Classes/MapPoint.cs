using UnityEngine;
using System;

[Serializable]
public class MapPoint
{
	public int X;
	public int Z;

	public MapPoint()
	{
		X = 0;Z = 0;
	}
	public MapPoint (int x, int z)
	{
		this.X=x;
		this.Z=z;
	}
}


