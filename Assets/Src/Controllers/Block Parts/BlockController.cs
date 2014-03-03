using UnityEngine;
using System.Collections;
using Pathfinding;
using System;

public class BlockController : BaseManagedController, ICustomer, IStorable {

	public enum Accessibility{
		Enclosed, // closed with blocks from all sides
		Cliff // has at least one cell without block among neibours
	}
	public delegate void CellHandler(int i, int j);
	public event CellHandler CellUpdated;
	public event CellHandler CellMouseOver;
	public event CellHandler CellMouseUp;

	public Block BlockProt;
	public BuildingController cellBuilding;
	public Accessibility IsAccessible;
	public GameObject ConstructionSitePrefab;

	DigJob digJob;

	Color COLOR_DESIGNATED = new Color(0,0,1,0.5f);
	Color COLOR_DEFAULT = new Color(0,0,1,1);
	int amount=1000;

	int posI, posJ;

	static float halfCell = TerrainMeshGenerator.CELL_SIZE/2;



	public void InitCell(int i, int j, BlockController[,] map)
	{

		posI=i;
		posJ=j;

		transform.localPosition = new Vector3(TerrainMeshGenerator.CELL_SIZE*j,0,TerrainMeshGenerator.CELL_SIZE*i);
		((BoxCollider)collider).center = new Vector3(halfCell,halfCell,halfCell);
		//cellObj.collider.isTrigger=false;
	}

	// Use this for initialization
	void Start () {
		if (ConstructionSitePrefab == null)
			throw new UnityException("ConstructionSitePrefab must not be null");
	
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
	
	// [performance]
	// vertex indexes used by mesh generator 
	public int lt,lb,rt,rb;

	public Item Contains{
		get{
			return BlockProt.ContainsItem;
		}
	}

	public void JobCompleted(IJob j)
	{
		digJob=null;

	}
	public DigResult Dig(IInventory dest, int digAmount)
	{
		DigResult res = DigResult.CannotDig;

		if(BlockProt!=null && BlockProt.Breakable)
		{
			digAmount = Math.Min(digAmount,amount);

			amount-=digAmount;
			if(BlockProt.ContainsItem!=null)
			{
				int left = dest.Put(BlockProt.ContainsItem,digAmount);
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
				if(CellUpdated!=null)
				{
					CellUpdated(posI,posJ);

					CellUpdated(posI-1,posJ);
					CellUpdated(posI+1,posJ);
					CellUpdated(posI,posJ-1);
					CellUpdated(posI,posJ+1);

					CellUpdated(posI-1,posJ-1);
					CellUpdated(posI-1,posJ+1);
					CellUpdated(posI+1,posJ-1);
					CellUpdated(posI+1,posJ+1);
				}
				digJob=null;
				res = DigResult.Finished;
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
			jm.AddJob(digJob,IsAccessible==Accessibility.Enclosed);
		}

		UpdateCellColor(jm);
	}


	// Update is called once per frame
	void Update () {
	
	}

	public void OnSelected()
	{
	}

	public void OnDeselected()
	{
	}

	void OnMouseOver() 
	{
		if(CellMouseOver!=null)
			CellMouseOver(posI,posJ);
	}

	void OnMouseUpAsButton() 
	{
		if(CellMouseUp!=null)
			CellMouseUp(posI,posJ);
		M.GetGUIController().SelectedObject = null;
		if (!Digged)
			DesignateDigJob();

	}

	public void Generate(BlockController[,] map,TerrainMeshGenerator terrGen,  bool editMode)
	{
		Manager manager = M;
		
		GetComponent<MeshFilter>().sharedMesh=null;
		GetComponent<MeshFilter>().mesh=null;

		// Creating mesh
		Mesh mesh = terrGen.Generate(map,posI,posJ);

		// Checking cell block
		if(BlockProt!=null)
		{
			// Cell with block
			collider.enabled=true;


			Material mat = LoadMaterial(BlockProt.MaterialName);
			if(mat!=null)
				renderer.material = mat;
		}
		else
		{
			collider.enabled=false;
		}

		//if(!editMode)
		//	AstarPath.active.UpdateGraphs(new GraphUpdateObject(collider.bounds));

		// checking accessibility
		Accessibility oldAc = IsAccessible;
		IsAccessible = terrGen.GetAccessibility(posI, posJ);
		if(digJob!=null && oldAc==Accessibility.Enclosed && IsAccessible==Accessibility.Cliff)
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
				
		conC.TargetGameObject = building;
		conC.Construct(resBC.Prototype,recipe);

		conC.ParentBlock = this;




		BuildOn(conBC);

		return true;
	}

	public void  BuildOn(BuildingController building)
	{
		building.transform.parent = transform;
		building.transform.localPosition=new Vector3(halfCell,0,halfCell);
		cellBuilding = building;
		if(M!=null && M.BuildingsRegistry!=null)
			M.BuildingsRegistry.Add (this, building);
		building.OnBuilded();

		GraphUpdateObject guo = new GraphUpdateObject(building.collider.bounds);
		AstarPath.active.UpdateGraphs(guo);
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



	Material LoadMaterial(string name)
	{
		
		return Resources.Load("Materials/"+name, typeof(Material)) as Material;
	}

	public enum DigResult
	{
		CannotDig, NotFinished, Finished, DestinationFull
	}

	#region IStorable implementation



	public void Save (WriterEx b)
	{


		b.Write(amount);
		
		b.WriteEx(BlockProt);
		b.WriteLink(digJob);


	}
	public void Load (Manager m, ReaderEx r)
	{
		digJob = null;
		cellBuilding = null;


		amount = r.ReadInt32();
		
		M.GameD.BlocksByName.TryGetValue(r.ReadString(),out BlockProt);
		digJob = (DigJob)r.ReadLink(m);

	}
	#endregion
}
