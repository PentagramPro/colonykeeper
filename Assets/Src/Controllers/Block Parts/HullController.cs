using UnityEngine;
using System.Collections;

public class HullController : MonoBehaviour {


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


	// Use this for initialization
	void Start () {
		curHP = maxHP;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
