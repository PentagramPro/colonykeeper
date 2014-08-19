using UnityEngine;
using System.Collections;

public class TestScript : BaseManagedController {

	// Use this for initialization
	void Start () {
		IInventory inv = GetComponent<IInventory>();
		if(inv!=null)
		{
			inv.Put(new Pile(M.GameD.Items["Iron Ore"],100));
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
