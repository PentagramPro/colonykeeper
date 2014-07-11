using UnityEngine;
using System.Collections;

public class ListItemAdapterController : MonoBehaviour, IListItemAdapter
{

    IListItemAdapter adapter;
	// Use this for initialization
	void Start () {
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
}
