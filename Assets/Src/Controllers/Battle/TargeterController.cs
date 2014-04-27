using UnityEngine;
using System.Collections.Generic;

public class TargeterController : BaseManagedController, IStorable {

	enum Modes {
		Idle,Search,Delay
	}
	Modes state = Modes.Idle;


	float delay = 0;


	public float Range = 6;
	public delegate void TargetFoundDelegate(VisualContact contact);
	public event TargetFoundDelegate OnFound;


	// at start()
	HullController self;

	// temp list, do not save
	List<HullController> targetsList = new List<HullController>();

	Manager.Sides currentSide;
	// Use this for initialization
	void Start () {
		self = GetComponent<HullController>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(state==Modes.Search)
		{
			targetsList.Clear();
			ClosestTargets();

			foreach(HullController hull in targetsList)
			{
				RaycastHit hit;
				if(Physics.Raycast(self.Center,hull.Center-self.Center,out hit,Range))
				{
					if(hit.transform!=hull.transform)
						continue;

					if(OnFound!=null)
						OnFound(new VisualContact(hull));
					state = Modes.Idle;
					break;
				}
			}

			if(state==Modes.Search)
			{
				delay=0;
				state = Modes.Delay;
			}
		}
		else if(state==Modes.Delay)
		{
			delay+=Time.deltaTime;
			if(delay>1)
			{
				state = Modes.Search;
			}
		}
	}

	public void Search(Manager.Sides searchEneminesOf)
	{
		currentSide = searchEneminesOf;
		state = Modes.Search;
	}

	void ClosestTargets()
	{
		foreach(VehicleController veh in M.VehiclesRegistry)
		{
			if(veh.Hull.Side!=currentSide)
			{
				HullController hull = veh.GetComponent<HullController>();
				if(hull!=null && Vector3.Distance(transform.position,hull.transform.position)<Range)
				{
					targetsList.Add(hull);
				}
			}
		}
	}

	#region IStorable implementation

	public void Save (WriterEx b)
	{
		b.WriteEnum(state);
		b.Write((double)delay);
	}

	public void Load (Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));
		delay = (float)r.ReadDouble();
	}

	#endregion
}
