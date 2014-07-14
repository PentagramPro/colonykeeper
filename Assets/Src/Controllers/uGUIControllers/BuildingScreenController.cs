using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(PageListController))]
public class BuildingScreenController : BaseManagedController {
    PageListController pageController;

    public Building SelectedBuilding
    {
        get {
            return pageController.SelectedItem as Building;
        }
    }
    
	// Use this for initialization
	void Start () {
        Debug.Log("BuildingScreenController.Start");
        pageController = GetComponent<PageListController>();
        foreach (Building b in M.GameD.Buildings)
        {
            if (b.Hide)
                continue;
            pageController.ItemsToDisplay.Add(b);
        }
        pageController.UpdateList();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable()
	{
		M.BlockMouseInput = true;
	}

	void OnDisable()
	{
		M.BlockMouseInput = false;
	}
    
}
