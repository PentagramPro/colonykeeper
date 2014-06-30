using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using System.IO;


enum TerrainControllerMode
{
	Idle,Picked
}
public class TerrainController : BaseManagedController, IStorable {

	[SerializeField]
	Map map;


	BlockController lastSelected;


	public GameObject cellContainer;
	public GameObject cellPrefab;

	public GameObject pickedObject;

	[HideInInspector]
	public int MapX=10,MapZ=10;

	[System.NonSerialized]
	public Vector3 markerPosition;

	TerrainControllerMode mode = TerrainControllerMode.Idle;

	MapGen mapGen;

	TerrainMeshGenerator terrGen = null;
	public TerrainMeshGenerator TerrGen
	{
		get
		{
			return terrGen;
		}
	}
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
			terrGen = new TerrainMeshGenerator(M);
		}
	


	}

	public Map Map{
		get{
			return map;
		}
	}

	public Vector3 CenterOfMap
	{
		get
		{
			return transform.position+new Vector3((float)map.Width/2f,0,(float)map.Height/2f);
		}
	}



	// Use this for initialization
	void Start () {


		M.GetGUIController().ItemPicked+=OnItemPicked;

		//PrepareTerrain(MapX,MapZ,false);

		lowerPlane = new Plane(Vector3.up, transform.position);

		InitAstar();

	}
	override protected void Awake ()
	{
		base.Awake ();
		if(map!=null)
			map.Restore();
		Init();
		BlockController[] blocks = cellContainer.GetComponentsInChildren<BlockController>();
		foreach(BlockController b in blocks)
		{
			map[b.MapPos] = b;
		}
	}
	public void PrepareMapForEditor()
	{
		if(map==null)
			map = new Map(MapX,MapZ,3);
		else
			map.Restore();
		Init();
		BlockController[] blocks = cellContainer.GetComponentsInChildren<BlockController>();
		foreach(BlockController b in blocks)
		{
			map[b.MapPos] = b;
			b.PrepareManager();
		}
		M.settings.FogOfWar=false;
		foreach(BlockController b in blocks)
		{
			b.BuildLightCache();
			b.Generate(terrGen,true,false);
		}
	}

	public void PrepareMapAfterEditor()
	{
		M.settings.FogOfWar=true;
		BlockController[] blocks = cellContainer.GetComponentsInChildren<BlockController>();
		foreach(BlockController b in blocks)
		{
			b.Generate(terrGen,true,false);
		}
	}

	public void CreateRandomMap(int mapX, int mapZ)
	{

	
		MapX = mapX;
		MapZ = mapZ;

		map = new Map(MapX,MapZ,3);

		GenerateMap(false);
		GenerateMesh(true);


	}

	public GameDictionary GameD
	{
		get
		{
			return M.GameD;
		}
	}
	void OnItemPicked(Building selected, RecipeInstance recipe)
	{
		mode = TerrainControllerMode.Picked;
		pickedObject = selected.Instantiate();
		pickedObject.transform.parent = transform;
		pickedObject.transform.position = new Vector3(0,0,0);
	}

	bool DetectCellUnderMouse(out int xRes, out int zRes)
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		float distance;
		xRes=zRes=-1;
		

		if (lowerPlane.Raycast(ray,out distance))
		{
			
			Vector3 hitPoint = ray.GetPoint(distance)-transform.position;
			
			int x = (int)(hitPoint.x/TerrainMeshGenerator.CELL_SIZE);
			int z = (int)(hitPoint.z/TerrainMeshGenerator.CELL_SIZE);
			if(x>=0 && z>=0 && z<map.Height && x<map.Width)
			{
				xRes=x;zRes=z;
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
				b.BuildLightCache();
			foreach(BlockController b in updateList)
				b.Generate(terrGen,false,true);
			updateList.Clear();

			//GraphUpdateObject guo = new GraphUpdateObject(collider.bounds);
			//AstarPath.active.UpdateGraphs(guo);
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
		int x,z;
		if(DetectCellUnderMouse(out x, out z))
			OnCellHover(x,z);
	}

	void OnMouseUp()
	{

		int x,z;
		if(DetectCellUnderMouse(out x, out z))
			OnCellClicked(x,z);

	}

	public void OnCellHover(int x, int z)
	{
		if(mode==TerrainControllerMode.Picked)
		{
			float cx = (x+0.5f)*TerrainMeshGenerator.CELL_SIZE;
			float cz = (z+0.5f)*TerrainMeshGenerator.CELL_SIZE;

			pickedObject.transform.localPosition = new Vector3(cx,0,cz);
		}
	}
	public void OnCellClicked(int x, int z)
	{
		BlockController c=map[x,z];
		if(mode==TerrainControllerMode.Picked)
		{
			if(c.CanBuild())
			{
				if(!c.Build(M,pickedObject,M.GetGUIController().LastRecipeInstance))
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

	public void OnCellUpdated(int x, int z)
	{
		updateList.Add(map [x, z]);
		
	}

	bool IsUpperVertex(int i, int j)
	{
		if(i==0||j==0||i==map.Height-1||j==map.Width-1)
			return true;
		return !map[i-1,j-1].Digged;
	}
	bool IsLowerVertex(int i, int j)
	{
		if(i==0||j==0||i==map.Height-1||j==map.Width-1)
			return false;
		return map[i-1,j-1].Digged;
	}

	void OnDrawGizmosSelected()
	{
		if(mapGen!=null)
		{
			mapGen.DrawDebugGizmos();
		}

		Gizmos.color = Color.red;    
		Gizmos.DrawWireCube(markerPosition, new Vector3(1,1, 1) * 1.1f);
	}

	public void ForceDiscover(int posI, int posJ, List<BlockController> discovered)
	{
		if(discovered==null)
			discovered = new List<BlockController>();

		map[posI,posJ].Discovered = true;
		discovered.Add(map[posI,posJ]);
		OnCellUpdated(posI,posJ);

		int imin = System.Math.Max (posI-1,0);
		int imax = System.Math.Min(posI+1,map.Height-1);
		
		int jmin = System.Math.Max (posJ-1,0);
		int jmax = System.Math.Min(posJ+1,map.Width-1);
		for(int i = imin;i<=imax;i++)
		{
			for (int j = jmin;j<=jmax;j++)
			{
				if(discovered.Contains(map[i,j]))
					continue;
				
				if(map[i,j].Digged==true)
				{
					ForceDiscover(i,j,discovered);
				}
				else
				{
					map[i,j].Discovered = true;
					OnCellUpdated(i,j);
				}
			}
		}
	}
	public void Discover(int posI, int posJ)
	{

		
		map[posI,posJ].Discovered = true;
		OnCellUpdated(posI,posJ);
		
		int imin = System.Math.Max (posI-1,0);
		int imax = System.Math.Min(posI+1,map.Height-1);
		
		int jmin = System.Math.Max (posJ-1,0);
		int jmax = System.Math.Min(posJ+1,map.Width-1);
		for(int i = imin;i<=imax;i++)
		{
			for (int j = jmin;j<=jmax;j++)
			{
				if(map[i,j].Discovered)
					continue;
				
				if(map[i,j].Digged==true)
				{
					Discover(i,j);
				}
				else
				{
					map[i,j].Discovered = true;
					OnCellUpdated(i,j);
				}
			}
		}
	}






	void GenerateMap(bool editMode)
	{

		int h = map.Height;
		int w = map.Width;

		Debug.Log("Generate Map");

		Transform[] children = cellContainer.GetComponentsInChildren<Transform>();
		foreach(Transform child in children)
		{
			if(child!=cellContainer.transform)
			{
				try{

					Object.DestroyImmediate(child.gameObject);
				}
				catch(System.Exception){}
			}
		}



		M.cameraController.bounds = new Rect(0,0,w*TerrainMeshGenerator.CELL_SIZE,h*TerrainMeshGenerator.CELL_SIZE);
		mapGen = new MapGen(M);

		mapGen.AddSpot(new PlayerSpot(M,w/2,h/2));
		mapGen.AddSpot(new SentrySpot(M,w/2+3,h/2));
		mapGen.AddSpot(new SentrySpot(M,-1,-1));
		mapGen.AddSpot(new SentrySpot(M,-1,-1));
		mapGen.AddSpot(new SentrySpot(M,-1,-1));
		mapGen.AddSpot(new SentrySpot(M,-1,-1));

		mapGen.AddSpot(new TowerSpot(M,-1,-1));
		mapGen.AddSpot(new TowerSpot(M,-1,-1));
		mapGen.AddSpot(new TowerSpot(M,-1,-1));

		mapGen.AddSpot(new EnragerSpot(M,-1,-1));
		mapGen.AddSpot(new EnragerSpot(M,-1,-1));


		for(int x=0;x<w;x++)
		{
			for(int z=0;z<h;z++)
			{
				GameObject cellObj = (GameObject)Instantiate(cellPrefab);
				map[x,z] = cellObj.GetComponent<BlockController>();
				BlockController c = map[x,z];
				c.transform.parent = cellContainer.transform;
				c.InitCell(x,z,this);
			}
		}

		mapGen.GenerateMap(map, editMode);


		((BoxCollider)collider).size = new Vector3(w,0.2f,h);
		((BoxCollider)collider).center = new Vector3(CenterOfMap.x,-0.1f,CenterOfMap.z);

		//map[1,1].Digged=true;
	}


	void InitAstar()
	{
		int graphW = map.Width*10,graphH = map.Height*10;
		Vector3 pos = transform.position+new Vector3((float)map.Width/2f,0.2f,(float)map.Height/2f);
		
		((BoxCollider)collider).size = new Vector3(Map.Width,0.2f,Map.Height);
		((BoxCollider)collider).center = new Vector3(CenterOfMap.x,-0.1f,CenterOfMap.z);

		AstarPath.active.astarData.gridGraph.width=graphW;
		
		AstarPath.active.astarData.gridGraph.depth=graphH;
		
		
		AstarPath.active.astarData.gridGraph.center= pos;
		
		AstarPath.active.astarData.gridGraph.UpdateSizeFromWidthDepth ();

		AstarPath.active.Scan();
		
			

	}
	void OnDestroy () 
	{
		/*Debug.Log("OnDestroy");
		foreach(Transform children in cellContainer.transform)
		{
			

			DestroyImmediate(children.gameObject);
			
		}*/
	}


	void GenerateMesh(bool editMode)
	{

		for(int x=0;x<map.Width;x++)
		{
			for(int z=0;z<map.Height;z++)
			{
				map[x,z].BuildLightCache();
				
			}
		}

		Debug.Log("Generate Mesh");

		for(int x=0;x<map.Width;x++)
		{
			for(int z=0;z<map.Height;z++)
			{
				map[x,z].Generate(terrGen, editMode,false);

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
		for(int i=0;i<map.Height;i++)
		{
			for(int j=0;j<map.Width;j++)
			{
				map[i,j].SaveUid(b);
			}
		}
	}

	public override void LoadUid (Manager m, ReaderEx r)
	{
		base.LoadUid (m, r);
		for(int i=0;i<map.Height;i++)
		{
			for(int j=0;j<map.Width;j++)
			{
				
				map[i,j].LoadUid(m,r);
			}
		}
	}
	public void Save (WriterEx b)
	{
		for(int i=0;i<map.Height;i++)
		{
			for(int j=0;j<map.Width;j++)
			{
				map[i,j].Save(b);
			}
		}
	}
	public void Load (Manager m, ReaderEx r)
	{
		for(int i=0;i<map.Height;i++)
		{
			for(int j=0;j<map.Width;j++)
			{

				map[i,j].Load(m,r);
			}
		}
		GenerateMesh(false);
	}
	#endregion
}


  