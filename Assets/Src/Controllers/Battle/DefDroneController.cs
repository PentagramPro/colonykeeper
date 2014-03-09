using UnityEngine;
using System.Collections;

public class DefDroneController : BaseManagedController, IStorable {

	enum Modes {
		Inactive,Intercept,Attack
	}
	//store
	Modes state = Modes.Inactive;

	//store and instantiate
	VisualContact curContact = null;
	
	public TargeterController targeter;
	public WeaponController weapon;
	public VehicleController vehicle;

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
				vehicle.Stop(0.2f);
				state = Modes.Attack;
				weapon.Attack(hull,curContact);
			}
			break;
		case Modes.Attack:
			//curContact.Update(weapon.GunPosition+transform.position);
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
		if(state == Modes.Attack)
			state = Modes.Intercept;
	}
	
	void OnTargetDestroyed()
	{
		curContact = null;
		state = Modes.Inactive;
	}

	#region IStorable implementation
	public override void SaveUid (WriterEx b)
	{

		if(curContact!=null)
		{
			b.Write(true);
			curContact.SaveUid(b);
		}
		else
			b.Write(false);

		base.SaveUid (b);
	}

	public override void LoadUid (Manager m, ReaderEx r)
	{
		if(r.ReadBoolean())
		{
			curContact = new VisualContact();
			curContact.LoadUid(m,r);
		}
		base.LoadUid (m, r);
	}

	public void Save (WriterEx b)
	{
		b.WriteEnum(state);
		if(curContact!=null)
			curContact.Save(b);
	}

	public void Load (Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));
		if(curContact!=null)
			curContact.Load(m,r);
	}

	#endregion
}
