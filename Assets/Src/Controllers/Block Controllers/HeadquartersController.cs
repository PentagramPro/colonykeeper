using UnityEngine;
using System.Collections;

public class HeadquartersController : BaseManagedController {

	// Use this for initialization
	void Start () {
		M.cameraController.ShowPoint(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
