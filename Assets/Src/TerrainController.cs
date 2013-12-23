using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour {

	Cell[,] map = new Cell[16,16];
	bool meshInitializedInEditor = false;

	public GameObject fogOfWar;
	FogController fogOfWarController = null;

	TerrainMeshGenerator terrGen = null;


	void Init()
	{

		if(terrGen==null)
		{
			terrGen = new TerrainMeshGenerator(map);
			GenerateMap();
		}
		if(fogOfWarController==null)
		{
			fogOfWarController = fogOfWar.GetComponent<FogController>();
		}
	}
	// Use this for initialization
	void Start () {
		Init();

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
		//Gizmos.DrawLine(transform.position,transform.position+new Vector3(map.GetUpperBound(0)*hw,0,0));
		//Gizmos.DrawLine(transform.position,transform.position+new Vector3(0,0,map.GetUpperBound(1)*hw));
		if(meshInitializedInEditor==false)
		{
			meshInitializedInEditor=true;
			Init();
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
		Mesh mesh = terrGen.Generate();

		if(editMode)
			GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			GetComponent<MeshFilter>().mesh = mesh;

		fogOfWarController.GenerateFog(map,editMode);
	}


}


