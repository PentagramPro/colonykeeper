using UnityEngine;
using System.Collections;

public class FogOfWarMeshGenerator : TerrainMeshGenerator {



	public FogOfWarMeshGenerator(Cell[,] m) : base(m)
	{

	}

	bool isVisibleVertex(int i, int j)
	{
		if(i==0 || j==0 || i==map.GetLength(0) || j==map.GetLength(1))
			return false;

		return 	map[i,j].Digged || 
		   		map[i-1,j].Digged || 
		   		map[i,j-1].Digged || 
		   		map[i-1,j-1].Digged;
	}

	public override Mesh Generate ()
	{
		Mesh mesh = new Mesh();
		
		
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
		colors.Clear();

		int h = map.GetLength(0);
		int w = map.GetLength(1);

		for(int i=0;i<h+1;i++)
		{
			for(int j=0;j<w+1;j++)
			{
				int idx = vertices.Count;
				vertices.Add(new Vector3(j*CELL_SIZE,CELL_SIZE,i*CELL_SIZE));
				uvs.Add(new Vector2(0,0));
				if(i>0)
				{
					if(j<w)
					{
						triangles.Add(idx);

						triangles.Add(idx-w);
						triangles.Add(idx-w-1);
					}

					if(j>0)
					{
						triangles.Add(idx);

						triangles.Add(idx-w-1);
						triangles.Add(idx-1);
					}
				}

				colors.Add(new Color(0,0,0,
				                     isVisibleVertex(i,j)?0:1));

			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.colors = colors.ToArray();

		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
		colors.Clear();

		return mesh;
	}
}
