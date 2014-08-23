using UnityEngine;
using System.Collections.Generic;

public class ScriptManager : BaseManagedController {

	ScriptNodeController[] scripts;

	int index = 0;
	// Use this for initialization
	void Start () {
		scripts = GetComponentsInChildren<ScriptNodeController>();
		//if(scripts.Length==0)
		ExecuteSequence(false);


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTipClosed()
	{
		ScriptNodeController s = scripts[index];
		if(s.Condition==ScriptConditions.TipClosed)
		{
			ExecuteSequence(true);
		}
	}

	void ExecuteSequence(bool includeFirst)
	{
		if(index>=scripts.Length)
			return;

		if(includeFirst)
		{
			scripts[index].ExecuteAction();
			index++;
		}

		for(;index<scripts.Length;index++)
		{
			ScriptNodeController s = scripts[index];
			if(s.Condition!=ScriptConditions.Sequence && s.Executed==false)
				break;
			s.ExecuteAction();
		}
	}
}
