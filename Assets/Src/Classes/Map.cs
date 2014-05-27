using System.Collections.Generic;
using UnityEngine;

public class Map
{
	BlockController[,] map;
	VertexBlock[,] mapVertexes;
	public class VertexBlock{

		public VertexBlock()
		{
			for(int i=0;i<points.GetLength(0);i++)
				for(int j=0;j<points.GetLength(1);j++)
					for(int k=0;k<points.GetLength(2);k++)
						points[i,j,k] = Random.insideUnitSphere*0.1f;

		}
		// i, j, h
		public Vector3[,,] points = new Vector3[3,3,4];
	}

	public Map(int sizeI, int sizeJ)
	{
		map = new BlockController[sizeI,sizeJ];
		mapVertexes = new VertexBlock[sizeI+1,sizeJ+1];

		for(int i=0;i<sizeI+1;i++)
		{
			for(int j=0;j<sizeJ+1;j++)
			{
				mapVertexes[i,j] = new VertexBlock();
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

	public Vector3 GetVertex(int mapI, int mapJ,int i, int j, int k)
	{
		return mapVertexes[mapI,mapJ].points[i,j,k];
	}

}


