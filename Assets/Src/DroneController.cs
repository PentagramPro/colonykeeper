﻿using UnityEngine;
using System.Collections;
using Pathfinding;

public class DroneController : BaseManagedController, IJobExecutor{
	enum Modes
	{
		Init,Idle,Calc,Turn,Follow,Work
	}

	public float speed = 2;
	public float turnSpeed=30;

	int currentWaypoint;
	//CharacterController controller;
	Path path;
	Seeker seeker;
	Modes state = Modes.Init;
	// Use this for initialization
	void Start () {
		seeker = GetComponent<Seeker>();
		//controller = GetComponent<CharacterController>();
	}

	Job digJob;
	
	// Update is called once per frame
	void Update () {
		if(state!=Modes.Idle)
		{
			switch(state)
			{
			case Modes.Init:
				M.JobManager.DigJobAdded+=OnDigJobAdded;
				state= Modes.Idle;
				break;
			case Modes.Follow:
				break;
			case Modes.Turn:

				break;
			case Modes.Calc:
				break;
			case Modes.Work:
				digJob.JobCell.Dig();
				M.JobManager.CompleteDigJob(digJob);
				Job j = M.JobManager.FindDigJob();
				if(j==null)
					state = Modes.Idle;
				else
					AssignJob(j);

				break;
			}
		}

		if(state==Modes.Follow || state==Modes.Turn)
		{
			if (path == null) {
				//We have no path to move after yet
				throw new UnityException("No path!");
				return;
			}
			if (currentWaypoint >= path.vectorPath.Count) {
				state=Modes.Work;
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

	public void FixedUpdate () {


	}

	public void OnPathComplete (Path p) {
		Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error && state==Modes.Calc) {
			path = p;
			currentWaypoint=1;
			state = Modes.Turn;
		}
	}

	void AssignJob(Job j)
	{
		state = Modes.Calc;
		M.JobManager.AssignDigJob(j,this);
		digJob=j;
		seeker.StartPath (transform.position,j.JobCell.Position, OnPathComplete);
	}
	void OnDigJobAdded(Job j)
	{
		if(j.Owner==null && state==Modes.Idle)
		{
			AssignJob(j);
		}
	}
	#region IJobExecutor implementation

	public void CancelJob ()
	{
		state = Modes.Idle;
	}

	#endregion
}
