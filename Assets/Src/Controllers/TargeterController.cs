using UnityEngine;
using System.Collections;

public class TargeterController : MonoBehaviour {

	enum Modes {
		Idle,Search
	}
	Modes state = Modes.Search;

	public delegate void TargetFoundDelegate();
	public event TargetFoundDelegate OnFound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(state==Modes.Search)
		{

		}
	}

	public void Search()
	{
		state = Modes.Search;
	}
}
