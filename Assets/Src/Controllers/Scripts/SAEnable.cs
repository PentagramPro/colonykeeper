using UnityEngine;
using System.Collections;

public class SAEnable : MonoBehaviour, IScriptAction {

	public GameObject ObjectToEnable;
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
			ObjectToEnable.SetActive(true);
	}

	#endregion
}
