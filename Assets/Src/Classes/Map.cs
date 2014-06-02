using System.Collections.Generic;
using UnityEngine;

public class Map
{
	BlockController[,] map;
	Vector3[,,] mapVertexes;
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

	public Map(int sizeI, int sizeJ, int segments)
	{
		this.segments = segments;
		map = new BlockController[sizeI,sizeJ];

		mapVertexes = new Vector3[sizeJ * (segments + 1), segments + 1, sizeI * (segments + 1)];
		for (int i=0; i<mapVertexes.GetLength(0); i++)
			for (int j=0; j<mapVertexes.GetLength(1); j++)
				for (int k=0; k<mapVertexes.GetLength(2); k++)
					//mapVertexes [i, j, k] = Random.insideUnitSphere * 0.1f;
					mapVertexes [i, j, k] = new Vector3((i%5==0 && j%5==0)? 0.1f : 0, 0, 0);
	}


	public BlockController this [int i, int j] 
	{
		get{
			return map[i,j];
		}

		set{
			map[i,j]=value;
		}
	}

	public int Width{
		get{return map.GetLength(1);}
	}

	public int Height{
		get{return map.GetLength(0);}
	}

	public Vector3 GetVertex(IntVector3 pos)
	{
		return mapVertexes[pos.X,pos.Y,pos.Z];
	}

}


