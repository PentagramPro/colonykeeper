using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

enum TerrainControllerMode
{
	Idle,Picked
}
public class TerrainController : BaseManagedController {

	BlockController[,] map = new BlockController[16,16];

	BlockController lastSelected;
	bool meshInitializedInEditor = false;

	public GameObject cellContainer;
	public GameObject cellPrefab;
	public GameObject fogOfWar;
	public GameObject pickedObject;
	TerrainControllerMode mode = TerrainControllerMode.Idle;

	FogController fogOfWarController = null;

	TerrainMeshGenerator terrGen = null;
	Plane lowerPlane;

	void Init(bool editMode)
	{
		if(M==null)
		{
			PrepareManager();
		}
		M.LoadResources();
		if(terrGen==null)
		{
			terrGen = new TerrainMeshGenerator(map);
			GenerateMap(editMode);
		}
		if(fogOfWarController==null)
		{
			fogOfWarController = fogOfWar.GetComponent<FogController>();
		}


	}
	// Use this for initialization
	void Start () {
		Init(false);

		M.GetGUIController().ItemPicked+=OnItemPicked;
		GenerateMesh(false);

		lowerPlane = new Plane(Vector3.up, transform.position);

		Camera.main.transform.position=new Vector3(
			transform.position.x+map.GetLength(1)*TerrainMeshGenerator.CELL_SIZE/2,
			Camera.main.transform.position.y,
			transform.position.y+map.GetLength(0)*TerrainMeshGenerator.CELL_SIZE/2);
//		upperPlane = new Plane(Vector3.up, transform.position+new Vector3(0,TerrainMeshGenerator.CELL_SIZE,0));
	}

	void OnItemPicked(Building selected)
	{
		mode = TerrainControllerMode.Picked;
		pickedObject = selected.Instantiate();
		pickedObject.transform.parent = transform;
		pickedObject.transform.position = new Vector3(0,0,0);
	}

	bool DetectCellUnderMouse(out int iRes, out int jRes)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		iRes=jRes=-1;
		

		if (lowerPlane.Raycast(ray,out distance))
		{
			
			Vector3 hitPoint = ray.GetPoint(distance)-transform.position;
			
			int i = (int)(hitPoint.z/TerrainMeshGenerator.CELL_SIZE);
			int j = (int)(hitPoint.x/TerrainMeshGenerator.CELL_SIZE);
			if(i>=0 && j>=0 && i<map.GetLength(0) && j<map.GetLength(1))
			{
				iRes=i;jRes=j;
				return true;
			}
		}
				
		return false;
	}


	// Update is called once per frame
	void Update () {
	
		//int i,j;
		//if(DetectCellUnderMouse(out i, out j))
		//   OnCellHover(i,j);


	}

	void OnMouseOver()
	{
		int i,j;
		if(DetectCellUnderMouse(out i, out j))
			OnCellHover(i,j);
	}

	void OnMouseUp()
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
		BlockController c=map[i,j];
		if(mode==TerrainControllerMode.Idle)
		{
			if(!c.Digged)
			{
				c.DesignateDigJob();
			}

		}
		else if(mode==TerrainControllerMode.Picked)
		{
			if(c.CanBuild())
			{
				c.Build(M,pickedObject);
				pickedObject=null;
				mode=TerrainControllerMode.Idle;
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
			Init(true);
			GenerateMesh(true);
		}
	}


	void OnCellUpdated(int i, int j)
	{
		GenerateMesh(false);
	}

	void GenerateMap(bool editMode)
	{
		int h = map.GetUpperBound(0);
		int w = map.GetUpperBound(1);

		Debug.Log("Generate Map");

		foreach(Transform children in cellContainer.transform)
		{

			if(!editMode)
				Destroy(children.gameObject);
			else
				DestroyImmediate(children.gameObject);

		}

		int middleI=h/2;
		int middleJ=w/2;
		
		for(int i=0;i<=h;i++)
		{
			for(int j=0;j<=w;j++)
			{
				GameObject cellObj = (GameObject)Instantiate(cellPrefab);
				map[i,j] = cellObj.GetComponent<BlockController>();
				BlockController c = map[i,j];
				c.transform.parent = cellContainer.transform;
				c.InitCell(i,j,map);


				c.CellUpdated+=OnCellUpdated;
				c.CellMouseOver+=OnCellHover;
				c.CellMouseUp+=OnCellClicked;

				if(i>middleI-2 && j>middleJ-2 && i<middleI+2 && j<middleJ+2)
				{

				}
				else if(i==0 || j==0 || i==h || j==w)
				{
					c.BlockProt=M.GameD.Blocks[0];
				}
				else
				{
					int v = Random.Range(1,M.GameD.Blocks.Count);
					if(v<M.GameD.Blocks.Count)
						c.BlockProt=M.GameD.Blocks[v];
				}
				//c.Digged=false;
			}
		}
		//map[1,1].Digged=true;
	}

	void OnDestroy () 
	{
		Debug.Log("OnDestroy");
		foreach(Transform children in cellContainer.transform)
		{
			

			DestroyImmediate(children.gameObject);
			
		}
	}


	void GenerateMesh(bool editMode)
	{

		Debug.Log("Generate Mesh");

		for(int i=0;i<map.GetLength(0);i++)
		{
			for(int j=0;j<map.GetLength(1);j++)
			{
				map[i,j].Generate(M,terrGen, editMode);

			}
		}



		fogOfWarController.GenerateFog(map,editMode);

		if(!editMode)
		{
			Debug.Log("Updating graph");
			GraphUpdateObject guo = new GraphUpdateObject(collider.bounds);
			AstarPath.active.UpdateGraphs(guo);
		}
	}


}


