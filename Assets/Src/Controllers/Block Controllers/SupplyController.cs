using UnityEngine;
using System.Collections;

public class SupplyController : BaseManagedController, ICustomer {

	enum Modes{
		Idle, Supply
	}

	Recipe targetRecipe;
	int quantity;

	public void Supply(Recipe recipe, int quantity)
	{
		this.targetRecipe = recipe;
		this.quantity = quantity;
	}

	public void Cancel()
	{

	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region ICustomer implementation


	public void JobCompleted (IJob job)
	{

	}


	#endregion

}
