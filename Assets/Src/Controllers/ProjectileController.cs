using UnityEngine;
using System.Collections;

public class ProjectileController : BaseManagedController {

	public float speed = 20;
	Vector3 direction = Vector3.zero;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate(direction*speed*Time.smoothDeltaTime);

	}

	void OnCollisionEnter(Collision col)
	{
		GameObject.Destroy(gameObject);
	}
	public void Fire(Vector3 pos, Vector3 dir)
	{
		transform.position = pos;
		direction = dir.normalized;
	}
}
