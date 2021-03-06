﻿using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using System;


public class BlockController : BaseManagedController, ICustomer, IStorable {

	public delegate void MinedDelegate();
	public delegate void DiscoveredDelegate();

	public enum Accessibility{
		Unknown, // this block is undiscovered
		Enclosed, // closed with blocks from all sides
		Cliff // has at least one cell without block among neibours
	}



	//public Block BlockProt;
	public BuildingController cellBuilding;
	public EffectController dustFXPrefab;
	EffectController dustFX;


	// This value is calculated during cell update from following variables:
	// - BlockProt
	// - Discovered
	[HideInInspector]
	public Accessibility IsAccessible;

	public event MinedDelegate OnMined;
	public event DiscoveredDelegate OnDiscovered;

	public GameObject ConstructionSitePrefab;

	DigJob digJob;

	[SerializeField]
	TerrainController terrainController;

	public Map Map
	{
		get
		{
			return terrainController.Map;
		}
	}



	public string Name;

	public bool Breakable = true;
	public bool discovered = false;

	Color COLOR_DESIGNATED = new Color(0,0,1,0.5f);
	Color COLOR_DEFAULT = new Color(0,0,1,1);
	int amount=1000;
	float leftover = 0;

	[SerializeField]
	MapPoint mapPos;

	[SerializeField]
	List<StaticLight> StaticLights = new List<StaticLight>();
	List<StaticLight> StaticLightsCache = new List<StaticLight>();


	public MapPoint MapPos{
		get
		{
			return mapPos;
		}
	}

	//int posX, posZ;

	public List<VehicleController> ObjectsCache = new List<VehicleController>();

	static float halfCell = TerrainMeshGenerator.CELL_SIZE/2;



	public void InitCell(int x, int z, TerrainController tcon)
	{
		//because we call it from editor
		PrepareManager();

		terrainController = tcon;

		mapPos.X=x;
		mapPos.Z=z;

		transform.localPosition = new Vector3(TerrainMeshGenerator.CELL_SIZE*x,0,TerrainMeshGenerator.CELL_SIZE*z);
		((BoxCollider)collider).center = new Vector3(halfCell,halfCell,halfCell);
		//cellObj.collider.isTrigger=false;
	}

	public Block BlockProt
	{
		get
		{
			Block res = null;
			M.GameD.BlocksByName.TryGetValue(Name,out res);
			return res;
		}
		set
		{
			if(value!=null)
				Name = value.Name;
			else
				Name = "";
		}
	}




	// Use this for initialization
	void Start () {
		if (ConstructionSitePrefab == null)
			throw new UnityException("ConstructionSitePrefab must not be null");
	
		GetComponent<TapController>().OnTap+=OnTap;
		terrainController.OnCellUpdated(MapPos.X,MapPos.Z);
	}


	public Vector3 Position
	{
		get{
			return transform.position+new Vector3(halfCell,0,halfCell);
		}
	}


	public bool Digged
	{
		get
		{
			return BlockProt==null;
		}
	}

	public bool Discovered
	{
		get{
			return discovered;
		}
		set{
			if(value==true && discovered==false)
			{
				Activate();
				if(OnDiscovered!=null)
					OnDiscovered();
			}
			discovered = value;
		}
	}
	// [performance]
	// vertex indexes used by mesh generator 
	public int lt,lb,rt,rb;

	public Item ContainedItem{
		get{
			if(!BlockProt.HasItem)
				return null;
			return M.GameD.Items[BlockProt.StoredItem];
		}
	}

	public void JobCanceled(IJob job)
	{
		digJob = null;
	}

	public void JobCompleted(IJob j)
	{
		digJob=null;

	}



