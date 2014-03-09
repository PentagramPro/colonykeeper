using UnityEngine;
using System;

public class VisualContact : IStorable
{
	enum Modes{
		Visible,Lost,Destroyed
	}

	UidContainer uidc;
	//store link
	public HullController Target = null;
	//store
	public Vector3 LastPosition;
	//store
	Modes state = Modes.Lost;

	public VisualContact ()
	{
		uidc = new UidContainer(this);
	}

	public VisualContact (HullController target)
	{
		Target = target;
		uidc = new UidContainer(this);
	}

	public void Update(Vector3 weaponPos)
	{
		if (Target.CurHP <= 0)
			state = Modes.Destroyed;
		if (state == Modes.Destroyed)
			return;
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
	public bool IsTargetDestroyed()
	{
		return state == Modes.Destroyed;
	}
	public bool IsTargetVisible()
	{
		return state==Modes.Visible;
	}

	public static bool IsVisible(HullController target, Vector3 source, float range)
	{
		RaycastHit hit;
		bool rayRes = Physics.Raycast(source,target.Center-source,out hit,range,0x00000700);

		return rayRes && hit.transform==target.transform;

	}

	#region IStorable implementation
	public void SaveUid (WriterEx b)
	{
		uidc.Save(b);
	}
	public void LoadUid (Manager m, ReaderEx r)
	{
		uidc.Load(m,r);
	}
	public void Save (WriterEx b)
	{
		b.WriteLink(Target);
		b.Write (LastPosition);
		b.WriteEnum(state);
	}
	public void Load (Manager m, ReaderEx r)
	{
		Target = (HullController)r.ReadLink(m);
		LastPosition = r.ReadVector3();
		state = (Modes)r.ReadEnum(typeof(Modes));

	}
	public int GetUID ()
	{
		return uidc.UID;
	}
	#endregion
}


