using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingItemController : MonoBehaviour, IListItemAdapter {

    public Text localNameLabel;
    public Text localDescrLabel;
    

 

	// Use this for initialization
	void Start () {
	
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
}
