using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PageListController : BaseManagedController
{
	public delegate void ItemSelectedDelegate(IListItem Item);
	public event ItemSelectedDelegate OnItemSelected;

    public ListItemAdapterController Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8;
	public Text PagesIndicator;

    List<IListItemAdapter> DisplaySlot = new List<IListItemAdapter>();

    [System.NonSerialized]
    public List<IListItem> ItemsToDisplay = new List<IListItem>();

    [System.NonSerialized]
    public IListItem SelectedItem;

    bool firstTimeHack = false;

    int currentPos = 0;

	IListItemAdapter lastSlot = null;

    // Use this for initialization
    void Start()
    {
        
    }

    public void UpdateList()
    {
        DisplaySlot.Clear();
        AddSlot(Slot1);
        AddSlot(Slot2);
        AddSlot(Slot3);
        AddSlot(Slot4);

        AddSlot(Slot5);
        AddSlot(Slot6);
        AddSlot(Slot7);
        AddSlot(Slot8);
    
        DisplayFrom(currentPos);
    }


    void AddSlot(Component s)
    {
        if (s != null)
        {
            if (s is IListItemAdapter)
            {
                
                DisplaySlot.Add(s as IListItemAdapter);

            }
            else
            {
                Debug.LogError("Not IListItemAdapter");
            }
        }
        
        
        
    }
    // Update is called once per frame
    void Update()
    {
        if (!firstTimeHack)
        {
            DisplayFrom(currentPos);
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
        if (ItemsToDisplay.Count > 0)
            DisplayFrom(currentPos);

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


    public void OnButton1()
    {
        Debug.Log("Button 1");
        SelectBuilding(0);
    }

    public void OnButton2()
    {
        SelectBuilding(1);
    }

    public void OnButton3()
    {
        SelectBuilding(2);
    }

    public void OnButton4()
    {
        SelectBuilding(3);
    }

    public void OnButton5()
    {
        SelectBuilding(4);
    }

    public void OnButton6()
    {
        SelectBuilding(5);
    }

    public void OnButton7()
    {
        SelectBuilding(6);
    }

    public void OnButton8()
    {
        SelectBuilding(7);
    }
}
