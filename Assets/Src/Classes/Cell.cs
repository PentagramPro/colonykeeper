using UnityEngine;
using System.Collections;
using System.IO;
using Pathfinding;

public class Cell {


	public delegate void CellUpdatedHandler();
	public event CellUpdatedHandler CellUpdated;


	Color COLOR_DESIGNATED = new Color(0,0,1,0.5f);
	Color COLOR_DEFAULT = new Color(0,0,1,1);
	int posI, posJ;
	Cell[,] map;
	float half_cell = TerrainMeshGenerator.CELL_SIZE/2;
	public Cell(int i, int j,Cell[,] m)
	{
		posI=i;
		posJ=j;
		map=m;
	}

	//public Mesh mapMesh;
	public Color Lighting = new Color(0,0,0);


	// [data]
	// block attached to this cell
	//public Block CellBlock;

	//public BlockController CellBlockController;
	public IBlock CellBlock;

	public Vector3 Position
	{
		get{
			return cellObj.transform.position+new Vector3(half_cell,0,half_cell);
		}
	}
	public GameObject cellObj;

	public bool Digged
	{
		get
		{
			return CellBlock==null || !CellBlock.IsDiggable();
		}
	}

	// [performance]
	// vertex indexes used by mesh generator 
	public int lt,lb,rt,rb;

	Material LoadMaterial(string name)
	{

		return Resources.Load("Materials/"+name, typeof(Material)) as Material;
	}

	public void Dig()
	{
		if(CellBlock!=null && CellBlock.IsDiggable())
		{
			CellBlock=null;
			if(CellUpdated!=null)
				CellUpdated();
		}
		cellObj.renderer.material.SetColor("_Color",COLOR_DEFAULT);
	}

	void UpdateCellColor(JobManager jm)
	{
		if(jm.IsForDig(this))
		{
			cellObj.renderer.material.SetColor("_Color",COLOR_DESIGNATED);
		}
		else
		{
			cellObj.renderer.material.SetColor("_Color",COLOR_DEFAULT);
		}
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

	public GameObject Generate(Manager manager, TerrainMeshGenerator terrGen, GameObject cellPrefab, GameObject parent, bool editMode)
	{
		if(cellObj==null)
		{
			float halfCell = TerrainMeshGenerator.CELL_SIZE/2;
			cellObj = (GameObject)GameObject.Instantiate(cellPrefab);
			cellObj.transform.parent=parent.transform;
			cellObj.transform.localPosition=new Vector3(posJ*TerrainMeshGenerator.CELL_SIZE,0,posI*TerrainMeshGenerator.CELL_SIZE);
			((BoxCollider)cellObj.collider).center = new Vector3(halfCell,halfCell,halfCell);
			cellObj.collider.isTrigger=false;
		}

		cellObj.GetComponent<MeshFilter>().sharedMesh=null;
		cellObj.GetComponent<MeshFilter>().mesh=null;

		Mesh mesh = terrGen.Generate(posI,posJ);

		if(CellBlock!=null)
		{
			cellObj.collider.enabled=true;
			Material mat = LoadMaterial(CellBlock.GetMaterialName());
			if(mat!=null)
				cellObj.renderer.material = mat;
		}
		else
		{
			cellObj.collider.enabled=false;
		}
		if(editMode)
			cellObj.GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			cellObj.GetComponent<MeshFilter>().mesh = mesh;
		if(!editMode)
		{
			UpdateCellColor(manager.JobManager);



		}
		return cellObj;
	}

	public bool IsEnclosed()
	{
		if(posI>0 && map[posI-1,posJ].Digged)
			return false;
		if(posJ>0 && map[posI,posJ-1].Digged)
			return false;
		if(posI<map.GetUpperBound(0) && map[posI+1,posJ].Digged)
			return false;
		if(posJ<map.GetUpperBound(1) && map[posI,posJ+1].Digged)
			return false;
		return true;
	}
}
