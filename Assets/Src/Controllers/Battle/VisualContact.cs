using UnityEngine;
using System;

public class VisualContact
{
	enum Modes{
		Visible,Lost
	}
	public HullController Target = null;
	public Vector3 LastPosition;
	Modes state = Modes.Lost;

	public VisualContact ()
	{
	}

	public VisualContact (HullController target)
	{
		Target = target;
	}

	public void Update(Vector3 weaponPos)
	{
		if(IsVisible(Target, weaponPos,6))
		{
			state = Modes.Visible;
			LastPosition = Target.transform.position;

		}
		else
		{
			state = Modes.Lost;
		}
	}

	public bool IsTargetVisible()
	{
		return state==Modes.Visible;
	}

	public static bool IsVisible(HullController target, Vector3 source, float range)
	{
		RaycastHit hit;
		return Physics.Raycast(source,target.Center-source,out hit,range) && hit.transform==target.transform;

	}

}


