using UnityEngine;
using System.Collections;

public class HullController : MonoBehaviour {

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
			curHP-=(int)proj.Damage;
			if(curHP<=0)
				Destroy(gameObject);
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
}
