using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PageListController : BaseManagedController
{
    public ListItemAdapterController Slot1, Slot2, Slot3, Slot4, Slot5, Slot6, Slot7, Slot8;
    
    List<IListItemAdapter> BuildingSlots = new List<IListItemAdapter>();

    [System.NonSerialized]
    public List<IListItem> ItemsToDisplay = new List<IListItem>();

    [System.NonSerialized]
    public IListItem SelectedItem;

    bool firstTimeHack = false;

    int currentPos = 0;

    // Use this for initialization
    void Start()
    {
        
    }

    public void UpdateList()
    {
        BuildingSlots.Clear();
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
                if (s != null)
                    BuildingSlots.Add(s as IListItemAdapter);
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

        int start = pos, end = Mathf.Min(pos + BuildingSlots.Count, ItemsToDisplay.Count);
        Debug.Log("start = " + start + " end = " + end);
        for (int i = start; i < end; i++)
        {
            IListItemAdapter c = BuildingSlots[i - pos];
            IListItem b = ItemsToDisplay[i];

            c.SetListItem(b);

            c.Activate();
            Debug.Log("activating item "+i+" with name "+b.GetName());
        }

        start = end ;
        end = pos + BuildingSlots.Count;
        Debug.Log("start = " + start + " end = " + end);
        for (int i = start; i < end; i++)
        {
            BuildingSlots[i - pos].Deactivate();
        }



    }
    void OnEnable()
    {
        M.BlockMouseInput = true;
        if (ItemsToDisplay.Count > 0)
            DisplayFrom(currentPos);

    }


    void OnDisable()
    {
        M.BlockMouseInput = false;

    }

    public void OnPrev()
    {
        currentPos -= BuildingSlots.Count;
        if (currentPos < 0)
            currentPos = 0;
        DisplayFrom(currentPos);
    }

    public void OnNext()
    {
        if (currentPos < ItemsToDisplay.Count - BuildingSlots.Count)
            currentPos += BuildingSlots.Count;
        DisplayFrom(currentPos);
    }


    void SelectBuilding(int index)
    {
        if (ItemsToDisplay.Count > currentPos + index)
        {
            SelectedItem = ItemsToDisplay[currentPos + index];
            Debug.Log("Selected " + SelectedItem.GetName());
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
