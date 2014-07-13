using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HullPanelController : MonoBehaviour {

	public Text NameLabel;
	public ProgressBarController HpBar;

	HullController hull;
	public HullController HullToDisplay
	{
		set
		{
			hull = value;
			if(value!=null)
			{
				NameLabel.text = hull.LocalName;
			}
			else
			{
				NameLabel.text = "";
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(hull!=null)
		{
			HpBar.Progress = hull.RelativeHP;
		}
	}
}
