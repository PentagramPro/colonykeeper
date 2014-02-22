using UnityEngine;
using System.Collections;

public class HeadquartersController : BaseManagedController, ICustomer, IStorable {

	public IInventory ColonyInventory;

	public string waterItemName;
	public float waterConsumption=1;

	Item waterItem;
	SupplyJob waterSupply;
	// Use this for initialization
	void Start () {
		M.cameraController.ShowPoint(transform.position);

		if(ColonyInventory==null)
			throw new UnityException("ColonyInventory must not be null");

		if(string.IsNullOrEmpty(waterItemName))
			throw new UnityException("Fill Water Item Name proterty!");

		if(!M.GameD.Items.ContainsKey(waterItemName))
			throw new UnityException(waterItemName+" not found in items dictionary!");

		waterItem = M.GameD.Items [waterItemName];
	}
	
	// Update is called once per frame
	void Update () {
	
		int take = (int)(waterConsumption * Time.smoothDeltaTime);
		int has = ColonyInventory.GetItemQuantity(waterItem);
		if (has > 0)
		{
			if (has < 100 && waterSupply == null)
			{
				waterSupply = new SupplyJob(M.JobManager, this, GetComponent<BuildingController>(), ColonyInventory, waterItem, 100);
				M.JobManager.AddJob(waterSupply, false);
			}
			ColonyInventory.Take(waterItem, take);
		} else
		{

		}
	}

	#region ICustomer implementation

	public void JobCompleted(IJob job)
	{
		waterSupply = null;
	}

	#endregion

	#region IStorable implementation

	public void Save (WriterEx b)
	{
		b.WriteLink(waterSupply);
	}

	public void Load (Manager m, ReaderEx r)
	{
		waterSupply = (SupplyJob)r.ReadLink(m);
	}

	#endregion
}
