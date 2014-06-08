using System.Collections.Generic;
using UnityEngine;

public class Map
{
	BlockController[,] map;
	Vector3[,,] mapVertexes;
	int[,] lights;

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

	public Map(int sizeX, int sizeZ, int segments)
	{
		this.segments = segments;
		map = new BlockController[sizeX,sizeZ];
		lights = new int[sizeX,sizeZ];

		mapVertexes = new Vector3[sizeX * (segments + 1), segments + 1, sizeZ * (segments + 1)];
		for (int i=0; i<mapVertexes.GetLength(0); i++)
			for (int j=0; j<mapVertexes.GetLength(1); j++)
				for (int k=0; k<mapVertexes.GetLength(2); k++)
					mapVertexes [i, j, k] = new Vector3(Random.Range(-0.1f,0.1f),
					                                    Random.Range(-0.03f,0.03f),
					                                    Random.Range(-0.1f,0.1f));

		for(int i=0;i<lights.GetLength(0);i++)
			for(int j=0;j<lights.GetLength(1);j++)
				lights[i,j]=0;
					//mapVertexes [i, j, k] = new Vector3((i%9==0 )? 0.1f : 0, 0, 0);
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

	public bool IsLit(int x, int z)
	{
		if(x>0 && z>0 && x<lights.GetLength(0) && z<lights.GetLength(1))
			return lights[x,z]>0;

		return false;
	}

	public void SetLight(MapPoint pos, bool val)
	{
		lights[pos.X,pos.Z]+=val?1:-1;
	}

	public float GetLightAmount(IntVector3 pos)
	{
		int cx = pos.X/segments;
		int cz = pos.Z/segments;



		if(IsLit(cx,cz))
			return 1.0f;

		List<Vector2> lights = new List<Vector2>();
		for(int x=cx-1;x<=cx+1;x++)
		{
			for(int z=cz-1;z<=cz+1;z++)
			{
				if(IsLit(x,z))
					lights.Add(new Vector2(x+0.5f,z+0.5f));
			}
		}

		float res = 0;
		foreach(Vector2 l in lights)
		{
			Vector2 dir = new Vector2(pos.X/(float)segments-l.x,pos.Z/(float)segments-l.y);
			res+=1/(dir.sqrMagnitude*2);
		}

		return res;
	}
}


