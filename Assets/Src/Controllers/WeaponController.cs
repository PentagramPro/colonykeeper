using UnityEngine;
using System.Collections;

public class WeaponController : BaseController {

	enum Modes {
		Idle,Attack
	}

	public delegate void TargetNotification();
	public event TargetNotification OnTargetLost;
	public event TargetNotification OnTargetDestroyed;

	Modes state = Modes.Idle;
	VisualContact curContact;

	public float rotationSpeed = 5;
	public float fireDelay = 1;
	public float fireDamage = 100;
	public GameObject projectilePrefab;
	public Vector3 RelativeGunPosition;
	public Vector3 GunPosition{
		get{
			return RelativeGunPosition+transform.position;
		}
	}

	HullController owner;

	float fireCounter = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (curContact != null)
		{
			curContact.Update(GunPosition);

			if (curContact.IsTargetDestroyed())
			{
				if (OnTargetDestroyed != null)
					OnTargetDestroyed();
				state = Modes.Idle;
			}
		}
		switch(state)
		{
		case Modes.Attack:
			DoAttack();
			break;
		}
	}

	public void Attack(HullController owner,VisualContact target)
	{
		curContact = target;
		state = Modes.Attack;
		this.owner = owner;
	}

	public void Stop()
	{
		curContact = null;
		state = Modes.Idle;
	}
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(GunPosition,0.1f);
		Gizmos.DrawLine(GunPosition,GunPosition+Vector3.forward);
	}

	void DoAttack()
	{
		HullController target = curContact.Target;
		if(curContact.IsTargetVisible())
		{
			Quaternion dir = Quaternion.LookRotation(target.Center-transform.position);
			transform.localRotation=
				Quaternion.RotateTowards(transform.localRotation,dir,rotationSpeed*Time.smoothDeltaTime);
			fireCounter+=Time.smoothDeltaTime;
			if(fireCounter>fireDelay)
			{
				fireCounter=0;
				Vector3 gunDir = transform.rotation*Vector3.forward;
				Physics.Raycast(GunPosition,gunDir);
				Shoot (gunDir);
			}
		}
		else
		{
			state = Modes.Idle;
			curContact = null;
			if(OnTargetLost!=null)
				OnTargetLost();

		}


	}

	void Shoot(Vector3 dir)
	{

		

		
		ProjectileController proj = ((GameObject)Instantiate(projectilePrefab))
			.GetComponent<ProjectileController>();
		
		proj.Fire(owner,GunPosition,dir,fireDamage);
	}
}
