using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainController : MonoBehaviour {

	Cell[,] map = new Cell[16,16];
	bool meshInitializedInEditor = false;
	bool lmbPressed=false;

	public GameObject fogOfWar;
	FogController fogOfWarController = null;

	TerrainMeshGenerator terrGen = null;
	Plane upperPlane, lowerPlane;

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

		lowerPlane = new Plane(Vector3.up, transform.position);
		upperPlane = new Plane(Vector3.up, transform.position+new Vector3(0,TerrainMeshGenerator.CELL_SIZE,0));
	}
	
	// Update is called once per frame
	void Update () {
	
		if(Input.GetMouseButtonDown(0))
		{
			if(lmbPressed==false)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				// plane.Raycast returns the distance from the ray start to the hit point
				float distance;
				
				if (upperPlane.Raycast(ray,out distance))
				{
					Debug.Log("intersects");
					Vector3 hitPoint = ray.GetPoint(distance)-transform.position;

					int i = (int)(hitPoint.z/TerrainMeshGenerator.CELL_SIZE);
					int j = (int)(hitPoint.x/TerrainMeshGenerator.CELL_SIZE);

					if(i>=0 && j>=0 && i<map.GetLength(0) && j<map.GetLength(1))
					{
						if(!map[i,j].Digged)
							OnCellClicked(i,j);
						else
						{
							if (lowerPlane.Raycast(ray,out distance))
							{

								hitPoint = ray.GetPoint(distance)-transform.position;
								
								i = (int)(hitPoint.z/TerrainMeshGenerator.CELL_SIZE);
								j = (int)(hitPoint.x/TerrainMeshGenerator.CELL_SIZE);
								if(i>=0 && j>=0 && i<map.GetLength(0) && j<map.GetLength(1))
									OnCellClicked(i,j);
							}
						}
					}
						
				}
			}

			lmbPressed=true;
		}
		else
		{
			lmbPressed=false;
		}
	}

	void OnCellClicked(int i, int j)
	{
		if(!map[i,j].Digged)
		{
			map[i,j].Digged=true;
			GenerateMesh(false);
		}
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
				if(i<2 || j<2 || i>h-2 || j>w-2)
					c.Digged=false;
				else
					c.Digged = (Random.Range(0,3)==0);
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

		if(!editMode)
		{

		}
	}


}


