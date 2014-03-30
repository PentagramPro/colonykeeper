using UnityEngine;
using System.Collections.Generic;


public class RocketController : BaseManagedController, IStorable {

	ProjectileController proj;
	enum Modes{
		Idle,Lift,Fly,End
	}

	public Vector3 forcePos;
	public Vector3 ForcePos{
		get{
			return transform.localToWorldMatrix.MultiplyVector(forcePos);
		}
	}



	Modes state = Modes.Idle;

	float timer = 0;

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position+ForcePos,0.02f);
	}

	// Use this for initialization
	void Start () {
		proj = GetComponent<ProjectileController>();
		state = Modes.Lift;
		timer = 0;
		rigidbody.AddForce(new Vector3(Random.Range(-50,50),Random.Range(200,300),0));
	}
	
	void Update () {
		switch(state)
		{
		case Modes.Lift:
			
			if(timer>0.6f)
				state = Modes.Fly;
			break;
		case Modes.Fly:
			
			if(timer>2)
				Destroy(gameObject);
			break;
		}
		timer+=Time.smoothDeltaTime;
	}

	void FixedUpdate()
	{
		if(state==Modes.Fly)
		{
			Vector3 dir = (proj.Target.transform.position-transform.position).normalized;
			
			rigidbody.AddForceAtPosition(dir*10,ForcePos);
			//rigidbody.AddForce(Vector3.up*1);
		}
	}

	#region IStorable implementation


	public void Save(WriterEx b)
	{
		b.WriteEnum(state);
		b.Write((double)timer);
	}


	public void Load(Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));
		timer = (float)r.ReadDouble();
	}


	#endregion

}
