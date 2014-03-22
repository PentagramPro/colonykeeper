using UnityEngine;
using System.Collections;

public class WeaponController : BaseController, IStorable{

	enum Modes {
		Idle,Attack, OutOfAmmo
	}

	public delegate void TargetNotification();
	public event TargetNotification OnTargetLost;
	public event TargetNotification OnTargetDestroyed;

	UidContainer uidc;
	//store
	Modes state = Modes.Idle;
	//store link
	VisualContact curContact;
	//store
	float fireCounter = 0;
	int roundCounter = 1;

	int ammunition = 10;
	public float fireRoundDelay = 3;
	public int fireRoundSize = 4;

	public float rotationSpeed = 5;
	public float fireDelay = 0.5f;
	public float fireDamage = 100;


	public GameObject projectilePrefab;
	public Vector3 RelativeGunPosition;
	public Vector3 GunPosition{
		get{
			return RelativeGunPosition+transform.position;
		}
	}

	HullController owner;



	// Use this for initialization
	void Start () {
		uidc = new UidContainer(this);
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
		Debug.Log("Weapon "+GetHashCode()+" of hull "+owner.GetHashCode()+
		          " attacks target "+target.GetHashCode()+"(hull = "+target.Target.GetHashCode()+")");
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
			transform.rotation=
				Quaternion.RotateTowards(transform.rotation,dir,rotationSpeed*Time.smoothDeltaTime);
			fireCounter+=Time.smoothDeltaTime;

			if(roundCounter>0)
			{
				if(fireCounter>fireDelay)
				{
					fireCounter=0;
					roundCounter++;
					Vector3 gunDir = transform.rotation*Vector3.forward;
					Physics.Raycast(GunPosition,gunDir);
					Shoot (target);
					if(roundCounter>fireRoundSize)
					{
						roundCounter=0;
					}
				}
			}
			else
			{
				if(fireCounter>fireRoundDelay)
				{
					roundCounter = 1;
					fireCounter = fireDelay;

				}
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

	void Shoot(HullController target)
	{

		

		
		ProjectileController proj = ((GameObject)Instantiate(projectilePrefab,GunPosition,transform.rotation))
			.GetComponent<ProjectileController>();
		
		proj.Fire(owner,fireDamage,target);
		ammunition--;
		if(ammunition<=0)
			state = Modes.OutOfAmmo;
	}

	#region IStorable implementation

	public void SaveUid (WriterEx b)
	{
		uidc.Save(b);
	}

	public void LoadUid (Manager m, ReaderEx r)
	{
		uidc.Load(m,r);
	}

	public void Save (WriterEx b)
	{

		b.WriteEnum(state);
		b.WriteLink(curContact);
		b.Write ((double)fireCounter);

	}

	public void Load (Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));
		curContact = (VisualContact)r.ReadLink(m);
		fireCounter = (float)r.ReadDouble();
	}

	public int GetUID ()
	{
		return uidc.UID;
	}

	#endregion
}
