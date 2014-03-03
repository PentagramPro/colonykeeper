using UnityEngine;
using System.Collections;

public class EnemyController : BaseManagedController {

	enum Modes {
		Inactive,Sentry,Attack
	}
	Modes state = Modes.Inactive;

	public TargeterController targeter;
	public WeaponController weapon;
	public VehicleController vehicle;

	VisualContact curContact = null;

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

	void OnFound(VisualContact target)
	{
		curContact = target;
		state = Modes.Attack;
		weapon.Attack(target);
	}

	void OnTargetLost()
	{
		curContact = null;
		state = Modes.Sentry;
		targeter.Search(vehicle.Side);
	}
}
