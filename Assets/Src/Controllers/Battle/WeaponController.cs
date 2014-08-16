using UnityEngine;
using System.Collections;

public class WeaponController : BaseController, IStorable{

	enum Modes {
		Idle,Attack, OutOfAmmo
	}

	public delegate void TargetNotification();
	public event TargetNotification OnTargetLost;
	public event TargetNotification OnTargetDestroyed;
	public event TargetNotification OnOutOfAmmo;

	UidContainer uidc;
	//store
	Modes state = Modes.Idle;
	//store link
	VisualContact curContact;
	//store
	float fireCounter = 0;
	int roundCounter = 1;


	public bool InfiniteAmmo = false;

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
			return transform.TransformPoint(RelativeGunPosition);
		}
	}

	public float DPS
	{
		get{
			return fireDamage*fireRoundSize / (fireRoundDelay+ (fireRoundSize-1)*fireDelay);
		}
	}

	float reloadProgress=1;
	public float ReloadProgress{
		get{
			return reloadProgress;
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
				curContact = null;
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
				reloadProgress = 1.0f-(roundCounter-1.0f)/(float)fireRoundSize;
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
						reloadProgress=0;
					}
				}
			}
			else
			{
				reloadProgress = fireCounter/fireRoundDelay;
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


	public int Ammunition{
		get{
			return ammunition;
		}
		set{
			ammunition = value;
			if(ammunition>0)
				state = Modes.Idle;
		}
	}


	void Shoot(HullController target)
	{

		

		
		ProjectileController proj = ((GameObject)Instantiate(projectilePrefab,GunPosition,transform.rotation))
			.GetComponent<ProjectileController>();
		
		proj.Fire(owner,fireDamage,target);
		if(!InfiniteAmmo)
		{
			ammunition--;
			if(ammunition<=0)
			{
				state = Modes.OutOfAmmo;
				if(OnOutOfAmmo!=null)
					OnOutOfAmmo();
			}
		}
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
		
		b.Write(roundCounter);
		b.Write(ammunition);
	}

	public void Load (Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));
		curContact = (VisualContact)r.ReadLink(m);
		fireCounter = (float)r.ReadDouble();

		roundCounter = r.ReadInt32();
		ammunition = r.ReadInt32();
	}

	public int GetUID ()
	{
		return uidc.UID;
	}

	#endregion
}
