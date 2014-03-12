using UnityEngine;
using System.Collections;
using Pathfinding;

public class VehicleController : BaseManagedController, IStorable  {
	private enum VehicleModes
	{
		Idle,Calc,Turn,Follow,Destroyed
	}

	public Manager.Sides Side = Manager.Sides.Player;

	public Vehicle Prototype;

	Vector3 currentDestination = Vector3.zero;

	public float speed = 2;
	public float turnSpeed=140;

	Path path;
	Seeker seeker;
	int currentWaypoint;
	private VehicleModes vehicleState = VehicleModes.Idle;


	public delegate void PathWalked();
	public event PathWalked OnPathWalked;

	bool stopping = false;
	float distanceToStop = 0;
	float distanceToStopWalked=0;
	
	void Start()
	{

		seeker = GetComponent<Seeker>();
	}

	
	// Update is called once per frame
	void Update () {
		if(vehicleState==VehicleModes.Follow || vehicleState==VehicleModes.Turn)
		{
			if (path == null) {
				//We have no path to move after yet
				throw new UnityException("No path!");
			}
			if (currentWaypoint >= path.vectorPath.Count) {
				vehicleState=VehicleModes.Idle;
				if(OnPathWalked!=null)
				{
					if(stopping)
						Debug.LogWarning("stopping==true while sending OnPathWalked event!");
					else
						OnPathWalked();
				}
				return;
			}
			
			Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
			dir.y=0;
			Quaternion dirRot = Quaternion.LookRotation(dir);

			if(Quaternion.Angle(transform.localRotation,dirRot)>10)
				vehicleState = VehicleModes.Turn;
			if(vehicleState==VehicleModes.Follow)
			{
				
				//Direction to the next waypoint
				
				dir *= speed * Time.smoothDeltaTime;
				
				
				transform.localRotation = dirRot;
				transform.position+=dir;
				//controller.SimpleMove (dir);
				//Check if we are close enough to the next waypoint
				//If we are, proceed to follow the next waypoint

				if(stopping)
				{
					distanceToStopWalked+=dir.magnitude;
					if(distanceToStopWalked>distanceToStop)
					{
						Stop ();
					}
				}

				if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < 0.25f) {
					currentWaypoint++;
					return;
				}


			}
			else if(vehicleState==VehicleModes.Turn)
			{
				
				transform.localRotation=Quaternion.RotateTowards(transform.localRotation,dirRot,turnSpeed*Time.smoothDeltaTime);
				
				
				if(transform.localRotation==dirRot)
					vehicleState=VehicleModes.Follow;
			}
		}
	}


	void OnDestroy()
	{
		vehicleState = VehicleModes.Destroyed;
		M.VehiclesRegistry.Remove(this);
	}



	public IInventory FindInventoryWith(Item itemType)
	{
		foreach (BlockController b in M.BuildingsRegistry.Keys) 
		{
			BuildingController building = M.BuildingsRegistry[b];
			
			IInventory i = building.GetComponent<IInventory>();
			if(i==null)
				continue;
			
			
			
			if(i.CanTake(itemType))
				return i;
			
		}
		return null;
	}
	public void Stop()
	{
		vehicleState = VehicleModes.Idle;
	}

	public void Stop(float distance)
	{
		if(vehicleState==VehicleModes.Follow || vehicleState==VehicleModes.Turn)
		{
			stopping = true;
			distanceToStopWalked=0;
			distanceToStop=distance;
		}
		else
			Stop ();
	}

	public void DriveTo(Vector3 dest)
	{
		stopping = false;
		currentDestination = dest;
		vehicleState = VehicleModes.Calc;

		seeker.StartPath (transform.position,dest, OnPathComplete);
	}

	private void OnPathComplete (Path p) {
		if (vehicleState == VehicleModes.Destroyed || vehicleState == VehicleModes.Idle)
			return;

		Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			path = p;
			currentWaypoint=1;
			vehicleState = VehicleModes.Turn;
		}
	}

	#region IStorable implementation
	public override void SaveUid (WriterEx b)
	{
		base.SaveUid (b);
		ComponentsSaveUid(b);
	}
	
	public override void LoadUid (Manager m, ReaderEx r)
	{
		base.LoadUid (m, r);
		ComponentsLoadUid(m,r);
	}
	public void Save (WriterEx b)
	{
		b.WriteMagic();
		b.Write(transform.position);
		b.Write(currentDestination);
		b.WriteEnum(vehicleState);
		b.Write((double)distanceToStop);
		b.Write((double)distanceToStopWalked);
		b.Write(stopping);
		ComponentsSave(b);
	}
	
	public void Load (Manager m, ReaderEx r)
	{
		r.CheckMagic();

		seeker = GetComponent<Seeker>();
		transform.position = r.ReadVector3();

		currentDestination = r.ReadVector3();


		vehicleState = (VehicleModes)r.ReadEnum(typeof(VehicleModes));
		distanceToStop = (float)r.ReadDouble();
		distanceToStopWalked = (float)r.ReadDouble();
		stopping  = r.ReadBoolean();
		ComponentsLoad(m,r);

		if(vehicleState!=VehicleModes.Idle)
		{
			vehicleState=VehicleModes.Idle;
			DriveTo(currentDestination);
		}
	}
	
	#endregion
}
