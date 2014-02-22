using UnityEngine;
using System.Collections;

public class ConstructionController : BaseManagedController, IInteractive, IStorable {
	enum Modes{
		Start,Prebuild,Supply,Build,End
	}

	Modes state = Modes.Start;
	Building targetBuilding;
	GameObject targetGameObject;
	public BlockController ParentBlock;

	public GameObject TargetGameObject{
		set{
			targetGameObject = value;
			value.SetActive(false);
		}
	}
	float productionPoints;
	float productionRate = 0.3f;
	float prebuildRate = 0.6f;

	public SupplyController supplyController;
	// Use this for initialization
	void Start () {
		if(supplyController==null)
			throw new UnityException("Supply controller must not be null");
	}



	// Update is called once per frame
	void Update () {
		switch(state)
		{
		case Modes.Supply:
			if(supplyController.CheckSupply(targetBuilding.recipe,1)== SupplyController.SupplyStatus.Ready)
			{
				productionPoints=0;
				state = Modes.Build;
			}
			break;
		case Modes.Prebuild:
				productionPoints+=prebuildRate*Time.smoothDeltaTime;
				//transform.position+=new Vector3(0,prebuildRate*Time.smoothDeltaTime,0);
				if(productionPoints>=1)
				{
					productionPoints=0;
					state = Modes.Supply;

					supplyController.Supply(targetBuilding.recipe,1);
				}
			break;
		case Modes.Build:
			productionPoints+=productionRate*Time.smoothDeltaTime;
			if(productionPoints>=1)
			{
				state = Modes.End;

				M.BuildingsRegistry.Remove(ParentBlock);
				targetGameObject.SetActive(true);
				ParentBlock.BuildOn(targetGameObject.GetComponent<BuildingController>());

				GameObject.Destroy(transform.gameObject);
			}
			break;
		}
	}

	public void Construct(Building building)
	{
		targetBuilding = building;
		state = Modes.Prebuild;

		//because Unity won`t call Start method of supply controller in time
		//supplyController.Init();
		//transform.position -= new Vector3(0, 1, 0);

	}

	#region IInteractive implementation

	public void OnDrawSelectionGUI ()
	{
		GUILayout.Label(targetBuilding.Name);
		switch (state)
		{
			case Modes.Prebuild:
				GUILayout.Label("Preparing construction site.");
				GUILayout.Label("Complete: "+productionPoints.ToString("n2"));
				break;
			case Modes.Supply:
				GUILayout.Label("Waiting for supply...");
				break;
			case Modes.Build:
				GUILayout.Label("Building.");
				GUILayout.Label("Complete: "+productionPoints.ToString("n2"));
				break;
		}

	}

	#endregion

	#region IStorable implementation

	public void Save (WriterEx b)
	{
		b.WriteEnum(state);
		b.WriteEx(targetBuilding);
		b.WriteLink(ParentBlock);
		b.Write((double)productionPoints);

	

	}

	public void Load (Manager m, ReaderEx r)
	{
		if(targetGameObject!=null)
			GameObject.Destroy(targetGameObject);
		state = (Modes)r.ReadEnum(typeof(Modes));
		targetBuilding = r.ReadBuilding(m);
		ParentBlock = (BlockController)r.ReadLink(m);
		productionPoints = (float)r.ReadDouble();

		targetGameObject = targetBuilding.Instantiate();
		targetGameObject.SetActive(false);
	}

	#endregion
}
