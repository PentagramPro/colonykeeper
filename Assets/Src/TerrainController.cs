using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour {

	Cell[,] map = new Cell[16,16];
	bool meshInitializedInEditor = false;
	private const float hw = 1;

	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();
	List<Vector2> uvs = new List<Vector2>();

	// Use this for initialization
	void Start () {
		GenerateMap();
		GenerateMesh(false);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool IsUpperVertex(int i, int j)
	{
		if(i==0||j==0||i==map.GetUpperBound(0)||j==map.GetUpperBound(1))
			return true;
		return !map[i-1,j-1].Digged;
	}
	bool IsLowerVertex(int i, int j)
	{
		if(i==0||j==0||i==map.GetUpperBound(0)||j==map.GetUpperBound(1))
			return false;
		return map[i-1,j-1].Digged;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawLine(transform.position,transform.position+new Vector3(map.GetUpperBound(0)*hw,0,0));
		Gizmos.DrawLine(transform.position,transform.position+new Vector3(0,0,map.GetUpperBound(1)*hw));
		if(meshInitializedInEditor==false)
		{
			meshInitializedInEditor=true;
			GenerateMap();
			GenerateMesh(true);
		}
	}

	void GenerateMap()
	{
		int h = map.GetUpperBound(0);
		int w = map.GetUpperBound(1);



		
		for(int i=0;i<=h;i++)
		{
			for(int j=0;j<=w;j++)
			{
				Cell c = new Cell();
				map[i,j]=c;
				c.Digged = (Random.Range(0,2)==0);
				//c.Digged=false;
			}
		}
		//map[1,1].Digged=true;
	}


	/*void AttachHRect(Vector2 p1, Vector3 p2, float z)
	{
		int idx = vertices.Count;
		vertices.Add(new Vector3(p1.x,z,p1.y));
		uvs.Add(new Vector2(0,0));
		vertices.Add(new Vector3((i+1)*hw,level,j*hw));
		uvs.Add(new Vector2(1,0));
		vertices.Add(new Vector3(i*hw,level,(j+1)*hw));
		uvs.Add(new Vector2(0,1));
		vertices.Add(new Vector3((i+1)*hw,level,(j+1)*hw));
		uvs.Add(new Vector2(1,1));
		
		triangles.Add(idx);
		triangles.Add(idx+2);
		triangles.Add(idx+1);
		
		triangles.Add(idx+2);
		triangles.Add(idx+3);
		triangles.Add(idx+1);

	}*/
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

	void GenerateMesh(bool editMode)
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


				vertices.Add(new Vector3(i*hw,level,j*hw));
				uvs.Add(new Vector2(0,0));
				vertices.Add(new Vector3((i+1)*hw,level,j*hw));
				uvs.Add(new Vector2(1,0));
				vertices.Add(new Vector3(i*hw,level,(j+1)*hw));
				uvs.Add(new Vector2(0,1));
				vertices.Add(new Vector3((i+1)*hw,level,(j+1)*hw));
				uvs.Add(new Vector2(1,1));

				triangles.Add(idx);
				triangles.Add(idx+2);
				triangles.Add(idx+1);

				triangles.Add(idx+2);
				triangles.Add(idx+3);
				triangles.Add(idx+1);

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


		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.RecalculateNormals();

		if(editMode)
			GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			GetComponent<MeshFilter>().mesh = mesh;

		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
	}
}
