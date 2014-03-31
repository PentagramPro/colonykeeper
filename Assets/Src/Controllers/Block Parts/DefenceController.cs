using UnityEngine;
using System;
using System.Collections.Generic;

public class DefenceController : BaseManagedController, IInteractive {

	List<HullController> targets = new List<HullController>();
	HullController currentTarget = null;
	List<DefDroneController> currentDefenders = new List<DefDroneController>();
	public float Range=10;
	public Projector RangeIndicator;

	[NonSerialized]
	public List<RadarController> Radars = new List<RadarController>();

	// Use this for initialization
	void Start () {
		M.defenceController = this;
		RangeIndicator.orthoGraphicSize = Range;
		RangeIndicator.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	bool IsInRange(Vector3 point)
	{
		if (Vector3.Distance(transform.position, point) <= Range)
			return true;

		foreach (RadarController r in Radars)
		{
			if(Vector3.Distance(r.transform.position,point)<=r.Range)
				return true;
		}
		return false;
	}
	public void UnderAttack(HullController victim, Transform attacker)
	{
		ProjectileController proj = attacker.GetComponent<ProjectileController>();
		if(proj==null)
			return;


		HullController hull = proj.Owner;
		if(currentTarget==hull)
			return;
		if(hull.CurHP<=0)
			return;

		if (IsInRange(attacker.position))
		{

			M.DisplayMessage(M.S ["Message.Attack"], Color.red);



			if (currentTarget == null)
			{
				currentTarget = hull;
				AttackTarget(hull);
			}
			if (!targets.Contains(hull))
				targets.Add(hull);
		} else
		{
			M.DisplayMessage(M.S ["Message.AttackOutOfRange"], Color.red);
		}
	}


	void AttackTarget(HullController t)
	{
		currentDefenders.Clear();
		foreach(VehicleController v in M.VehiclesRegistry)
		{
			if(v.Side!=Manager.Sides.Player)
				continue;
			DefDroneController d = v.GetComponent<DefDroneController>();
			if(d!=null)
				currentDefenders.Add(d);
		}

		foreach(DefDroneController d in currentDefenders)
		{
			d.Attack(t);
		}
	}

	public void TargetDestroyed(HullController t)
	{
		if(!targets.Remove(t))
			return;

		if(t==currentTarget)
		{
			currentTarget = null;
			foreach(DefDroneController d in currentDefenders)
				d.Stop();
			currentDefenders.Clear();

			if(targets.Count>0)
			{
				currentTarget = targets[0];
				AttackTarget(currentTarget);
			}
		}
	}

	#region IInteractive implementation

	public void OnDrawSelectionGUI()
	{

	}

	public void OnSelected()
	{
		RangeIndicator.enabled = true;
	}

	public void OnDeselected()
	{
		RangeIndicator.enabled = false;
	}

	#endregion
}
