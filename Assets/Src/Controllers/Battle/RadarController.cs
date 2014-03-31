using UnityEngine;
using System.Collections;

public class RadarController : BaseManagedController, IInteractive {


	public float Range = 5;
	public Projector RangeIndicator;
	// Use this for initialization
	void Start () {
		RangeIndicator.enabled = false;
		RangeIndicator.orthoGraphicSize = Range;
		M.defenceController.Radars.Add(this);
	}

	void OnDestroy()
	{
		M.defenceController.Radars.Remove(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IInteractive implementation

	public void OnDrawSelectionGUI()
	{

	}

	public void OnSelected()
	{
		RangeIndicator.enabled = true;
	}

	public void OnDeselected()
	{
		RangeIndicator.enabled = false;
	}

	#endregion
}
