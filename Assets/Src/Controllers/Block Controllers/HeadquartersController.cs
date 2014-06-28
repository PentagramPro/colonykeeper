using UnityEngine;
using System.Collections;

public class HeadquartersController : BaseManagedController, ICustomer, IStorable, IInteractive {

	public IInventory ColonyWaterInventory;
	public IInventory ColonyStorageInventory;

	public string waterItemName;
	public float waterConsumption=0.04f;
	float waterConsumptionPoints=0;

	public int initialWater = 200;

	private BuildingController buildingController;

	Item waterItem;
	SupplyJob waterSupply;
	// Use this for initialization
	void Start () {
		M.cameraController.ShowPoint(transform.position);

		if(ColonyWaterInventory==null)
			throw new UnityException("ColonyInventory must not be null");

		if(string.IsNullOrEmpty(waterItemName))
			throw new UnityException("Fill Water Item Name proterty!");

		if(!M.GameD.Items.ContainsKey(waterItemName))
			throw new UnityException(waterItemName+" not found in items dictionary!");


		foreach(PileXML item in M.GameD.StartItemsList)
		{
			ColonyStorageInventory.Put(M.GameD.Items[item.Name],item.Quantity);
		}

		waterItem = M.GameD.Items [waterItemName];
		ColonyWaterInventory.Put(waterItem,initialWater);

		buildingController = GetComponent<BuildingController>();
		//M.BuildingsRegistry.Add(buildingController.nativeBlock,buildingController);
	}
	
	// Update is called once per frame
	void Update () {
	
		int take = 0;
		waterConsumptionPoints+=(waterConsumption * Time.smoothDeltaTime);
		if(waterConsumptionPoints>1)
		{
			take = (int)waterConsumptionPoints;
			waterConsumptionPoints=0;
		}

		int has = ColonyWaterInventory.GetItemQuantity(waterItem);
		if (has > 0)
		{
			if (has < 100 && waterSupply == null)
			{
				waterSupply = new SupplyJob(M.JobManager, this, GetComponent<BuildingController>(), ColonyWaterInventory, waterItem, 100);
				M.JobManager.AddJob(waterSupply, false);
			}
			if(take>0)
				ColonyWaterInventory.Take(waterItem, take);
		} else
		{

		}
	}

	#region IInteractive implementation
	public void OnSelected()
	{
		
	}
	
	public void OnDeselected()
	{
		
	}

	public void OnDrawSelectionGUI ()
	{
		GUILayout.Space(10);
		GUILayout.Label("Water left: "+ColonyWaterInventory.GetItemQuantity(waterItem));
		GUILayout.Label("Water request: "+waterConsumptionPoints.ToString("n2"));
	}

	#endregion

	#region ICustomer implementation

	public void JobCanceled(IJob job)
	{
		waterSupply = null;
	}
	public void JobCompleted(IJob job)
	{
		waterSupply = null;
	}

	#endregion

	#region IStorable implementation

	public void Save (WriterEx b)
	{
		b.WriteLink(waterSupply);
		b.Write((double)waterConsumptionPoints);
	}

	public void Load (Manager m, ReaderEx r)
	{
		waterSupply = (SupplyJob)r.ReadLink(m);
		waterConsumptionPoints = (float)r.ReadDouble();
	}

	#endregion
}
