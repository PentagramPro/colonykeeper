 using UnityEngine;
using System.Collections;

public class EnemyController : BaseManagedController, IStorable {

	enum Modes {
		Start,Inactive,Sentry,Attack,Intercept
	}
	//store
    Modes state = Modes.Start;
	//store and instantiate
	VisualContact curContact = null;

	public TargeterController targeter;
	public WeaponController weapon;
	public VehicleController vehicle;
	public bool Move = true;

	AITarget aiTarget = null;
	Vector3 startPosition;
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
		targeter.LocalTargetingPoint = weapon.transform.position+weapon.RelativeGunPosition-transform.position;

		weapon.OnTargetLost+=OnTargetLost;
		weapon.OnTargetDestroyed += OnTargetDestroyed;
		hull = GetComponent<HullController>();

		vehicle.OnPathWalked += OnPathWalked;
		vehicle.OnActivated += OnActivated;
		startPosition = transform.position;

	}
	
	// Update is called once per frame
	void Update () {


		switch(state)
		{
        case Modes.Start:
                state = Modes.Inactive;
                BlockController cell = M.terrainController.Map[vehicle.Hull.currentCell];
                if (cell.Discovered)
                    OnActivated();
                    
            break;
		case Modes.Inactive:
			break;
		case Modes.Sentry:
			if(M.AI.HasTargets)
				HandleAITarget(M.AI.GetFirstTarget());
			break;
		case Modes.Attack:
			break;
		
		}
	}
	void OnDestroy()
	{
		if(M!=null)
			M.AI.RemoveTarget(aiTarget);
	}

	void HandleAITarget(AITarget ait)
	{
		if(ait==null || ait.Target==null)
			return;

		state = Modes.Intercept;
		curContact = new VisualContact(ait.Target);
		curContact.LastPosition = ait.Target.Center;
		if(Move)
			vehicle.DriveTo(curContact.LastPosition);
		targeter.Search(vehicle.Hull.Side);
	}
	void OnActivated()
	{
        if (state == Modes.Inactive)
        {
            state = Modes.Sentry;
            targeter.Search(vehicle.Hull.Side);
        }
	}

	void SentryOrReturn()
	{
		if(Vector3.Distance(transform.position,startPosition)<1)
			state = Modes.Sentry;
		else
		{
			state = Modes.Intercept;
			vehicle.DriveTo(startPosition);
		}
	}
	void OnPathWalked()
	{
		if(state==Modes.Intercept)
		{
			SentryOrReturn();
		}
	}

	void OnFound(VisualContact target)
	{
		if(state == Modes.Intercept )
			vehicle.Stop();
		if(target.IsTargetBuilding())
		{

			M.AI.RemoveTarget(aiTarget);

			aiTarget = M.AI.CreateTarget(target.Target);
		}
		curContact = target;
		state = Modes.Attack;
		weapon.Attack(hull,target);
	}

	void OnTargetLost()
	{
		if(state==Modes.Intercept)
			return;

		state = Modes.Intercept;
		if(Move)
			vehicle.DriveTo(curContact.LastPosition);
		targeter.Search(vehicle.Hull.Side);
	}

	void OnTargetDestroyed()
	{
		M.AI.RemoveTarget(aiTarget);
		curContact = null;
		SentryOrReturn();
		targeter.Search(vehicle.Hull.Side);
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
