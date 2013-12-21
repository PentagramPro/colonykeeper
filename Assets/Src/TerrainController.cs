using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour {

	Cell[,] map = new Cell[50,50];
	bool meshInitializedInEditor = false;
	private const float hw = 1;
	// Use this for initialization
	void Start () {
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
			GenerateMesh(true);
		}
	}

	void GenerateMesh(bool editMode)
	{

		Mesh mesh;
		if(editMode)
			mesh = GetComponent<MeshFilter>().sharedMesh;
		else 
			mesh = GetComponent<MeshFilter>().mesh;

		List<Vector3> vertices = new List<Vector3>();

		List<int> triangles = new List<int>();

		int h = map.GetUpperBound(0)+1;
		int w = map.GetUpperBound(1)+1;
		for(int i=0;i<h;i++)
		{
			for(int j=0;j<w;j++)
			{

				vertices.Add(new Vector3(j*hw,0,i*hw));
				if(j>0 && i>0)
				{
					triangles.Add(i*w+j);
					triangles.Add((i-1)*w+j);
					triangles.Add(i*w+j-1);


					triangles.Add((i-1)*w+j-1);
					triangles.Add(i*w+j-1);
					triangles.Add((i-1)*w+j);

				}

			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();
	}
}