	private void UpdateAdjacentCells()
	{

		int xl = Math.Max(mapPos.X-2,0);
		int zl = Math.Max(mapPos.Z-2,0);
		int xh = Math.Min(mapPos.X+1,Map.Width-1);
		int zh = Math.Min(mapPos.Z+1,Map.Height-1);
		for(int x=xl;x<=xh;x++)
			for(int z=zl;z<=zh;z++)
				terrainController.OnCellUpdated(x,z);
			


	}
	public DigResult Dig(IInventory dest)
	{
		float fdigAmount = BlockProt.DigSpeed*Time.smoothDeltaTime*100+leftover;
		int digAmount = (int)fdigAmount;
		leftover = fdigAmount-digAmount;

		DigResult res = DigResult.CannotDig;

		if(BlockProt!=null && BlockProt.Breakable)
		{
			digAmount = Math.Min(digAmount,amount);

			amount-=digAmount;
			if(dustFX==null)
			{
				GameObject obj = (GameObject)GameObject.Instantiate(dustFXPrefab.gameObject);
				dustFX = (EffectController)obj.GetComponent<ParticleFxController>();
				dustFX.transform.parent = transform;
				dustFX.transform.localPosition = new Vector3(0.5f,0.3f,0.5f);
			}
			dustFX.Spark(0.1f);
			if(BlockProt.HasItem)
			{
				Pile minedOre = new Pile(ContainedItem, digAmount);
				minedOre.Properties = ContainedItem.BaseProperties.copy();

				int left = dest.Put(minedOre);
				if(left>0)
				{
					res = DigResult.DestinationFull;
					amount+=left;
				}
				else
					res = DigResult.NotFinished;
			}
			if(amount<=0)
			{

				BlockProt=null;

				UpdateAdjacentCells();



				terrainController.Discover(mapPos.X,mapPos.Z);

				digJob=null;
				res = DigResult.Finished;

				if(OnMined!=null)
					OnMined();
			}


		}

		return res;

	}
	
	
	public void DesignateDigJob()
	{
		JobManager jm = M.JobManager;

		if(digJob!=null)
		{
			if(!jm.RemoveJob(digJob))
				digJob.Cancel();
			digJob=null;
		}
		else
		{
			digJob = new DigJob(jm,this,this);
			jm.AddJob(digJob,IsAccessible!=Accessibility.Cliff);
		}

		UpdateCellColor(jm);
	}


	// Update is called once per frame
	void Update () {
	
	}

	public void AddLight(StaticLight light)
	{
		StaticLights.Add(light);
	}

	public void RemoveAllLights(Component owner)
	{
		List<StaticLight> remove = new List<StaticLight>();
		foreach(StaticLight l in StaticLights)
		{
			if(l.Owner==owner)
				remove.Add(l);
		}

		foreach(StaticLight l in remove)
		{
			StaticLights.Remove(l);
		}

	}

	public List<StaticLight> GetStaticLightsCache()
	{
		return StaticLightsCache;
	}

	public List<StaticLight> GetStaticLigths()
	{
		return StaticLights;
	}

	public void OnSelected()
	{
	}

	public void OnDeselected()
	{
	}

	void OnMouseOver() 
	{

			terrainController.OnCellHover(mapPos.X,mapPos.Z);
	}

	void OnTap()
	{
	
			
			terrainController.OnCellClicked(mapPos.X,mapPos.Z);
			M.GetGUIController().SelectedObject = null;
			if (!Digged)
				DesignateDigJob();
	
	}

	public void BuildLightCache()
	{
		//building light cache
		StaticLightsCache.Clear();

		Map.EnumerateCells(mapPos.X-1,mapPos.Z-1,mapPos.X+1,mapPos.Z+1, (int x,int z)=>{
			foreach(StaticLight l in Map[x,z].GetStaticLigths())
			{
				StaticLightsCache.Add(l);
			}
		});
	}

