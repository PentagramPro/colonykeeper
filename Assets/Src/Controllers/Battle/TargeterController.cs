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

    UnityTickedQueue queueInstance;
	TickedObject tickedObject;
	Vector3 lastp1,lastp2,lastt;


	public Vector3 LocalTargetingPoint;
	Vector3 TargetingPoint{
		get{
			return LocalTargetingPoint+transform.position;
		}
	}
	// at start()
	HullController self;

	// temp list, do not save
	List<HullController> targetsList = new List<HullController>();

	Manager.Sides currentSide;

	void Awake()
	{
		self = GetComponent<HullController>();
		LocalTargetingPoint = self.CenterPos;
	}
	// Use this for initialization
	void Start () {


		tickedObject = new TickedObject(OnUpdateTargets);
		tickedObject.TickLength = 1f;
        //UnityTickedQueue.GetInstance("AI");
        queueInstance = UnityTickedQueue.Instance;
        queueInstance.Add(tickedObject);

        
	}
	
	// Update is called once per frame
	void Update () {
	
		if(state==Modes.Search)
		{

			foreach(HullController hull in targetsList)
			{
				RaycastHit hit;

				if(hull==null || hull==self)
					continue;

				Vector3 point1, point2, dir = hull.Center-TargetingPoint;

				if(CheckRay(TargetingPoint,dir,hull.transform))
				{

					CalculateLookAtPoints(TargetingPoint,0.1f,dir,out point1, out point2);
					lastp1=point1;lastp2=point2;lastt = hull.transform.position;
					if(
						CheckRay(point1,hull.Center-point1,hull.transform)
						&&
						CheckRay(point2,hull.Center-point2,hull.transform)
						)
					{
						if(OnFound!=null)
							OnFound(new VisualContact(hull));
						state = Modes.Idle;
						break;
					}
				}
			}


		}

	}

	bool CheckRay(Vector3 start, Vector3 dir, Transform target)
	{
		RaycastHit hit;
		if(!Physics.Raycast(start,dir,out hit,Range))
			return false;

		if(hit.transform!=target)
			return false;

		return true;
	}


	void CalculateLookAtPoints(Vector3 pos, float dist, Vector3 dir, out Vector3 point1, out Vector3 point2)
	{
		Vector3 normal = Vector3.Cross(dir,Vector3.up).normalized*dist;
		point1 = pos+normal;
		point2 = pos-normal;
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

	void OnDrawGizmos()
	{
		Gizmos.DrawLine(lastp1,lastt);
		Gizmos.DrawLine(lastp2,lastt);
	}


	void OnDestroy()
	{
        queueInstance.Remove(tickedObject);
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
