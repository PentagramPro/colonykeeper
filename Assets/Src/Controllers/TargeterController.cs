using UnityEngine;
using System.Collections;

public class TargeterController : BaseManagedController {

	enum Modes {
		Idle,Search,Delay
	}
	Modes state = Modes.Search;

	float delay = 0;
	public float Range = 6;
	public delegate void TargetFoundDelegate(HullController target);
	public event TargetFoundDelegate OnFound;

	Manager.Sides currentSide;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(state==Modes.Search)
		{
			foreach(VehicleController veh in M.VehiclesRegistry)
			{
				if(veh.Side!=currentSide)
				{
					HullController hull = veh.GetComponent<HullController>();
					if(hull!=null && Vector3.Distance(transform.position,hull.transform.position)<Range)
					{
						if(OnFound!=null)
							OnFound(hull);
						state = Modes.Idle;
						break;
					}
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
}
