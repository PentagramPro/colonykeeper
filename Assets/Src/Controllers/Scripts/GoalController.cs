using UnityEngine;
using System.Collections;

public class GoalController : BaseManagedController {

	public string GoalMessage;

	GoalState state = GoalState.Disabled;
	public GoalState State{
		get{return state;}
		set{
			state = value;
			switch(state)
			{
			case GoalState.Failed:
			case GoalState.Completed:
			case GoalState.Disabled:
				gameObject.SetActive(false);
				break;
			case GoalState.Enabled:
				gameObject.SetActive(true);
				break;
			}
		}
	}

	public string GoalText
	{
		get{
			return M.S[GoalMessage];
		}
	}
	// Use this for initialization
	void Start () {
	
	}

	void OnEnable()
	{
		M.GUIController.GoalPanel.UpdateGoals();
	}

	void OnDisable()
	{
		M.GUIController.GoalPanel.UpdateGoals();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
