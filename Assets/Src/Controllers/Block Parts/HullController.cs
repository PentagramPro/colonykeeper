using UnityEngine;
using System;
using System.Collections.Generic;

public class HullController : BaseManagedController, IStorable, IInteractive {

	public delegate void UnderAttack(Transform attacker);
	public event UnderAttack OnUnderAttack;

	List<IValueModifier> modifiers = new List<IValueModifier>();

	public bool ReportAttackToManager = false;

	public Manager.Sides Side = Manager.Sides.Player;
	
	[HideInInspector]
	public MapPoint currentCell;


	public String LocalName
	{
		get;set;
	}

	public Vector3 CenterPos;
	public Vector3 Center
	{
		get{
			return transform.position+CenterPos;
		}
	}
	public int maxHP = 1000;
	public int MaxHP{
		get{
			return maxHP;
		}
	}

	int curHP = 0;
	public int CurHP{
		get{
			return curHP;
		}
	}
	public float RelativeHP{
		get{
			return (float)maxHP/(float)curHP;
		}
	}

	bool selected = false;
	public bool IsSelected{
		get{
			return selected;
		}
	}

	void OnTriggerEnter(Collider other)
	{

		ProjectileController proj = other.GetComponent<ProjectileController>();
		if (proj != null)
		{
			if(ReportAttackToManager)
				M.UnderAttack(this,other.transform);

			int damage = (int)proj.Damage;
			foreach(IValueModifier m in modifiers)
				m.Modify(ref damage);
			curHP-=damage;

			if(curHP<=0)
				Destroy(gameObject);
			else
			{
				if(OnUnderAttack!=null)
					OnUnderAttack(other.transform);
			}
		}
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(Center,0.1f);
	}

	// Use this for initialization
	void Start () {
		curHP = maxHP;
		Component[] comps = GetComponents<Component>();
		foreach(Component c in comps)
		{
			if(c is IValueModifier)
				modifiers.Add(c as IValueModifier);
		}

		GetComponent<TapController>().OnTap+=OnTap;
	}

	void OnTap()
	{
		M.GetGUIController().SelectedObject = this;
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IInteractive implementation

	public void OnSelected()
	{
		selected=true;
	}
	
	public void OnDeselected()
	{
		selected=false;
	}

	public void OnDrawSelectionGUI ()
	{
		GUILayout.Label("HP: "+CurHP+"/"+MaxHP);
	}

	#endregion

	#region IStorable implementation

	public void Save (WriterEx b)
	{
		b.Write(curHP);
	}

	public void Load (Manager m, ReaderEx r)
	{
		curHP = r.ReadInt32();
	}

	#endregion
}
