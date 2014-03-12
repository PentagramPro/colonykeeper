using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

public class Manager : MonoBehaviour {

	public enum Sides{
		Player,Aliens
	}
	public delegate void UpdatedDelegate();

	public TerrainController terrainController;
	public IInventory cratePrefab;

	public DateTime GameDateTime;
	Calendar calendar = CultureInfo.InvariantCulture.Calendar;
	float hourCounter = 0;

	GUIController guiController;

	public Stats Stat;

	//Object that stores GUI controller
	public GameObject gui;

	//Game dictionary contains prototypes for blocks and items
	// loaded from resources
	public GameDictionary GameD;

	public DefenceController defenceController;

	//Job manager holds list of jobs
	// has to be stored and loaded
	public JobManager JobManager ;

	//has to be stored and loaded
	[NonSerialized]
	public DictionaryEx<BlockController,BuildingController> BuildingsRegistry = new DictionaryEx<BlockController, BuildingController>();

	//has to be stored and loaded
	[NonSerialized]
	public List<VehicleController> VehiclesRegistry = new List<VehicleController>();

	[NonSerialized]
	public List<CrateController> CratesRegistry = new List<CrateController>();

	public CameraController cameraController;

	//cache for references to objects. Used during loading of game
	[NonSerialized]
	public Dictionary<int, object> LoadedLinks = new Dictionary<int, object>();

	public GUIController GetGUIController()
	{
		if(guiController==null)
			guiController = gui.GetComponent<GUIController>();
		return guiController;
	}
	// Use this for initialization
	void Start () {
		GameDateTime = new DateTime(2260,2,1);
		JobManager = new JobManager(this);

		if(terrainController==null)
			throw new UnityException("terrainController must not be null");
		if(Stat==null)
			throw new UnityException("Stat must not be null");
		if(cratePrefab==null)
			throw new UnityException("Crate prefab must not be null");
	}

	void Update()
	{

		hourCounter+=Time.smoothDeltaTime;
		if(hourCounter>1)
		{
			hourCounter--;
			//calendar.AddMinutes(GameDateTime,1);
			GameDateTime = GameDateTime.AddMinutes(1);
		}
		JobManager.UpdateJobs();
	}

	public void LoadResources()
	{
		string path = Path.Combine(Application.dataPath, "Resources/gamedictionary.xml");
		Debug.Log("XML path: "+path);
		GameD = GameDictionary.Load(path);
	// 	GameD.Save(path);
	}

	public IInventory FindInventoryFor(Item itemType)
	{
		foreach (BlockController b in BuildingsRegistry.Keys) 
		{
			BuildingController building = BuildingsRegistry[b];
			
			IInventory i = building.GetComponent<IInventory>();
			if(i==null)
				continue;
			
			
			
			if(i.CanPut(itemType)>0)
				return i;
			
		}
		return null;
	}

	public IInventory CreateCrate(Transform pos)
	{
		GameObject crate = (GameObject)Instantiate(cratePrefab.gameObject, pos.position, pos.rotation);
		IInventory crateInv = crate.GetComponent<IInventory>();
		CrateController crateController = crate.GetComponent<CrateController>();

		CratesRegistry.Add(crateController);
		return crateInv;

	}
	public void UnderAttack(HullController victim, Transform attacker)
	{
		if(defenceController!=null)
		{
			defenceController.UnderAttack(victim,attacker);
		}
	}
	public void SaveGame()
	{

		using(WriterEx b = new WriterEx(File.Open("savegame",FileMode.Create)))
		{


			terrainController.SaveUid(b);

			JobManager.SaveUid(b);

			b.Write(VehiclesRegistry.Count);
			foreach(VehicleController d in VehiclesRegistry)
			{
				b.WriteEx(d.Prototype);
				d.SaveUid(b);
			}

			terrainController.Save(b);

			b.Write(BuildingsRegistry.Count);
			foreach(BlockController bc in BuildingsRegistry.Keys)
			{
				BuildingController savingBuilding = BuildingsRegistry[bc];



				b.Write(bc.GetUID());
				b.Write(savingBuilding.Prototype.Name);
				savingBuilding.SaveUid(b);

				savingBuilding.Save(b);


			}

			JobManager.Save(b);
			b.WriteMagic();

			foreach(VehicleController v in VehiclesRegistry)
				v.Save(b);

			b.Write(GameDateTime);


		}
	}

	public void LoadGame()
	{
		LoadedLinks.Clear();
		using(ReaderEx b = new ReaderEx(File.Open("savegame",FileMode.Open)))
		{
			//cleanup
			foreach(BuildingController building in BuildingsRegistry.Values)
			{
				GameObject.Destroy(building.gameObject);
			}

			foreach(VehicleController vehicle in VehiclesRegistry)
			{
				GameObject.Destroy(vehicle.gameObject);
			}
			VehiclesRegistry.Clear();
			BuildingsRegistry.Clear();


			terrainController.LoadUid(this,b);

			JobManager.LoadUid(this,b);
		

			int count = b.ReadInt32();
			for(int i=0;i<count;i++)
			{
				Vehicle vehicleProt = b.ReadVehicle(this);
				VehicleController vehicleController = vehicleProt.Instantiate().GetComponent<VehicleController>();
				vehicleController.LoadUid(this,b);
				VehiclesRegistry.Add(vehicleController);
			}

			terrainController.Load(this, b);

			int buildingsCount = b.ReadInt32();
			for(int i=0;i<buildingsCount;i++)
			{
				BlockController bc = (BlockController)LoadedLinks[b.ReadInt32()];
				Building building = GameD.BuildingsByName [b.ReadString()];
				BuildingController loadedBuilding = building.Instantiate().GetComponent<BuildingController>();
				bc.BuildOn(loadedBuilding);


				loadedBuilding.LoadUid(this,b);

				loadedBuilding.Load(this,b);
			}

			JobManager.Load(this,b);
			b.CheckMagic();

			foreach(VehicleController v in VehiclesRegistry)
				v.Load(this,b);

			GameDateTime = b.ReadDateTime();
		}
	}
}
