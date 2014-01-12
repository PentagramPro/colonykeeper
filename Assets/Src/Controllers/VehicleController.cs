using UnityEngine;
using System.Collections;
using Pathfinding;

public class VehicleController : BaseManagedController {
	private enum Modes
	{
		Idle,Calc,Turn,Follow
	}

	public float speed = 2;
	public float turnSpeed=140;

	Path path;
	Seeker seeker;
	int currentWaypoint;
	Modes state = Modes.Idle;
	private PathWalked OnPathWalked;

	public delegate void PathWalked();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(state==Modes.Follow || state==Modes.Turn)
		{
			if (path == null) {
				//We have no path to move after yet
				throw new UnityException("No path!");
			}
			if (currentWaypoint >= path.vectorPath.Count) {
				state=Modes.Idle;
				OnPathWalked();
				return;
			}
			
			Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
			dir.y=0;
			Quaternion dirRot = Quaternion.LookRotation(dir);
			
			if(state==Modes.Follow)
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
			else if(state==Modes.Turn)
			{
				
				transform.localRotation=Quaternion.RotateTowards(transform.localRotation,dirRot,turnSpeed*Time.smoothDeltaTime);
				
				
				if(transform.localRotation==dirRot)
					state=Modes.Follow;
			}
		}
	}

	protected void DriveTo(Vector3 dest, PathWalked onPathWalked)
	{
		state = Modes.Calc;
		OnPathWalked = onPathWalked;
		seeker.StartPath (transform.position,dest, OnPathComplete);
	}

	private void OnPathComplete (Path p) {
		Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			path = p;
			currentWaypoint=1;
			state = Modes.Turn;
		}
	}
}
