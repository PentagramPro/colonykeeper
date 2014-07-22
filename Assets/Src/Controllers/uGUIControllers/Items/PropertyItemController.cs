using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PropertyItemController : BaseController, IListItemAdapter {

	public Text Label;
	public ProgressBarController Indicator;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IListItemAdapter implementation

	public void SetListItem (IListItem item)
	{
		Label.text = item.GetName();
		if(item is Field)
		{
			Field f = item as Field;
			Indicator.Progress = 1-1/(f.Value+1);
		}
	}

	public Button GetButton ()
	{
		return null;
	}

	public void Activate ()
	{
		gameObject.SetActive(true);
	}
	public void Deactivate ()
	{
		gameObject.SetActive(false);
	}
	public void Select ()
	{

	}

	public void Deselect ()
	{

	}

	#endregion
}
