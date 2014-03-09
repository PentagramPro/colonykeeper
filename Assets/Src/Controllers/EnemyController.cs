using UnityEngine;
using System.Collections;

public class EnemyController : BaseManagedController {

	enum Modes {
		Inactive,Sentry,Attack,Intercept
	}
	Modes state = Modes.Inactive;

	public TargeterController targeter;
	public WeaponController weapon;
	public VehicleController vehicle;

	VisualContact curContact = null;

	HullController hull;

	// Use this for initialization
	void Start () {
		if(targeter==null)
			throw new UnityException("Targeter must not be null");

		if(weapon==null)
			throw new UnityException("Weapon must not be null");

		if(vehicle==null)
			throw new UnityException("Vehicle must not be null");

		targeter.OnFound+=OnFound;
		weapon.OnTargetLost+=OnTargetLost;
		weapon.OnTargetDestroyed += OnTargetDestroyed;
		hull = GetComponent<HullController>();
		vehicle.OnPathWalked+=OnPathWalked;
	}
	
	// Update is called once per frame
	void Update () {


		switch(state)
		{
		case Modes.Inactive:
			state = Modes.Sentry;
			targeter.Search(vehicle.Side);
			break;
		case Modes.Sentry:
			break;
		case Modes.Attack:
			break;
		
		}
	}


	void OnPathWalked()
	{
		if(state==Modes.Intercept)
			state = Modes.Sentry;
	}

	void OnFound(VisualContact target)
	{
		if(state == Modes.Intercept)
			vehicle.Stop(0.1f);
		curContact = target;
		state = Modes.Attack;
		weapon.Attack(hull,target);
	}

	void OnTargetLost()
	{

		state = Modes.Intercept;
		vehicle.DriveTo(curContact.LastPosition);
		targeter.Search(vehicle.Side);
	}

	void OnTargetDestroyed()
	{
		curContact = null;
		state = Modes.Sentry;
		targeter.Search(vehicle.Side);
	}
}
