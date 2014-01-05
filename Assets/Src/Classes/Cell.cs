using UnityEngine;
using System.Collections;
using System.IO;

public class Cell {

	int posI, posJ;

	public Cell(int i, int j)
	{
		posI=i;
		posJ=j;
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
			return CellBlock==null;
		}
	}

	// [performance]
	// vertex indexes used by mesh generator 
	public int lt,lb,rt,rb;

	Material LoadMaterial(string name)
	{

		return Resources.Load("Materials/"+name, typeof(Material)) as Material;
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


}
