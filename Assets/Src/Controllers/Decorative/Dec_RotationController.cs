using UnityEngine;
using System.Collections;

public class Dec_RotationController : MonoBehaviour {

	public float RotationSpeed=10;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(Vector3.up, Time.smoothDeltaTime*RotationSpeed);
	}
}
