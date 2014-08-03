using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProductionPanelController : MonoBehaviour {

	public ProgressBarController ProgressBar;
	public Text Title;
	public Text Counter;
	public Text Status;
	public FurnaceController TargetFurnace;

	public float Progress
	{
		get
		{
			return ProgressBar.Progress;
		}
		set
		{
			ProgressBar.Progress = value;
		}
	}

	public string TitleText
	{
		get
		{
			return Title.text;
		}
		set
		{
			Title.text = value;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(TargetFurnace!=null)
		{
			Progress = TargetFurnace.Progress;
			TitleText = TargetFurnace.ProductionName;
			SetCounter(TargetFurnace.MaxTargetQuantity-TargetFurnace.TargetQuantity,TargetFurnace.MaxTargetQuantity);
			switch(TargetFurnace.State)
			{
			case FurnaceController.Modes.Fill:
				Status.text = "status: loading"; break;
			case FurnaceController.Modes.Prod:
				Status.text = "status: producing"; break;
			case FurnaceController.Modes.FreeIn:
				Status.text = "status: freeing input"; break;
			case FurnaceController.Modes.FreeOut:
				Status.text = "status: unloading"; break;
			default:
				Status.text = "status: idle"; break;
			}
		}
	}

	void OnCancel()
	{
		if(TargetFurnace!=null)
			TargetFurnace.Cancel();
	}

	void OnDisable()
	{
		TargetFurnace = null;
	}
	public void SetCounter(int cur, int total)
	{
		if(total>0)
			Counter.text = cur+"/"+total;
		else
			Counter.text = "-/-";
	}
}
