using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PageListController : BaseController
{
	public delegate void ItemSelectedDelegate(IListItem Item);
	public event ItemSelectedDelegate OnItemSelected;

    //public ListItemAdapterController Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8;
	public Text PagesIndicator;

	public List<ListItemAdapterController> DisplaySlot = new List<ListItemAdapterController>();


    [System.NonSerialized]
    public List<IListItem> ItemsToDisplay = new List<IListItem>();

    [System.NonSerialized]
    public IListItem SelectedItem;

    bool firstTimeHack = false;

	public bool ConstantlyUpdate = false;
    int currentPos = 0;

	IListItemAdapter lastSlot = null;


	protected void Awake ()
	{
		//base.Awake ();
		foreach(IListItemAdapter i in DisplaySlot)
		{
			Button b = i.GetButton();
			if(b!=null)
				b.onClick.AddListener(OnAdapterClick);
		}
	}
    // Use this for initialization
    void Start()
    {
        
    }

    public void UpdateList()
    {

    
        DisplayFrom(currentPos);
    }


    // Update is called once per frame
    void Update()
    {
        if (!firstTimeHack || ConstantlyUpdate)
        {
			UpdateList();
            firstTimeHack = true;
        }
    }

    void DisplayFrom(int pos)
    {

        Debug.Log("DisplayFrom.... ItemsToDisplay.Count=" + ItemsToDisplay.Count+" pos="+pos);

        int start = pos, end = Mathf.Min(pos + DisplaySlot.Count, ItemsToDisplay.Count);
        Debug.Log("start = " + start + " end = " + end);
        for (int i = start; i < end; i++)
        {
            IListItemAdapter c = DisplaySlot[i - pos];
            IListItem b = ItemsToDisplay[i];

            c.SetListItem(b);

            c.Activate();
			if(SelectedItem!=null && b==SelectedItem)
				c.Select();
			else
				c.Deselect();
            Debug.Log("activating item "+i+" with name "+b.GetName());
        }

		lastSlot = null;
        start = end ;
        end = pos + DisplaySlot.Count;
        Debug.Log("start = " + start + " end = " + end);
        for (int i = start; i < end; i++)
        {
            DisplaySlot[i - pos].Deactivate();
        }

		if(PagesIndicator!=null)
		{
			int cur=0,total=0;
			if(DisplaySlot.Count>0)
			{
				cur = pos/DisplaySlot.Count+1;
				total = ItemsToDisplay.Count / DisplaySlot.Count+1;
			}
			if(total>0)
				PagesIndicator.text = cur+"/"+total;
			else
				PagesIndicator.text = "-/-";
		}

    }
    void OnEnable()
    {
        //M.BlockMouseInput = true;
        //if (ItemsToDisplay.Count > 0)
        //    DisplayFrom(currentPos);

    }


    void OnDisable()
    {
        //M.BlockMouseInput = false;

    }

    public void OnPrev()
    {
        currentPos -= DisplaySlot.Count;
        if (currentPos < 0)
            currentPos = 0;
        DisplayFrom(currentPos);
    }

    public void OnNext()
    {
        if (currentPos < ItemsToDisplay.Count - DisplaySlot.Count)
            currentPos += DisplaySlot.Count;
        DisplayFrom(currentPos);
    }


    void SelectBuilding(int index)
    {
        if (ItemsToDisplay.Count > currentPos + index)
        {
            SelectedItem = ItemsToDisplay[currentPos + index];
			if(OnItemSelected!=null)
				OnItemSelected(SelectedItem);
            Debug.Log("Selected " + SelectedItem.GetName());
			IListItemAdapter newSlot = DisplaySlot[index];
			if(newSlot!=lastSlot)
			{
				newSlot.Select();
				if(lastSlot!=null)
					lastSlot.Deselect();
				lastSlot = newSlot;
			}
        }
        else
            SelectedItem = null;
    }

	void OnAdapterClick(Button b)
	{
		int index = 0;
		foreach(IListItemAdapter i in DisplaySlot)
		{
			if(i.GetButton()==b)
				break;
			index++;
		}

		SelectBuilding(index);


	}

}
