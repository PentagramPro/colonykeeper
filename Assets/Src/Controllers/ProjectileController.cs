using UnityEngine;
using System.Collections;

public class ProjectileController : BaseManagedController {

	public float speed = 20;
	float damage=0;

	HullController owner;
	public HullController Owner{
		get{
			return owner;
		}
	}
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
		if(other.tag!="Projectile")
			Destroy(gameObject);
	}
	public void Fire(HullController owner,Vector3 pos, Vector3 dir, float damage)
	{
		this.owner = owner;
		transform.position = pos;

		direction = dir.normalized;
		this.damage = damage;
	}
}