	public void Generate(TerrainMeshGenerator terrGen,  bool editMode,bool updateAstar)
	{
		Manager manager = M;



		GetComponent<MeshFilter>().sharedMesh=null;
		GetComponent<MeshFilter>().mesh=null;

		// Creating mesh
		Mesh mesh = terrGen.Generate(Map,mapPos.X,mapPos.Z);

		// Checking cell block
		if(BlockProt!=null)
		{
			// Cell with block
			collider.enabled=true;


			Material mat = LoadMaterial(BlockProt.PrefabName);
			if(mat!=null)
				renderer.material = mat;
		}
		else
		{
			collider.enabled=false;
		}

		if(!editMode && updateAstar)
		{
			Bounds b = collider.bounds;
			b.Expand(1);
			AstarPath.active.UpdateGraphs(new GraphUpdateObject(b));
		}

		// checking accessibility
		Accessibility oldAc = IsAccessible;
		IsAccessible = terrGen.GetAccessibility(mapPos.X, mapPos.Z);
		if(digJob!=null && oldAc!=Accessibility.Cliff && IsAccessible==Accessibility.Cliff)
			M.JobManager.UnblockJob(digJob);

		// Setting mesh to component
		if(editMode)
			GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			GetComponent<MeshFilter>().mesh = mesh;

		//Updating color 
		if(!editMode)
		{
			UpdateCellColor(manager.JobManager);
		}

	}

	public bool CanBuild()
	{
		if(cellBuilding!=null || BlockProt!=null)
			return false;

		return true;
	}
	public bool Build(Manager m,GameObject building, RecipeInstance recipe)
	{
		if(!CanBuild())
			return false;

		GameObject conSite = M.GameD.BuildingsByName["Construction Site"].Instantiate();
		ConstructionController conC = conSite.GetComponent<ConstructionController>();
		BuildingController conBC = conSite.GetComponent<BuildingController>();
		BuildingController resBC = building.GetComponent<BuildingController>();

		if(conBC==null || conC==null || resBC==null)
			return false;
				
		conC.TargetGameObject = resBC;
		conC.Construct(recipe);

		conC.ParentBlock = this;




		BuildImmediate(conBC);

		return true;
	}

	public void RemoveBuildingImmediate()
	{
		if(cellBuilding==null)
			return;
		RemoveAllLights(cellBuilding);
		DestroyImmediate(cellBuilding.gameObject);
		cellBuilding = null;

		Bounds b = collider.bounds;
		b.Expand(1);
		GraphUpdateObject guo = new GraphUpdateObject(b);
		AstarPath.active.UpdateGraphs(guo);

		UpdateAdjacentCells();
	}

	public void  BuildImmediate(BuildingController building)
	{
		building.transform.parent = transform;
		building.transform.localPosition=new Vector3(halfCell,0,halfCell);
		cellBuilding = building;
		if(M!=null && M.BuildingsRegistry!=null)
		{
			M.BuildingsRegistry.Add (this, building);
		}
		else
		{
			building.ToRegistryOnStart = true;
		}
		building.OnBuilded(this);

		Bounds b = collider.bounds;
		b.Expand(1);
		GraphUpdateObject guo = new GraphUpdateObject(b);
		AstarPath.active.UpdateGraphs(guo);

		UpdateAdjacentCells();
	}
	void UpdateCellColor(JobManager jm)
	{
		if(digJob!=null)
		{
			renderer.material.SetColor("_Color",COLOR_DESIGNATED);
		}
		else
		{
			renderer.material.SetColor("_Color",COLOR_DEFAULT);
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		if(Discovered)
			Gizmos.DrawWireCube(new Vector3(0.5f,0.5f,0.5f)+transform.position, new Vector3(0.3f,0.3f,0.3f));
		foreach(StaticLight l in StaticLights)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(l.GlobalPosition,0.15f);
		}
	}


	Material LoadMaterial(string name)
	{
		
		return Resources.Load("Materials/"+name, typeof(Material)) as Material;
	}

	public enum DigResult
	{
		CannotDig, NotFinished, Finished, DestinationFull
	}


	void Activate()
	{
		foreach(VehicleController v in ObjectsCache)
		{
			v.Activate();
		}
	}
	#region IStorable implementation



	public void Save (WriterEx b)
	{


		b.Write(amount);
		
		b.WriteEx(BlockProt);
		b.WriteLink(digJob);

		b.Write(Discovered);
	}
	public void Load (Manager m, ReaderEx r)
	{
		digJob = null;
		cellBuilding = null;


		amount = r.ReadInt32();
		
		//M.GameD.BlocksByName.TryGetValue(r.ReadString(),out BlockProt);
		digJob = (DigJob)r.ReadLink(m);
		Discovered = r.ReadBoolean();

	}
	#endregion
}
