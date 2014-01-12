using UnityEngine;
using System.Collections;

public class BlockController : BaseController {

	public delegate void CellHandler(int i, int j);
	public event CellHandler CellUpdated;
	public event CellHandler CellMouseOver;
	public event CellHandler CellMouseUp;

	public Block BlockProt;
	public BuildingController cellBuilding;

	Color COLOR_DESIGNATED = new Color(0,0,1,0.5f);
	Color COLOR_DEFAULT = new Color(0,0,1,1);
	float amount=10;

	int posI, posJ;

	float halfCell = TerrainMeshGenerator.CELL_SIZE/2;

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

	public bool Dig(IInventory dest, float digAmount)
	{

		if(BlockProt!=null && BlockProt.Breakable)
		{
			digAmount = Mathf.Min(digAmount,amount);

			amount-=digAmount;
			if(BlockProt.ContainsItem!=null)
				dest.Put(BlockProt.ContainsItem,digAmount);
			if(amount<=0)
			{

				BlockProt=null;
				if(CellUpdated!=null)
					CellUpdated(posI,posJ);
				renderer.material.SetColor("_Color",COLOR_DEFAULT);
				return true;
			}


		}

		return false;

	}
	
	
	public void DesignateDigJob(JobManager jm)
	{
		if(jm.IsForDig(this))
		{
			jm.RemoveDigJob(this);
		}
		else
		{
			jm.AddDigJob(this);
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
	}

	public void Generate(Manager manager, TerrainMeshGenerator terrGen,  bool editMode)
	{
	
		
		GetComponent<MeshFilter>().sharedMesh=null;
		GetComponent<MeshFilter>().mesh=null;
		
		Mesh mesh = terrGen.Generate(posI,posJ);
		
		if(BlockProt!=null)
		{
			collider.enabled=true;
			Material mat = LoadMaterial(BlockProt.MaterialName);
			if(mat!=null)
				renderer.material = mat;
		}
		else
		{
			collider.enabled=false;
		}
		if(editMode)
			GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			GetComponent<MeshFilter>().mesh = mesh;
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
	public bool Build(GameObject building)
	{
		if(!CanBuild())
			return false;

		BuildingController bc = building.GetComponent<BuildingController>();

		if(bc==null)
			return false;

		bc.transform.parent=transform;
		bc.transform.localPosition=new Vector3(halfCell,0,halfCell);
		cellBuilding = bc;

		return true;
	}
	void UpdateCellColor(JobManager jm)
	{
		if(jm.IsForDig(this))
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

}
