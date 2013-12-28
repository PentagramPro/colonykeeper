using UnityEngine;
using System.Collections;

public class FogController : BaseController {


	FogOfWarMeshGenerator fogGen = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GenerateFog(Cell[,] map, bool editMode)
	{
		if(fogGen==null)
			fogGen = new FogOfWarMeshGenerator(map);

		Mesh mesh = fogGen.Generate();
		
		if(editMode)
			GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			GetComponent<MeshFilter>().mesh = mesh;
	}
}
