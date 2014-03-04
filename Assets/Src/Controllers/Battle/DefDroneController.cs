using UnityEngine;
using System.Collections;

public class DefDroneController : BaseManagedController {

	enum Modes {
		Inactive,Intercept,Attack
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
		weapon.OnTargetDestroyed += OnTargetDestroyed;
	}
	
	// Update is called once per frame
	void Update () {
		
		
		switch(state)
		{
		case Modes.Inactive:
			break;
		case Modes.Intercept:
			curContact.Update(weapon.GunPosition);
			if(curContact.IsTargetVisible())
			{
				vehicle.Stop();
				state = Modes.Attack;
				weapon.Attack(curContact);
			}
			break;
		case Modes.Attack:
			curContact.Update(weapon.GunPosition);
			break;
		}
	}

	public void Attack(HullController target)
	{
		curContact = new VisualContact(target);
		state = Modes.Intercept;
		vehicle.DriveTo(target.Center);
	}

	public void Stop()
	{
		weapon.Stop();
		vehicle.Stop();
		state = Modes.Inactive;
	}
	
	void OnFound(VisualContact target)
	{

	}
	
	void OnTargetLost()
	{

	}
	
	void OnTargetDestroyed()
	{
		curContact = null;
		state = Modes.Inactive;
	}
}
