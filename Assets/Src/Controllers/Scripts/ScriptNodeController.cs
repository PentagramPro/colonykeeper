using UnityEngine;
using System.Collections.Generic;

public class ScriptNodeController : BaseManagedController {
	enum Modes{
		Idle, Countdown, Executed, End
	}


	public float Delay = 0;
	Modes state = Modes.Idle;
	List<IScriptCondition> conditions = new List<IScriptCondition>();
	List<IScriptAction> actions = new List<IScriptAction>();

	float counter = 0;


	public bool Executed{
		get{
			return state==Modes.Executed;
		}
	}

	public int ConditionsCount{
		get{
			return conditions.Count;
		}
	}
	protected override void Awake ()
	{
		base.Awake ();
		if(Delay==0)
			Delay = 0.05f;
		Component[] components = GetComponents<Component>();
		foreach(Component c in components)
		{
			if(c is IScriptAction)
				actions.Add(c as IScriptAction);
			if(c is IScriptCondition )
				conditions.Add(c as IScriptCondition);
		}
	}
	// Use this for initialization
	void Start () {


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

	void OnDisable()
	{
		state = Modes.End;
	}

	public void OnTipClosed()
	{
		foreach(IScriptCondition c in conditions)
			c.OnTipClosed();
	}

	public void OnConditionChecked()
	{
		if(state==Modes.End)
			return;

		bool allChecked=true;
		foreach(IScriptCondition c in conditions)
		{
			if(!c.IsChecked())
			{
				allChecked=false;
				break;
			}
		}

		if(allChecked)
		{

			ExecuteAction();
		}

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
		foreach(IScriptAction a in actions)
			a.Execute();


		M.Script.ExecuteSequence(this);
		gameObject.SetActive(false);
	}
}
