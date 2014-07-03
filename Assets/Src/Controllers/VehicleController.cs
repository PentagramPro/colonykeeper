using UnityEngine;
using System.Collections;
using Pathfinding;
using System;
using UnitySteer;

[RequireComponent(typeof(HullController))]
public class VehicleController : BaseManagedController, IStorable  {
	private enum Modes
	{
		Idle,Calc,Follow,Destroyed
	}

	HullController hull;
	public HullController Hull{
		get{return hull;}
	}

	public string Name;
	public string LocalName
	{
		get
		{
			return Name;
		}
	}

	Vector3 currentDestination = Vector3.zero;

	SteerForPathSimplified steerForPath;
	public TickedVehicle avehicle;
	Seeker seeker;

	private Modes state = Modes.Idle;


	public delegate void PathWalked();
	public event PathWalked OnPathWalked;

	public delegate void Activated();
	public event Activated OnActivated;




	void Start()
	{
		seeker = GetComponent<Seeker>();
		steerForPath = GetComponent<SteerForPathSimplified>();


		hull = GetComponent<HullController>();

		M.PositionChanged(this);
		if(avehicle!=null)
			avehicle.CanMove = false;
		
		if(steerForPath!=null)
			steerForPath.OnArrival+=OnArrival;

		GetComponent<TapController>().OnTap+=OnTap;
		if(!M.VehiclesRegistry.Contains(this))
			M.VehiclesRegistry.Add(this);
	}

	public void Activate()
	{
		if(OnActivated!=null)
			OnActivated();
	}
	
	// Update is called once per frame
	void Update () {

		if(state==Modes.Follow)
		{

			M.PositionChanged(this);

		}

	}


	void OnDestroy()
	{
		if(avehicle!=null)
			avehicle.Stop();
		state = Modes.Destroyed;
		M.VehiclesRegistry.Remove(this);
		M.RemoveObjectFromCellCache(this);
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
		state = Modes.Idle;
		if(avehicle!=null)
			avehicle.Stop();
	}



	public void DriveTo(Vector3 dest)
	{
		if(avehicle==null)
			return;
		avehicle.CanMove = true;
		currentDestination = dest;
		state = Modes.Calc;

		seeker.StartPath (transform.position,dest, OnPathComplete);
	}

	private void OnPathComplete (Path p) {
		if (state == Modes.Destroyed || state == Modes.Idle)
			return;

		Debug.Log ("Yey, we got a path back. Did it have an error? "+p.error);
		if (!p.error) {
			Vector3Pathway path = new Vector3Pathway(p.vectorPath,1,false);
			steerForPath.Path = path;
			state = Modes.Follow;
		}
	}

	private void OnArrival(UnitySteer.Helpers.SteeringEvent<Vehicle> sender)
	{
		avehicle.Stop ();
		state = Modes.Idle;
		if(OnPathWalked!=null)
		{
			OnPathWalked();
		}

	}

	void OnTap()
	{
		M.GetGUIController().SelectedObject = gameObject;
		

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
		b.WriteEnum(state);
		ComponentsSave(b);
	}
	
	public void Load (Manager m, ReaderEx r)
	{
		r.CheckMagic();

		seeker = GetComponent<Seeker>();
		transform.position = r.ReadVector3();

		currentDestination = r.ReadVector3();


		state = (Modes)r.ReadEnum(typeof(Modes));

		ComponentsLoad(m,r);

		if(state!=Modes.Idle)
		{
			state=Modes.Idle;
			DriveTo(currentDestination);
		}
	}
	
	#endregion
}
