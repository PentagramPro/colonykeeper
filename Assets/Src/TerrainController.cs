using UnityEngine;
using System.Collections;
using System.Collections.Generic;

enum TerrainControllerMode
{
	Idle,Picked
}
public class TerrainController : BaseManagedController {

	Cell[,] map = new Cell[16,16];
	bool meshInitializedInEditor = false;


	public GameObject fogOfWar;
	public GameObject pickedObject;
	TerrainControllerMode mode = TerrainControllerMode.Idle;

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

		M.GetGUIController().ItemPicked+=OnItemPicked;
		GenerateMesh(false);

		lowerPlane = new Plane(Vector3.up, transform.position);
		upperPlane = new Plane(Vector3.up, transform.position+new Vector3(0,TerrainMeshGenerator.CELL_SIZE,0));
	}

	void OnItemPicked(GameObject prefab)
	{
		mode = TerrainControllerMode.Picked;
		pickedObject = (GameObject)Instantiate(prefab);
		pickedObject.transform.parent = transform;
		pickedObject.transform.position = new Vector3(0,0,0);
	}

	bool DetectCellUnderMouse(out int iRes, out int jRes)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		iRes=jRes=-1;
		
		if (upperPlane.Raycast(ray,out distance))
		{
			//Debug.Log("intersects");
			Vector3 hitPoint = ray.GetPoint(distance)-transform.position;
			
			int i = (int)(hitPoint.z/TerrainMeshGenerator.CELL_SIZE);
			int j = (int)(hitPoint.x/TerrainMeshGenerator.CELL_SIZE);
			
			if(i>=0 && j>=0 && i<map.GetLength(0) && j<map.GetLength(1))
			{
				if(!map[i,j].Digged)
				{
					iRes=i;jRes=j;
					return true;
				}
				else
				{
					if (lowerPlane.Raycast(ray,out distance))
					{
						
						hitPoint = ray.GetPoint(distance)-transform.position;
						
						i = (int)(hitPoint.z/TerrainMeshGenerator.CELL_SIZE);
						j = (int)(hitPoint.x/TerrainMeshGenerator.CELL_SIZE);
						if(i>=0 && j>=0 && i<map.GetLength(0) && j<map.GetLength(1))
						{
							iRes=i;jRes=j;
							return true;
						}
					}
				}
			}
		}
		return false;
	}
	// Update is called once per frame
	void Update () {
	
		int i,j;
		if(DetectCellUnderMouse(out i, out j))
		   OnCellHover(i,j);


	}

	void OnMouseDown()
	{
		int i,j;
		if(DetectCellUnderMouse(out i, out j))
		   OnCellClicked(i,j);
			
	}

	void OnCellHover(int i, int j)
	{
		if(mode==TerrainControllerMode.Picked)
		{
			float cx = (j+0.5f)*TerrainMeshGenerator.CELL_SIZE;
			float cy = (i+0.5f)*TerrainMeshGenerator.CELL_SIZE;

			pickedObject.transform.localPosition = new Vector3(cx,0,cy);
		}
	}
	void OnCellClicked(int i, int j)
	{
		if(mode==TerrainControllerMode.Idle)
		{
			if(!map[i,j].Digged)
			{
				map[i,j].Digged=true;
				GenerateMesh(false);
			}
		}
		else if(mode==TerrainControllerMode.Picked)
		{
			if(map[i,j].Digged)
			{
				if(map[i,j].Block==null)
				{
					map[i,j].Block=pickedObject.GetComponent<BlockController>();
					pickedObject=null;
					mode=TerrainControllerMode.Idle;
				}
			}
			else
			{
				mode=TerrainControllerMode.Idle;
				Destroy(pickedObject);
			}
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


