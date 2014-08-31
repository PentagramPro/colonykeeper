using UnityEngine;
using System.Collections;

public class ScriptNodeController : BaseManagedController {
	enum Modes{
		Idle, Countdown, Executed, End
	}

	public ScriptConditions Condition;
	public float Delay = 0;
	Modes state = Modes.Idle;

	float counter = 0;

	public BlockController LinkedBlock;

	public bool Executed{
		get{
			return state==Modes.Executed;
		}
	}
	// Use this for initialization
	void Start () {
		if(Condition==ScriptConditions.Mined && LinkedBlock!=null)
			LinkedBlock.OnMined+=OnMined;
	}
	
	// Update is called once per frame
	void Update () {
		if(state==Modes.Countdown)
		{
			counter+=Time.smoothDeltaTime;
			if(counter>Delay)
			{
				state = Modes.Executed;
				InternalExecute();
			}
		}
	}

	void OnApplicationQuit()
	{
		state = Modes.End;
	}

	void OnDestroy()
	{
		if(state==Modes.Idle && Condition==ScriptConditions.Destroyed)
		{
			ExecuteAction();
		}
	}

	void OnMined()
	{

		if(state== Modes.Idle && Condition==ScriptConditions.Mined)
			ExecuteAction();
	}

	public void ExecuteAction()
	{
		if(state==Modes.End || state==Modes.Executed)
			return;
		if(Delay==0)
		{
			state = Modes.Executed;

			InternalExecute();
		}
		else
		{
			state = Modes.Countdown;
		}
	}

	void InternalExecute()
	{
		Component[] components = GetComponents<Component>();
		foreach(Component c in components)
		{
			if(c is IScriptAction)
			{
				(c as IScriptAction).Execute();
			}
		}
		
		gameObject.SetActive(false);
		M.Script.ExecuteSequence();
	}
}
