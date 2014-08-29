using UnityEngine;
using System.Collections;

public class SATip : BaseManagedController, IScriptAction {

	public string StringName = "";
	public Transform LinkedObject;

	public bool ChangeCameraLocation=false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IScriptAction implementation

	public void Execute ()
	{
		M.GUIController.ShowTip(StringName);
		if(LinkedObject!=null && ChangeCameraLocation)
		{
			M.cameraController.ShowPoint(LinkedObject.position);
		}
	}

	#endregion
}
