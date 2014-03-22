using UnityEngine;
using System.Collections;
using System;

public class ProjectileController : BaseManagedController {

	float damage=0;
	WeakReference target;
	public HullController Target{
		get{
			if(target.IsAlive)
				return (HullController)target.Target;
			else return null;
		}

		set{
			target = new WeakReference(value);
		}
	}

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


	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag!="Projectile")
			Destroy(gameObject);
	}
	public void Fire(HullController owner,float damage, HullController target)
	{
		this.owner = owner;
		Target = target;


		this.damage = damage;
	}
}
