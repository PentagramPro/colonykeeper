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

	public void Clamp(int width, int height)
	{
		X = Mathf.Clamp(X,0,width-1);
		Z = Mathf.Clamp(Z,0,height-1);
	}
}


