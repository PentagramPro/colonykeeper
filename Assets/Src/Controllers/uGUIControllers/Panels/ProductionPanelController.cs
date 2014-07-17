using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProductionPanelController : MonoBehaviour {

	public ProgressBarController ProgressBar;
	public Text Title;
	public Text Counter;
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
		}
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
