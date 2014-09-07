using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemItemController : BaseController, IListItemAdapter {
    public Text localNameLabel;
	public Text quantityLabel;

	Color notSelected;
	Image parent;


	void Awake()
	{
		//localNameLabel = GetComponentInChildren<Text>();

		
		parent = GetComponent<Image>();
		notSelected = parent.color;
	}
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetListItem(IListItem item)
    {
		if(localNameLabel!=null)
        	localNameLabel.text = item.GetName();
		if(item is CombinedPile)
		{
			CombinedPile cp = item as CombinedPile;

			localNameLabel.color = cp.FirstPile.Properties.color/2 + new Color(0.5f,0.5f,0.5f,0.5f);
			quantityLabel.text = cp.StringQuantity;
		}
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

	public void Select()
	{
		parent.color = new Color(0.1f,0.8f,0.2f);
	}
	
	public void Deselect()
	{
		parent.color = notSelected;
	}

	public Button GetButton()
	{
		return GetComponent<Button>();;
	}
}
