using UnityEngine;
using System.Collections;

public class TerrainMeshGenerator : MeshGenerator {

	private const float hw = 1;
	Cell[,] map;

	public TerrainMeshGenerator(Cell [,] targetMap)
	{
		map = targetMap;
	}
	void AttachHRect(Vector2 p1, Vector2 p2, float z)
	{
		int idx = vertices.Count;
		vertices.Add(new Vector3(p1.x,z,p1.y));
		uvs.Add(new Vector2(0,0));
		vertices.Add(new Vector3(p2.x,z,p1.y));
		uvs.Add(new Vector2(1,0));
		vertices.Add(new Vector3(p1.x,z,p2.y));
		uvs.Add(new Vector2(0,1));
		vertices.Add(new Vector3(p2.x,z,p2.y));
		uvs.Add(new Vector2(1,1));
		
		triangles.Add(idx);
		triangles.Add(idx+2);
		triangles.Add(idx+1);
		
		triangles.Add(idx+1);
		triangles.Add(idx+2);
		triangles.Add(idx+3);
		
	}
	void AttachVRect(Vector2 p1, Vector3 p2, float z1, float z2)
	{
		int idx = vertices.Count;
		vertices.Add(new Vector3(p1.x,z1,p1.y));
		uvs.Add(new Vector2(0,0));
		vertices.Add(new Vector3(p2.x,z1,p2.y));
		uvs.Add(new Vector2(0,1));
		vertices.Add(new Vector3(p1.x,z2,p1.y));
		uvs.Add(new Vector2(1,0));
		vertices.Add(new Vector3(p2.x,z2,p2.y));
		uvs.Add(new Vector2(1,1));
		
		triangles.Add(idx);
		triangles.Add(idx+1);
		triangles.Add(idx+2);
		
		triangles.Add(idx+1);
		triangles.Add(idx+3);
		triangles.Add(idx+2);
		
		
	}

	public override Mesh Generate ()
	{
		Mesh mesh = new Mesh();
		
		
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
		
		int h = map.GetUpperBound(0);
		int w = map.GetUpperBound(1);
		int index=0;
		
		for(int i=0;i<=h;i++)
		{
			for(int j=0;j<=w;j++)
			{
				Cell c = map[i,j];
				int idx = vertices.Count;
				float level = c.Digged?0:hw;
				
				
				
				AttachHRect(new Vector2(	i		*hw, j		*hw),
				            new Vector2(	(i+1)	*hw, (j+1)	*hw),level);
				
				if(c.Digged)
				{
					if(j>0 && map[i,j-1].Digged==false)
					{
						AttachVRect(
							new Vector2(	(i+1)	*hw, j		*hw),
							new Vector2(	i		*hw, j		*hw),
							hw,0);
					}
					
					if(i>0 && map[i-1,j].Digged==false)
					{
						AttachVRect(new Vector2(	i		*hw, j		*hw),
						            new Vector2(	i		*hw, (j+1)	*hw),
						            hw,0);
					}
					
					if(j<w && map[i,j+1].Digged==false)
					{
						AttachVRect(
							new Vector2(	i		*hw, (j+1)	*hw),
							new Vector2(	(i+1)	*hw, (j+1)	*hw),
							hw,0);
					}
					
					if(i<h && map[i+1,j].Digged==false)
					{
						AttachVRect(
							new Vector2(	(i+1)	*hw, (j+1)	*hw),
							new Vector2(	(i+1)	*hw, j		*hw),
							hw,0);
					}
				}
			}
		}
		
		/*for(int i=0;i<=h+1;i++)
		{
			for(int j=0;j<=w+1;j++)
			{
				int idx = vertices.Count;
				vertices.Add(new Vector3(j*hw,hw,i*hw));
				uvs.Add(new Vector2(0,0));
				if(i>0 && j>0 && i<=h && j<=w)
				{
					triangles.Add(idx-2-w);
					triangles.Add(idx-w-1);
					triangles.Add(idx);
					
					//triangles.Add(idx-1-w);
					//triangles.Add(idx);
					//triangles.Add(idx-1);
				}
			}
		}*/
		
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();
		

		
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();

		return mesh;
	}
}
