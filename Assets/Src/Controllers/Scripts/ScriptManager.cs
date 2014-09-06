using UnityEngine;
using System.Collections.Generic;

public class ScriptManager : BaseManagedController {

	//ScriptNodeController[] scripts;
	List<ScriptNodeController> scripts;

	int index = 0;
	protected override void Awake ()
	{
		base.Awake ();
		ScriptNodeController[] s = GetComponentsInChildren<ScriptNodeController>();
		scripts = new List<ScriptNodeController>(s);
	}
	// Use this for initialization
	void Start () {
		ExecuteSequence(null);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTipClosed()
	{

		if(index<scripts.Count)
		{

			ScriptNodeController s = scripts[index];
			s.OnTipClosed();

		}
	}

	public void ExecuteSequence(ScriptNodeController caller)
	{
		if(index>=scripts.Count)
			return;

		if(caller!=null && !scripts.Contains(caller))
			return;

		ScriptNodeController s = scripts[index];
		
		while(s.Executed)
		{
			index++;
			if(index>=scripts.Count)
				return;
			s = scripts[index];
		}

		if(caller!=null)
		{
			
			ScriptNodeController lastExecuted = scripts[index-1];

			if(lastExecuted!=caller)
			{

				index = scripts.IndexOf(caller);
				s = scripts[index];
				Debug.LogWarning("A script node was executed not in time: "+caller.gameObject.name);
			}
		}



		if(s.ConditionsCount!=0)
			return;
		index++;
		s.ExecuteAction();

	}
}
