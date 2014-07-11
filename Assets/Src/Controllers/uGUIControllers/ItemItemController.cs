using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemItemController : MonoBehaviour, IListItemAdapter {
    public Text localNameLabel;
	// Use this for initialization
	void Start () {
        localNameLabel = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetListItem(IListItem item)
    {
        localNameLabel.text = item.GetName();
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
