using UnityEngine;
using System.Collections;

[RequireComponent (typeof (ScriptNodeController))]
public class SAGoal : MonoBehaviour, IScriptAction {

	public GoalController Goal;
	public GoalState SetGoalStateTo = GoalState.Enabled;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IScriptAction implementation

	public void Execute ()
	{
		if(Goal!=null)
		{
			Goal.State = SetGoalStateTo;
		}
	}

	#endregion
}
