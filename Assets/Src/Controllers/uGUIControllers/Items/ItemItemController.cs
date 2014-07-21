using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemItemController : BaseController, IListItemAdapter {
    public Text localNameLabel;

	Color notSelected;
	Image parent;
	Button button;

	void Awake()
	{
		localNameLabel = GetComponentInChildren<Text>();
		button = GetComponent<Button>();
		
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
		if(item is Pile)
		{
			Pile p = item as Pile;
			localNameLabel.color = p.Properties.color/2 + new Color(0.5f,0.5f,0.5f,0.5f);
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
		parent.color = Color.green;
	}
	
	public void Deselect()
	{
		parent.color = notSelected;
	}

	public Button GetButton()
	{
		return button;
	}
}
