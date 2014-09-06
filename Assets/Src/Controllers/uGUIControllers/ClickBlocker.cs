using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class ClickBlocker : BaseManagedController {


	bool blocked = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerEnter()
	{
		Block();
		//Debug.Log("Pointer enter");
	}

	public void OnPointerLeave()
	{
		Unblock();
		//Debug.Log("Pointer leave");
	}

	void OnDisable()
	{
		Unblock();
	}

	void Block()
	{
		if(blocked==false)
		{
			M.BlockMouseInput = true;
			blocked = true;
		}
	}

	void Unblock()
	{
		if(blocked==true)
		{
			M.BlockMouseInput = false;
			blocked = false;
		}
	}
}
