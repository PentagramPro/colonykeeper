using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ScriptNodeController))]
public class SAEnable : MonoBehaviour, IScriptAction {

	public GameObject ObjectToEnable;
	public bool Enable = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IScriptAction implementation

	public void Execute ()
	{
		if(ObjectToEnable!=null)
			ObjectToEnable.SetActive(Enable);
	}

	#endregion
}
