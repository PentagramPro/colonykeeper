using UnityEngine;
using System.Collections;

public class WeaponController : BaseController {

	enum Modes {
		Idle,Attack
	}
	Modes state = Modes.Idle;
	HullController target;

	public float rotationSpeed = 5;
	public float fireDelay = 1;
	public float fireDamage = 60;
	public GameObject projectilePrefab;

	float fireCounter = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		switch(state)
		{
		case Modes.Attack:
			DoAttack();
			break;
		}
	}

	public void Attack(HullController target)
	{
		this.target = target;
		state = Modes.Attack;
	}

	void DoAttack()
	{

		Quaternion dir = Quaternion.LookRotation(target.transform.position-transform.position);
		transform.localRotation=
			Quaternion.RotateTowards(transform.localRotation,dir,rotationSpeed*Time.smoothDeltaTime);
		fireCounter+=Time.smoothDeltaTime;
		if(fireCounter>fireDelay)
		{
			fireCounter=0;

			Vector3 gunDir = transform.rotation*Vector3.forward;

			ProjectileController proj = ((GameObject)GameObject.Instantiate(projectilePrefab))
				.GetComponent<ProjectileController>();

			proj.Fire(transform.position,gunDir);
		}


	}
}
