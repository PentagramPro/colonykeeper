using UnityEngine;
using System.Collections;

public class BlockController : BaseController {

	public delegate void CellHandler(int i, int j);
	public event CellHandler CellUpdated;
	public event CellHandler CellMouseOver;
	public event CellHandler CellMouseUp;

	public Block BlockProt;

	Color COLOR_DESIGNATED = new Color(0,0,1,0.5f);
	Color COLOR_DEFAULT = new Color(0,0,1,1);
	public string Name = "Lamp";

	int posI, posJ;
	BlockController[,] mapRef;
	float halfCell = TerrainMeshGenerator.CELL_SIZE/2;

	public void InitCell(int i, int j, BlockController[,] map)
	{

		posI=i;
		posJ=j;
		mapRef=map;
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
			return BlockProt==null || !BlockProt.IsDiggable();
		}
	}
	
	// [performance]
	// vertex indexes used by mesh generator 
	public int lt,lb,rt,rb;
	

	
	public void Dig()
	{
		if(BlockProt!=null && BlockProt.IsDiggable())
		{
			BlockProt=null;
			if(CellUpdated!=null)
				CellUpdated(posI,posJ);
		}
		renderer.material.SetColor("_Color",COLOR_DEFAULT);
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
			Material mat = LoadMaterial(BlockProt.GetMaterialName());
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

	void OnDrawGizmos()
	{
		Vector3 center = transform.position+new Vector3(0,0.5f,0);
		Gizmos.DrawWireCube(center,new Vector3(1,1,1));
		Gizmos.DrawWireSphere(transform.position,0.2f);
	}

	Material LoadMaterial(string name)
	{
		
		return Resources.Load("Materials/"+name, typeof(Material)) as Material;
	}

}
