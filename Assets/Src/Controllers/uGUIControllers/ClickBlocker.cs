using UnityEngine;
using System.Collections;

public class ClickBlocker : BaseManagedController {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnPointerEnter()
	{
		M.BlockMouseInput = true;
	}

	public void OnPointerLeave()
	{
		M.BlockMouseInput =false;
	}
}
