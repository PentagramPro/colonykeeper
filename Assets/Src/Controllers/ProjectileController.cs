using UnityEngine;
using System.Collections;

public class ProjectileController : BaseManagedController {

	public float speed = 20;
	float damage=0;

	Vector3 direction = Vector3.zero;

	public float Damage{
		get{
			return damage;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		transform.Translate(direction*speed*Time.smoothDeltaTime);

	}

	void OnTriggerEnter(Collider other)
	{
		Destroy(gameObject);
	}
	public void Fire(Vector3 pos, Vector3 dir, float damage)
	{
		transform.position = pos;

		direction = dir.normalized;
		this.damage = damage;
	}
}
