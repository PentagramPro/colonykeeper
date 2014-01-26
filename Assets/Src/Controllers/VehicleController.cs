using UnityEngine;
using System.Collections;
using Pathfinding;

public class VehicleController : BaseManagedController {
	private enum VehicleModes
	{
		Idle,Calc,Turn,Follow
	}

	public float speed = 2;
	public float turnSpeed=140;

	Path path;
	Seeker seeker;
	int currentWaypoint;
	private VehicleModes vehicleState = VehicleModes.Idle;
	private PathWalked OnPathWalked;

	public delegate void PathWalked();

	protected void Init()
	{
		seeker = GetComponent<Seeker>();
	}
	
	// Update is called once per frame
	protected void Update () {
		if(vehicleState==VehicleModes.Follow || vehicleState==VehicleModes.Turn)
		{
			if (path == null) {
				//We have no path to move after yet
				throw new UnityException("No path!");
			}
			if (currentWaypoint >= path.vectorPath.Count) {
				vehicleState=VehicleModes.Idle;
				OnPathWalked();
				return;
			}
			
			Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
			dir.y=0;
			Quaternion dirRot = Quaternion.LookRotation(dir);
			
			if(vehicleState==VehicleModes.Follow)
			{
				
				//Direction to the next waypoint
				
				dir *= speed * Time.smoothDeltaTime;
				
				
				transform.localRotation = dirRot;
				transform.position+=dir;
				//controller.SimpleMove (dir);
				//Check if we are close enough to the next waypoint
				//If we are, proceed to follow the next waypoint
				if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < 0.3f) {
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

	protected IInventory FindInventoryFor(Item itemType)
	{
		foreach (BlockController b in M.BuildingsRegistry.Keys) 
		{
			BuildingController building = M.BuildingsRegistry[b];
			
			IInventory i = building.GetComponent<IInventory>();
			if(i==null)
				continue;
			
	
			
			if(i.CanPut(itemType)>0)
				return i;
			
		}
		return null;
	}

	protected IInventory FindInventoryWith(Item itemType)
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

	protected void DriveTo(Vector3 dest, PathWalked onPathWalked)
	{
		vehicleState = VehicleModes.Calc;
		OnPathWalked = onPathWalked;
		seeker.StartPath (transform.position,dest, OnPathComplete);
	}

	private void OnPathComplete (Path p) {
		Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			path = p;
			currentWaypoint=1;
			vehicleState = VehicleModes.Turn;
		}
	}
}
