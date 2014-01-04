using UnityEngine;
using System.Collections;

public class TerrainMeshGenerator : MeshGenerator {

	public const float CELL_SIZE = 1;
	public static Color AMBIENT_LIGHT = new Color(0.2f,0.2f,0.2f);
	protected Cell[,] map;

	public TerrainMeshGenerator(Cell [,] targetMap)
	{
		map = targetMap;
	}
	void AttachHRect(Vector2 p1, Vector2 p2, float z)
	{
		int idx = vertices.Count;
		AddVertex(p1.x,z,p1.y);
		AddUV(0,0,0.3f,0.3f);

		AddVertex(p2.x,z,p1.y);
		AddUV(1,0,0.6f,0.3f);

		AddVertex(p1.x,z,p2.y);
		AddUV(0,1,0.3f,0.6f);

		AddVertex(p2.x,z,p2.y);
		AddUV(1,1,0.6f,0.6f);
		
		triangles.Add(idx);
		triangles.Add(idx+2);
		triangles.Add(idx+1);
		
		triangles.Add(idx+1);
		triangles.Add(idx+2);
		triangles.Add(idx+3);
	
		if(z==0)
		{
			colors.Add(new Color(0.4f,0.4f,0.4f));
			colors.Add(new Color(0.4f,0.4f,0.4f));
			colors.Add(new Color(0.4f,0.4f,0.4f));
			colors.Add(new Color(0.4f,0.4f,0.4f));
		}
		else
		{
			colors.Add(new Color(1,1,1));
			colors.Add(new Color(1,1,1));
			colors.Add(new Color(1,1,1));
			colors.Add(new Color(1,1,1));
		}

		
	}
	void AttachVRect(Vector2 p1, Vector3 p2, float z1, float z2)
	{
		int idx = vertices.Count;
		AddVertex(p1.x,z1,p1.y);
		AddUV(0,0,0.3f,0);

		AddVertex(p2.x,z1,p2.y);
		AddUV(0,1,0.6f,0);

		AddVertex(p1.x,z2,p1.y);
		AddUV(1,0,0.3f,0.3f);

		AddVertex(p2.x,z2,p2.y);
		AddUV(1,1,0.6f,0.3f);
		
		triangles.Add(idx);
		triangles.Add(idx+1);
		triangles.Add(idx+2);
		
		triangles.Add(idx+1);
		triangles.Add(idx+3);
		triangles.Add(idx+2);

		colors.Add(new Color(1,1,1));
		colors.Add(new Color(1,1,1));
		colors.Add(new Color(0.4f,0.4f,0.4f));
		colors.Add(new Color(0.4f,0.4f,0.4f));
		
	}

	public override Mesh Generate (int i, int j)
	{
		Mesh mesh = new Mesh();
		
		
		Clear();
		
		int h = map.GetUpperBound(0);
		int w = map.GetUpperBound(1);

		
	
		Cell c = map[i,j];

		float level = c.Digged?0:CELL_SIZE;
		
		
		
		AttachHRect(new Vector2(	j		*CELL_SIZE, i		*CELL_SIZE),
		            new Vector2(	(j+1)	*CELL_SIZE, (i+1)	*CELL_SIZE),level);
		
		if(!c.Digged)
		{
			if(j>0 && map[i,j-1].Digged==true)
			{
				AttachVRect(
					new Vector2(	j		*CELL_SIZE, (i+1)	*CELL_SIZE),
					new Vector2(	j		*CELL_SIZE, i		*CELL_SIZE),

					CELL_SIZE,0);
			}
			
			if(i>0 && map[i-1,j].Digged==true)
			{
				AttachVRect(
							new Vector2(	j		*CELL_SIZE, i		*CELL_SIZE),
				            new Vector2(	(j+1)	*CELL_SIZE, i	*CELL_SIZE),
							
				            CELL_SIZE,0);
			}
			
			if(j<w && map[i,j+1].Digged==true)
			{
				AttachVRect(
					new Vector2(	(j+1)	*CELL_SIZE, i		*CELL_SIZE),
					new Vector2(	(j+1)	*CELL_SIZE, (i+1)	*CELL_SIZE),

					CELL_SIZE,0);
			}
			
			if(i<h && map[i+1,j].Digged==true)
			{
				AttachVRect(
					new Vector2(	(j+1)	*CELL_SIZE, (i+1)	*CELL_SIZE),
					new Vector2(	j		*CELL_SIZE, (i+1)	*CELL_SIZE),

					CELL_SIZE,0);
			}
		}

		
	
		
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.uv2 = uvs2.ToArray();
		mesh.colors = colors.ToArray();
		mesh.RecalculateNormals();
		

		
	
		return mesh;
	}
}
