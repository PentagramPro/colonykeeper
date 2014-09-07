using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StorageItemController : BaseController, IListItemAdapter {

	public Text Name;
	public Text Quantity;
	Image bg;
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
		if(item is CombinedPile)
		{
			CombinedPile cp = item as CombinedPile;
			Quantity.text = cp.StringQuantity;
			Name.color = cp.FirstPile.Properties.color*0.8f + new Color(0.2f,0.2f,0.2f,0.2f);
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
		if(bg==null)
			bg = GetComponent<Image>();

		bg.color = new Color(bg.color.r,bg.color.g,bg.color.b,0.4f);
	}
	public void Deselect ()
	{
		if(bg==null)
			bg = GetComponent<Image>();
		bg.color = new Color(bg.color.r,bg.color.g,bg.color.b,0);
	}

	public Button GetButton()
	{
		return GetComponent<Button>();
	}
	#endregion
}
