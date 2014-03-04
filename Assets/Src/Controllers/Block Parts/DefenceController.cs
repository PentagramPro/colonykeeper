using UnityEngine;
using System.Collections.Generic;

public class DefenceController : BaseManagedController {

	List<HullController> targets = new List<HullController>();
	HullController currentTarget = null;
	List<DefDroneController> currentDefenders = new List<DefDroneController>();

	// Use this for initialization
	void Start () {
		M.defenceController = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UnderAttack(HullController victim, Transform attacker)
	{
		HullController hull = attacker.GetComponent<HullController>();
		if(currentTarget==hull)
			return;

		if(currentTarget==null)
		{
			currentTarget = hull;
			AttackTarget(hull);
		}
		if(!targets.Contains(hull))
			targets.Add(hull);
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
}
