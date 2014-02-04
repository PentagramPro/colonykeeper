using UnityEngine;
using System.Collections;

public class ConstructionController : BaseManagedController, IInteractive {
	enum Modes{
		Start,Supply,Build,End
	}

	Modes state = Modes.Start;
	Building targetBuilding;
	GameObject targetGameObject;
	public GameObject TargetGameObject{
		set{
			targetGameObject = value;
			value.SetActive(false);
		}
	}
	float productionPoints;
	float productionRate = 5;

	public SupplyController supplyController;
	// Use this for initialization
	void Start () {
		if(supplyController==null)
			throw new UnityException("Supply controller must not be null");
	}

	public void OnMouseUpAsButton()
	{
		M.GetGUIController().SelectedObject = this;
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
		case Modes.Build:
			productionPoints+=productionRate*Time.smoothDeltaTime;
			if(productionPoints>=1)
			{
				state = Modes.End;
				
				targetGameObject.SetActive(true);
				
			}
			break;
		}
	}

	public void Construct(Building building)
	{
		targetBuilding = building;
		state = Modes.Supply;
		supplyController.Supply(building.recipe,1);
	}

	#region IInteractive implementation

	public void OnDrawSelectionGUI ()
	{
		if(state==Modes.Supply)
		{
			GUILayout.Label(targetBuilding.Name);
			GUILayout.Label("Waiting for supply...");
		}
		else if(state == Modes.Build)
		{
			GUILayout.Label(targetBuilding.Name);
			GUILayout.Label("Complete: "+productionPoints.ToString("n2"));
		}
	}

	#endregion
}
