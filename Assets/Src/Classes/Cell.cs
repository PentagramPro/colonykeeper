using UnityEngine;
using System.Collections;
using System.IO;

public class Cell {

	int posI, posJ;
	Cell[,] map;

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
	public Block CellBlock;

	public BlockController CellBlockController;


	public GameObject cellObj;

	public bool Digged
	{
		get
		{
			return CellBlock==null || CellBlockController!=null;
		}
	}

	// [performance]
	// vertex indexes used by mesh generator 
	public int lt,lb,rt,rb;

	Material LoadMaterial(string name)
	{

		return Resources.Load("Materials/"+name, typeof(Material)) as Material;
	}

	public void DesignateDigJob(JobManager jm)
	{
		if(jm.IsForDig(this))
		{
			jm.RemoveDigJob(this);
			cellObj.renderer.material.SetColor("_Color",new Color(0,0,1,1f));
		}
		else
		{
			jm.AddDigJob(this);
			cellObj.renderer.material.SetColor("_Color",new Color(0,0,1,0.5f));
		}
	}

	public GameObject Generate(TerrainMeshGenerator terrGen, GameObject cellPrefab, GameObject parent, bool editMode)
	{
		if(cellObj==null)
		{
			cellObj = (GameObject)GameObject.Instantiate(cellPrefab);
			cellObj.transform.parent=parent.transform;
			cellObj.transform.localPosition=Vector3.zero;

		}

		cellObj.GetComponent<MeshFilter>().sharedMesh=null;
		cellObj.GetComponent<MeshFilter>().mesh=null;

		Mesh mesh = terrGen.Generate(posI,posJ);

		if(CellBlock!=null)
		{
			Material mat = LoadMaterial(CellBlock.MaterialName);
			if(mat!=null)
				cellObj.renderer.material = mat;
		}
		if(editMode)
			cellObj.GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			cellObj.GetComponent<MeshFilter>().mesh = mesh;



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
