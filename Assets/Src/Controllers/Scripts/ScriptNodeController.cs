using UnityEngine;
using System.Collections;

public class ScriptNodeController : BaseManagedController {

	bool closing = false;
	bool executed = false;
	public ScriptConditions Condition;


	public BlockController LinkedBlock;

	public bool Executed{
		get{
			return executed;
		}
	}
	// Use this for initialization
	void Start () {
		if(Condition==ScriptConditions.Mined && LinkedBlock!=null)
			LinkedBlock.OnMined+=OnMined;
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

	void OnMined()
	{
		if(Condition==ScriptConditions.Mined)
			ExecuteAction();
	}

	public void ExecuteAction()
	{
		if(executed)
			return;
		executed = true;

		Component[] components = GetComponents<Component>();
		foreach(Component c in components)
		{
			if(c is IScriptAction)
			{
				(c as IScriptAction).Execute();
			}
		}

		gameObject.SetActive(false);
	}
}
