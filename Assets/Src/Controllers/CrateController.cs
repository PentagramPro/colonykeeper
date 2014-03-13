using UnityEngine;
using System.Collections;

public class CrateController : BaseManagedController {


	public UnloadController Unloader;

	// Use this for initialization
	void Start () {
		if(Unloader==null)
			throw new UnityException("UnloadController must not be null");

		Unloader.OnFreed += OnFreed;
		Unloader.InventoryToUnload.ItemRemoved += OnItemRemoved;
		Unloader.FreeInventory();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDestroy()
	{
		Unloader.OnFreed -= OnFreed;
		Unloader.InventoryToUnload.ItemRemoved -= OnItemRemoved;
	}
	void OnItemRemoved()
	{
		if(Unloader.InventoryToUnload.Quantity==0)
			Destroy(gameObject);
	}

	void OnFreed()
	{
		Destroy(gameObject);
	}

	public void OnMouseUpAsButton()
	{
		M.GetGUIController().SelectedObject = gameObject;
		
		
	}
}
