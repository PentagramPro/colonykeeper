using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using System.IO;

enum TerrainControllerMode
{
	Idle,Picked
}
public class TerrainController : BaseManagedController, IStorable {

	BlockController[,] map;

	BlockController lastSelected;
	bool meshInitializedInEditor = false;

	public GameObject cellContainer;
	public GameObject cellPrefab;
	public GameObject fogOfWar;
	public GameObject pickedObject;

	[HideInInspector]
	public int MapX=10,MapY=10;

	TerrainControllerMode mode = TerrainControllerMode.Idle;



	TerrainMeshGenerator terrGen = null;
	Plane lowerPlane;

	List<BlockController> updateList = new List<BlockController>();

	public void Init()
	{
		if(M==null)
		{
			PrepareManager();
		}
		M.LoadResources();
		if(terrGen==null)
		{
			terrGen = new TerrainMeshGenerator();
		}
	


	}
	// Use this for initialization
	void Start () {
		Init();

		M.GetGUIController().ItemPicked+=OnItemPicked;

		PrepareTerrain(MapX,MapY,false);

		lowerPlane = new Plane(Vector3.up, transform.position);


	}

	public void PrepareTerrain(int mapX, int mapY, bool editMode)
	{
		MapX = mapX;
		MapY = mapY;
		map = new BlockController[MapX,MapY];


		GenerateMap(editMode);
		GenerateMesh(editMode);
	}

	void OnItemPicked(Building selected, RecipeInstance recipe)
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
	
		if (updateList.Count > 0)
		{
			foreach(BlockController b in updateList)
				b.Generate(map,terrGen,false);
			updateList.Clear();

			GraphUpdateObject guo = new GraphUpdateObject(collider.bounds);
			AstarPath.active.UpdateGraphs(guo);
		}

	}

	void OnMouseOver()
	{
		if(Input.GetMouseButtonUp(1) && mode == TerrainControllerMode.Picked)
		{
			M.GetGUIController().OnPlaced();
			mode=TerrainControllerMode.Idle;
			Destroy(pickedObject);
			pickedObject=null;
		}
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
		if(mode==TerrainControllerMode.Picked)
		{
			if(c.CanBuild())
			{
				if(!c.Build(M,pickedObject,M.GetGUIController().recipeInstance))
				{
					GameObject.DestroyObject(pickedObject);
					throw new UnityException("Some error while trying to build construction site");
				}
				pickedObject=null;
				mode=TerrainControllerMode.Idle;
				M.GetGUIController().OnPlaced();
			}
		

		}
		else if(mode==TerrainControllerMode.Idle)
		{
			M.GetGUIController().OnDeselect();
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
	
	}


	void OnCellUpdated(int i, int j)
	{
		updateList.Add(map [i, j]);

	}



	void GenerateMap(bool editMode)
	{

		int h = map.GetLength(0);
		int w = map.GetLength(1);
		GameObject mainBuilding = null;
		Debug.Log("Generate Map");

		Transform[] children = cellContainer.GetComponentsInChildren<Transform>();
		foreach(Transform child in children)
		{
			if(child!=cellContainer.transform)
			{
				try{
					Object.DestroyImmediate(child.gameObject);
				}
				catch(UnityException e){}
			}
		}



		M.cameraController.bounds = new Rect(0,0,w*TerrainMeshGenerator.CELL_SIZE,h*TerrainMeshGenerator.CELL_SIZE);

		Block[,] pattern = MapGenerator.GenerateBlocksPattern(M, h, w);

		for(int i=0;i<h;i++)
		{
			for(int j=0;j<w;j++)
			{
				GameObject cellObj = (GameObject)Instantiate(cellPrefab);
				map[i,j] = cellObj.GetComponent<BlockController>();
				BlockController c = map[i,j];
				c.transform.parent = cellContainer.transform;
				c.InitCell(i,j,map);


				c.CellUpdated+=OnCellUpdated;
				c.CellMouseOver+=OnCellHover;
				c.CellMouseUp+=OnCellClicked;
				c.BlockProt = pattern[i,j];

				if(!editMode && i==h/2 && j==w/2)
				{

					mainBuilding = M.GameD.BuildingsByName["Headquarters"].Instantiate();

					c.BuildOn(mainBuilding.GetComponent<BuildingController>());
					StorageController st = mainBuilding.GetComponent<StorageController>();
					foreach(PileXML item in M.GameD.StartItemsList)
					{
						st.Put(M.GameD.Items[item.Name],item.Quantity);
					}
				}
				if(!editMode  && i==h/2 && (j==w/2-1 || j==w/2+1))
				{
					GameObject veh = M.GameD.VehiclesByName["Drone"].Instantiate();
					M.VehiclesRegistry.Add(veh.GetComponent<VehicleController>());
					veh.transform.position = c.Position;
				}
				if(!editMode  && i==h/2+1 && j==w/2)
				{
					GameObject veh = M.GameD.VehiclesByName["Defender Drone"].Instantiate();
					M.VehiclesRegistry.Add(veh.GetComponent<VehicleController>());
					veh.transform.position = c.Position;
				}

				//c.Digged=false;
			}
		}

		int graphW = map.GetLength(1)*5,graphH = map.GetLength(0)*5;
		Vector3 pos = transform.position+new Vector3((float)map.GetLength(1)/2f,0.2f,(float)map.GetLength(0)/2f);

		((BoxCollider)collider).size = new Vector3(w,0.2f,h);
		((BoxCollider)collider).center = new Vector3(pos.x,-0.1f,pos.z);
		if(!editMode)	
		{
			AstarPath.active.astarData.gridGraph.width=graphW;
				
			AstarPath.active.astarData.gridGraph.depth=graphH;
				

			AstarPath.active.astarData.gridGraph.center= pos;
			
			AstarPath.active.astarData.gridGraph.UpdateSizeFromWidthDepth ();


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
				map[i,j].Generate(map,terrGen, editMode);

			}
		}





		if(!editMode)
		{
			Debug.Log("Updating graph");
			AstarPath.active.Scan();
			//GraphUpdateObject guo = new GraphUpdateObject(collider.bounds);
			//AstarPath.active.UpdateGraphs(guo);
		}
	}


	#region IStorable implementation

	public override void SaveUid (WriterEx b)
	{
		base.SaveUid (b);
		for(int i=0;i<map.GetLength(0);i++)
		{
			for(int j=0;j<map.GetLength(1);j++)
			{
				map[i,j].SaveUid(b);
			}
		}
	}

	public override void LoadUid (Manager m, ReaderEx r)
	{
		base.LoadUid (m, r);
		for(int i=0;i<map.GetLength(0);i++)
		{
			for(int j=0;j<map.GetLength(1);j++)
			{
				
				map[i,j].LoadUid(m,r);
			}
		}
	}
	public void Save (WriterEx b)
	{
		for(int i=0;i<map.GetLength(0);i++)
		{
			for(int j=0;j<map.GetLength(1);j++)
			{
				map[i,j].Save(b);
			}
		}
	}
	public void Load (Manager m, ReaderEx r)
	{
		for(int i=0;i<map.GetLength(0);i++)
		{
			for(int j=0;j<map.GetLength(1);j++)
			{

				map[i,j].Load(m,r);
			}
		}
		GenerateMesh(false);
	}
	#endregion
}


  