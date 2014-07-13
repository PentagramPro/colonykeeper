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
		Pile p = item as Pile;
		Name.text = p.GetName();
		Quantity.text = p.StringQuantity;
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
