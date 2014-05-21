using System.Collections.Generic;
using UnityEngine;

public class Map
{
	BlockController[,] map;
	VertexBlock[,] mapVertexes;
	public class VertexBlock{

		// i, j, h
		public Vector3[,,] points = new Vector3[3,3,4];
	}

	public Map(int sizeI, int sizeJ)
	{
		map = new BlockController[sizeI,sizeJ];
		mapVertexes = new VertexBlock[sizeI+1,sizeJ+1];

		for(int i=0;i<sizeI;i++)
		{
			for(int j=0;j<sizeJ;j++)
			{
				// init
			}
		}
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


}


