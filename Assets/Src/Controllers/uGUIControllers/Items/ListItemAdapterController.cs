﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ListItemAdapterController : MonoBehaviour, IListItemAdapter
{

    IListItemAdapter adapter;

	void InitAdapter()
	{
		Component[] components = GetComponents<Component>();
		foreach (Component c in components)
		{
			if (c is IListItemAdapter && c!=this)
			{
				adapter = c as IListItemAdapter;
				break;
			}
		}
	}
	void Awake()
	{
		InitAdapter();
	}
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetListItem(IListItem item)
    {
        if (adapter != null)
            adapter.SetListItem(item);
    }

    public void Activate()
    {
        if (adapter != null)
            adapter.Activate();
    }

    public void Deactivate()
    {
        if (adapter != null)
            adapter.Deactivate();
    }
	public void Select()
	{
		if (adapter != null)
			adapter.Select();
	}
	
	public void Deselect()
	{
		if (adapter != null)
			adapter.Deselect();
	}

	public Button GetButton()
	{
		if(adapter==null)
			InitAdapter();

		if(adapter!=null)
			return adapter.GetButton();
		return null;
	}
}
