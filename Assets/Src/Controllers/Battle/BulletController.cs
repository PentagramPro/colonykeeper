using UnityEngine;
using System.Collections;

public class BulletController : MonoBehaviour {

	public float speed;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.forward*speed*Time.smoothDeltaTime);
	}
}
