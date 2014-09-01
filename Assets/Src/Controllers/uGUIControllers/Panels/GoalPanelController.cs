using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Text;

public class GoalPanelController : BaseManagedController {

	public Text GoalText;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateGoals()
	{
		StringBuilder b = new StringBuilder();
		GoalController[] goals = M.GetComponentsInChildren<GoalController>();
		foreach(GoalController g in goals)
		{
			b.Append("- ");
			b.AppendLine(g.GoalText);
		}
		GoalText.text = b.ToString();
	}


}
