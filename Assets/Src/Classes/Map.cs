using System.Collections.Generic;
using UnityEngine;

public class Map
{
	BlockController[,] map;
	Vector3[,,] mapVertexes;
	bool[,] lights;

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
		lights = new bool[sizeX,sizeZ];

		mapVertexes = new Vector3[sizeX * (segments + 1), segments + 1, sizeZ * (segments + 1)];
		for (int i=0; i<mapVertexes.GetLength(0); i++)
			for (int j=0; j<mapVertexes.GetLength(1); j++)
				for (int k=0; k<mapVertexes.GetLength(2); k++)
					mapVertexes [i, j, k] = new Vector3(Random.Range(-0.1f,0.1f),
					                                    Random.Range(-0.03f,0.03f),
					                                    Random.Range(-0.1f,0.1f));

		for(int i=0;i<lights.GetLength(0);i++)
			for(int j=0;j<lights.GetLength(1);j++)
				lights[i,j]=false;
					//mapVertexes [i, j, k] = new Vector3((i%9==0 )? 0.1f : 0, 0, 0);
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
		return lights[x,z];
	}

	public bool SetLight(int x, int z, bool val)
	{
		lights[x,z]=val;
	}
}


