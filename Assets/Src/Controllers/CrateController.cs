using UnityEngine;
using System.Collections;

public class CrateController : BaseManagedController {


	public UnloadController Unloader;

	// Use this for initialization
	void Start () {
		if(Unloader==null)
			throw new UnityException("Unload controller must not be null");

		Unloader.OnFreed += OnFreed;
		Unloader.FreeInventory();
	}
	
	// Update is called once per frame
	void Update () {
	
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
