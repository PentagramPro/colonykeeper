using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Map
{
	//these variables are used to properly create map object after serialization
	
	public int width;
	
	public int height;

	BlockController[,] map;
	Vector3[,,] mapVertexes;

	[SerializeField]
	int segments;



	public int Segments{
		get{
			return segments;
		}
	}
	public Vector3[,,] MapVertexes
	{
		get{
			return mapVertexes;
		}
	}
	public Map()
	{

	}

	public Map(int sizeX, int sizeZ, int segments)
	{
		this.segments = segments;
		width = sizeX;
		height = sizeZ;

		Restore();

	
					//mapVertexes [i, j, k] = new Vector3((i%9==0 )? 0.1f : 0, 0, 0);
	}

	public void Restore()
	{
		map = new BlockController[width,height];
		
		
		mapVertexes = new Vector3[width * (segments + 1), segments + 1, height * (segments + 1)];
		for (int i=0; i<mapVertexes.GetLength(0); i++)
			for (int j=0; j<mapVertexes.GetLength(1); j++)
				for (int k=0; k<mapVertexes.GetLength(2); k++)
					mapVertexes [i, j, k] = new Vector3(Random.Range(-0.1f,0.1f),
					                                    Random.Range(-0.03f,0.03f),
					                                    Random.Range(-0.1f,0.1f));

	}


	public void EnumerateCells(int xMin, int zMin, int xMax, int zMax, System.Action<int,int> method)
	{
		xMin = System.Math.Max(0,xMin);
		zMin = System.Math.Max(0,zMin);

		xMax = System.Math.Min(xMax, map.GetUpperBound(0));
		zMax = System.Math.Min(zMax, map.GetUpperBound(1));

		for(int x=xMin;x<=xMax;x++)
		{
			for(int z=zMin;z<=zMax;z++)
			{
				method(x,z);
			}
		}
	}
	public BlockController this [MapPoint pos] 
	{
		get{
			return map[pos.X,pos.Z];
		}
		
		set{
			map[pos.X,pos.Z]=value;
		}
	}

	public BlockController this [int x, int z] 
	{
		get{
			return map[x,z];
		}

		set{
			map[x,z]=value;
		}
	}

	// x axis
	public int Width{
		get{return map.GetLength(0);}
	}

	// z axis
	public int Height{
		get{return map.GetLength(1);}
	}

	public Vector3 GetVertex(IntVector3 pos)
	{
		return mapVertexes[pos.X,pos.Y,pos.Z];
	}

    public Vector3 GetLightAmount(Vector3 pos)
	{
		int cx = (int)pos.x;
		int cz = (int)pos.z;
		
		if(cx>=map.GetLength(0) || cz>=map.GetLength(1))
			return Vector3.zero;

        Vector3 res = Vector3.zero;
		foreach(StaticLight l in map[cx,cz].GetStaticLightsCache())
		{
			if(l.Owner==null)
				continue;
			Vector3 dir = pos-l.GlobalPosition;
            float mag = dir.sqrMagnitude * dir.sqrMagnitude * l.Falloff;
            Vector3 lightVal = new Vector3(
                l.Col.r / (mag),
                l.Col.g / (mag),
                l.Col.b / (mag)
                ) *l.Multiplier;
            res += lightVal;
		}
		
		
		return res;
	}


    public Vector3 GetLightAmount(IntVector3 pos)
	{
		return GetLightAmount(pos/(float)segments);
		/*
		int cx = pos.X/segments;
		int cz = pos.Z/segments;

		if(cx>=map.GetLength(0) || cz>=map.GetLength(1))
			return 0;

		float res = 0;
		foreach(StaticLight l in map[cx,cz].GetStaticLightsCache())
		{
			Vector3 dir = pos/(float)segments-l.GlobalPosition;

			res+=1/(dir.sqrMagnitude*2);
		}


		return res;*/
	}
}


