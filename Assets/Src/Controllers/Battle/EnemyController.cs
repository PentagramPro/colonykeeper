using UnityEngine;
using System.Collections;

public class EnemyController : BaseManagedController, IStorable {

	enum Modes {
		Inactive,Sentry,Attack,Intercept
	}
	//store
	Modes state = Modes.Inactive;
	//store and instantiate
	VisualContact curContact = null;

	public TargeterController targeter;
	public WeaponController weapon;
	public VehicleController vehicle;
	public bool Move = true;



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

		vehicle.OnPathWalked += OnPathWalked;
		vehicle.OnActivated += OnActivated;

	}
	
	// Update is called once per frame
	void Update () {


		switch(state)
		{
		case Modes.Inactive:
			break;
		case Modes.Sentry:
			break;
		case Modes.Attack:
			break;
		
		}
	}

	void OnActivated()
	{
		state = Modes.Sentry;
		targeter.Search(vehicle.Side);
	}


	void OnPathWalked()
	{
		if(state==Modes.Intercept)
			state = Modes.Sentry;
	}

	void OnFound(VisualContact target)
	{
		if(state == Modes.Intercept )
			vehicle.Stop(0.1f);
		curContact = target;
		state = Modes.Attack;
		weapon.Attack(hull,target);
	}

	void OnTargetLost()
	{

		state = Modes.Intercept;
		if(Move)
			vehicle.DriveTo(curContact.LastPosition);
		targeter.Search(vehicle.Side);
	}

	void OnTargetDestroyed()
	{
		curContact = null;
		state = Modes.Sentry;
		targeter.Search(vehicle.Side);
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
