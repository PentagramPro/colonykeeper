using UnityEngine;
using System.Collections;

public class ScriptNodeController : BaseManagedController {

	bool closing = false;
	public ScriptConditions Condition;
	public ScriptActions Action;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnApplicationQuit()
	{
		closing = true;
	}

	void OnDestroy()
	{
		if(!closing && Condition==ScriptConditions.Destroyed)
		{
			ExecuteAction();
		}
	}

	void ExecuteAction()
	{
		switch(Action)
		{
		case ScriptActions.Victory:
				M.FinishLevel(true);
			break;
		case ScriptActions.Defeat:
				M.FinishLevel(false);
			break;
		}
	}
}
