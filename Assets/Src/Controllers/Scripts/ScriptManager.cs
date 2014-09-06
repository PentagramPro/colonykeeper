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

	public void ExecuteSequence(ScriptNodeController lastExecuted)
	{
		if(index>=scripts.Count)
			return;

		ScriptNodeController s = scripts[index];

		if(lastExecuted!=null)
		{
			if(!scripts.Contains(lastExecuted))
				return;
			


			if(s!=lastExecuted)
			{
				s = lastExecuted;
				index = scripts.IndexOf(s);
				Debug.LogWarning("A script node was executed not in time: "+s.gameObject.name);
			}
		}
		while(s.Executed)
		{
			index++;
			if(index>=scripts.Count)
				return;
			s = scripts[index];
		}

		if(s.ConditionsCount!=0)
			return;
		index++;
		s.ExecuteAction();

	}
}
