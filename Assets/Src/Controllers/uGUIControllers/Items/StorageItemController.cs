using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StorageItemController : BaseController, IListItemAdapter {

	public Text Name;
	public Text Quantity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	#region IListItemAdapter implementation
	public void SetListItem (IListItem item)
	{
		Name.text = item.GetName();
		if(item is Pile)
		{
			Pile p = item as Pile;
			Quantity.text = p.StringQuantity;
			Name.color = p.Properties.color*0.8f + new Color(0.2f,0.2f,0.2f,0.2f);
		}



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

	public Button GetButton()
	{
		return null;
	}
	#endregion
}
