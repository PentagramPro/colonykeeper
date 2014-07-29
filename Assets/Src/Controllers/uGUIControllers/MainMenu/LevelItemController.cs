using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelItemController : MonoBehaviour, IListItemAdapter {

	public Text LabelName,LabelDescription;

	Image bg;
	Color bgColor;

	void InitBg()
	{
		if(bg==null)
		{
			bg = GetComponent<Image>();
			bgColor = bg.color;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IListItemAdapter implementation

	public void SetListItem (IListItem item)
	{
		LabelName.text = item.GetName();
		LabelDescription.text = "";
		if(item is LevelDataController)
		{
			LevelDataController level = item as LevelDataController;
			LabelDescription.text = level.LevelDescription;
		}
	}

	public UnityEngine.UI.Button GetButton ()
	{
		return GetComponent<Button>();
	}

	public void Activate()
	{
		gameObject.SetActive(true);
	}
	
	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void Select ()
	{
		InitBg();
		bg.color = bg.color*2;
	}

	public void Deselect ()
	{
		InitBg();
		bg.color = bgColor;
	}

	#endregion
}
