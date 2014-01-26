using UnityEngine;
using System.Collections;

public class BuildingController : BaseController {

	public Building Prototype = null;
	float halfCell = TerrainMeshGenerator.CELL_SIZE/2;
	// Use this for initialization
	void Start () {
	
	}

	void OnDrawGizmos()
	{
		Vector3 center = transform.position+new Vector3(0,0.5f,0);
		Gizmos.DrawWireCube(center,new Vector3(1,1,1));
		Gizmos.DrawWireSphere(transform.position,0.2f);
	}

	public Vector3 Position
	{
		get{
			return transform.position+new Vector3(halfCell,0,halfCell);
		}
	}

	public void OnBuilded()
	{
		collider.enabled = true;
	}


	// Update is called once per frame
	void Update () {
	
	}
}
