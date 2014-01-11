﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

enum TerrainControllerMode
{
	Idle,Picked
}
public class TerrainController : BaseManagedController {

	Cell[,] map = new Cell[16,16];

	BlockController lastSelected;
	bool meshInitializedInEditor = false;

	public GameObject cellContainer;
	public GameObject cellPrefab;
	public GameObject fogOfWar;
	public GameObject pickedObject;
	TerrainControllerMode mode = TerrainControllerMode.Idle;

	FogController fogOfWarController = null;

	TerrainMeshGenerator terrGen = null;
	Plane upperPlane, lowerPlane;

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
		Cell c=map[i,j];
		if(mode==TerrainControllerMode.Idle)
		{
			if(!c.Digged)
			{
				c.DesignateDigJob(M.JobManager);
			}
			else if(!c.CellBlock.IsDiggable())
			{
				if(lastSelected!=null)
					lastSelected.OnDeselected();
				lastSelected = c.CellBlock.GetBlockController();
				if(lastSelected!=null)
					lastSelected.OnSelected();

			}
		}
		else if(mode==TerrainControllerMode.Picked)
		{
			if(c.CellBlock==null)
			{

				BlockController bc = pickedObject.GetComponent<BlockController>();

				c.CellBlock = (IBlock)bc;

				//map[i,j].Block=pickedObject.GetComponent<BlockController>();
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


	void OnCellUpdated()
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

				Cell c = new Cell(i,j,map);
				map[i,j]=c;
				c.CellUpdated+=OnCellUpdated;
				if(i>middleI-2 && j>middleJ-2 && i<middleI+2 && j<middleJ+2)
				{

				}
				else
				{
					int v = Random.Range(0,M.GameD.CellBlocks.Count);
					if(v<M.GameD.CellBlocks.Count)
						c.CellBlock=M.GameD.CellBlocks[v];
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
				map[i,j].Generate(M,terrGen,cellPrefab,cellContainer, editMode);

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


