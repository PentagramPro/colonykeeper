using UnityEngine;
using System.Collections;

public class HullController : BaseManagedController, IStorable, IInteractive {

	public delegate void UnderAttack();
	public event UnderAttack OnUnderAttack;


	public bool ReportAttackToManager = false;

	public Vector3 CenterPos;
	public Vector3 Center
	{
		get{
			return transform.position+CenterPos;
		}
	}
	int maxHP = 1000;
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



	void OnTriggerEnter(Collider other)
	{

		ProjectileController proj = other.GetComponent<ProjectileController>();
		if (proj != null)
		{
			if(ReportAttackToManager)
				M.UnderAttack(this,other.transform);
			curHP-=(int)proj.Damage;
			if(curHP<=0)
				Destroy(gameObject);
			else
			{
				if(OnUnderAttack!=null)
					OnUnderAttack();
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
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IInteractive implementation

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
