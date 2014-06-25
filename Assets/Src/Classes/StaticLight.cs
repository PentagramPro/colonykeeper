using System;
using UnityEngine;

[Serializable]
public class StaticLight
{
	public Vector3 Position;
	public Color Col;
	public Component Owner;
	public float Falloff=3;

	public StaticLight()
	{

	}

	public StaticLight(Component owner,Vector3 pos, Color col)
	{
		Owner = owner;
		Position = pos;
		Col = col;
	}

	public Vector3 GlobalPosition
	{
		get
		{
			return Position+Owner.transform.position;
		}
	}

}


