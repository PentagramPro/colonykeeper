﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemItemController : BaseController, IListItemAdapter {
    public Text localNameLabel;

	Color notSelected;
	Image parent;

	void Awake()
	{
		localNameLabel = GetComponentInChildren<Text>();
		
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
		parent.color = Color.red;
	}
	
	public void Deselect()
	{
		parent.color = notSelected;
	}
}
