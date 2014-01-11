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

	public void GenerateFog(BlockController[,] map, bool editMode)
	{
		if(fogGen==null)
			fogGen = new FogOfWarMeshGenerator(map);

		Mesh mesh = fogGen.Generate(0,0);
		
		if(editMode)
			GetComponent<MeshFilter>().sharedMesh=mesh;
		else 
			GetComponent<MeshFilter>().mesh = mesh;
	}
}
