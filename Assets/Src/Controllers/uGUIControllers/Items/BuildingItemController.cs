using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingItemController : MonoBehaviour, IListItemAdapter {

    public Text localNameLabel;
    public Text localDescrLabel;
    
	Color notSelected;
	Image parent;


	// Use this for initialization
	void Start () {
		parent = GetComponent<Image>();

		notSelected = parent.color;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetListItem(IListItem item)
    {
        if (item != null)
        {
            localNameLabel.text = item.GetName();
            localDescrLabel.text = "";
        }
        else
        {
            localNameLabel.text = "";
            localDescrLabel.text = "";
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
		if(parent!=null)
			parent.color = notSelected;
	}

	public Button GetButton()
	{
		Button b = GetComponent<Button>();
		return b;
	}
}
