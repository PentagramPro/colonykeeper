using UnityEngine;
using System.Collections.Generic;
using TickedPriorityQueue;

public class TargeterController : BaseManagedController, IStorable {

	enum Modes {
		Idle,Search
	}
	Modes state = Modes.Idle;

	public float Range = 6;
	public delegate void TargetFoundDelegate(VisualContact contact);
	public event TargetFoundDelegate OnFound;

	TickedObject tickedObject;

	// at start()
	HullController self;

	// temp list, do not save
	List<HullController> targetsList = new List<HullController>();

	Manager.Sides currentSide;
	// Use this for initialization
	void Start () {
		self = GetComponent<HullController>();

		tickedObject = new TickedObject(OnUpdateTargets);
		tickedObject.TickLength = 1f;

		UnityTickedQueue.Instance.Add(tickedObject);
	}
	
	// Update is called once per frame
	void Update () {
	
		if(state==Modes.Search)
		{

			foreach(HullController hull in targetsList)
			{
				RaycastHit hit;

				if(hull==null)
					continue;

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


		}

	}

	private void OnUpdateTargets(object obj)
	{
		targetsList.Clear();
		Collider[] colliders = Physics.OverlapSphere(transform.position,Range);
		foreach(Collider c in colliders)
		{
			HullController hull = c.GetComponent<HullController>();
			if(hull==null || hull.Side==currentSide)
				continue;

			targetsList.Add(hull);
		}
	}

	public void Search(Manager.Sides searchEneminesOf)
	{
		currentSide = searchEneminesOf;
		state = Modes.Search;
	}




	void OnDestroy()
	{
		UnityTickedQueue.Instance.Remove(tickedObject);
	}

	#region IStorable implementation

	public void Save (WriterEx b)
	{
		b.WriteEnum(state);
	}

	public void Load (Manager m, ReaderEx r)
	{
		state = (Modes)r.ReadEnum(typeof(Modes));
	}

	#endregion
}
