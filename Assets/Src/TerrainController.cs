using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour {

	Cell[,] map = new Cell[8,8];
	bool meshInitializedInEditor = false;
	private const float hw = 1;
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

	void GenerateMesh(bool editMode)
	{

		Mesh mesh = new Mesh();


		List<Vector3> vertices = new List<Vector3>();

		List<int> triangles = new List<int>();

		int h = map.GetUpperBound(0);
		int w = map.GetUpperBound(1);
		int index=0;

		for(int i=0;i<=h;i++)
		{
			for(int j=0;j<=w;j++)
			{
				Cell c = map[i,j];
				c.lt=-1;
				c.lb=-1;
				c.rt=-1;
				c.rb=-1;
				float level = c.Digged?hw:0;
				//Debug.Log("lt="+c.lt+" rt="+c.rt);
				if(j>0 && c.Digged==map[i,j-1].Digged)
				{
					c.lt=map[i,j-1].rt;
					c.lb=map[i,j-1].rb;
				}

				if(i>0 && c.Digged==map[i-1,j].Digged)
				{
					c.lt=map[i-1,j].lb;
					c.rt=map[i-1,j].rb;
				}

				if(c.lt==-1)
				{
					c.lt=index;
					vertices.Add(new Vector3(j*hw,level,i*hw));
					index++;
				}
				if(c.rt==-1)
				{
					c.rt=index;
					vertices.Add(new Vector3((j+1)*hw,level,i*hw));
					index++;
				}
				if(c.lb==-1)
				{
					c.lb=index;
					vertices.Add(new Vector3(j*hw,level,(i+1)*hw));
					index++;
				}
				if(c.rb==-1)
				{
					c.rb=index;
					vertices.Add(new Vector3((j+1)*hw,level,(i+1)*hw));
					index++;
				}

				triangles.Add(c.lt);
				triangles.Add(c.lb);
				triangles.Add(c.rt);

				triangles.Add(c.rt);
				triangles.Add(c.lb);
				triangles.Add(c.rb);

				if(j>0 && c.Digged!=map[i,j-1].Digged)
				{
					triangles.Add(map[i,j-1].rt);
					triangles.Add(map[i,j-1].rb);
					triangles.Add(c.lb);


					triangles.Add(map[i,j-1].rt);
					triangles.Add(c.lb);
					triangles.Add(c.lt);
				}

				if(i>0 && c.Digged!=map[i-1,j].Digged)
				{
					triangles.Add(map[i-1,j].rb);
					triangles.Add(map[i-1,j].lb);
					triangles.Add(c.lt);
					
					
					triangles.Add(map[i-1,j].rb);
					triangles.Add(c.lt);
					triangles.Add(c.rt);
				}
			}
		}
		//Debug.Log("index = "+index);


		/*int h = map.GetUpperBound(0)+1;
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
		}*/

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.RecalculateNormals();

		if(editMode)
			GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			GetComponent<MeshFilter>().mesh = mesh;
	}
}
