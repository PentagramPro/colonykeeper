using UnityEngine;
using System.Collections.Generic;

public class ScriptManager : BaseManagedController {

	ScriptNodeController[] scripts;

	int index = 0;
	// Use this for initialization
	void Start () {
		scripts = GetComponentsInChildren<ScriptNodeController>();
		//if(scripts.Length==0)
		ExecuteSequence();


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTipClosed()
	{
		if(index<scripts.Length)
		{

			ScriptNodeController s = scripts[index];
			if(s.Condition==ScriptConditions.TipClosed)
			{
				s.ExecuteAction();
			}
		}
	}

	public void ExecuteSequence()
	{
		if(index>=scripts.Length)
			return;

	

		ScriptNodeController s = scripts[index];
		while(s.Executed)
		{
			index++;
			if(index>=scripts.Length)
				return;
			s = scripts[index];
		}

		if(s.Condition!=ScriptConditions.Sequence)
			return;
		index++;
		s.ExecuteAction();

	}
}
