using UnityEngine;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;
using TickedPriorityQueue;

public class Manager : MonoBehaviour {

	public enum Sides{
		Player,Aliens
	}
	public delegate void UpdatedDelegate();

	public TerrainController terrainController;
	public IInventory cratePrefab;

	public DateTime GameDateTime;
	float hourCounter = 0;

	GUIController guiController;


	public Stats Stat;

	[NonSerialized]
	public Strings S;

	[NonSerialized]
	public Settings settings = new Settings();

	//Object that stores GUI controller
	public GameObject gui;

	//Game dictionary contains prototypes for blocks and items
	// loaded from resources
	[NonSerialized]
	public GameDictionary GameD;



	public TextAsset dataFile;
	public TextAsset stringsFile;

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

	void OnDestroy()
	{

	}
	public void LoadResources()
	{
		//string path = Path.Combine(Application.dataPath, "Resources/gamedictionary.xml");
		//Debug.Log("XML path: "+path);
		GameD = GameDictionary.Load(dataFile.text);

		//dataFile.
		//path = Path.Combine(Application.dataPath, "Resources/strings.xml");
		S = Strings.Load(stringsFile.text);
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
	public void DisplayMessage(string message)
	{
		GetGUIController().DisplayMessage(message,Vector3.zero,Color.white);
	}

	public void DisplayMessage(string message, Color color)
	{
		GetGUIController().DisplayMessage(message,Vector3.zero,color);
	}

	public void Scroll(Vector2 delta)
	{
		cameraController.Scroll(delta);
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

	public bool IsCellDiscovered(MapPoint point)
	{
		return terrainController.Map[point.X,point.Z].Discovered;
	}
	public void PositionChanged(VehicleController vehicle)
	{
		Vector3 loc = vehicle.transform.position-terrainController.transform.position;
		int x = (int)loc.x;
		int z = (int)loc.z;

		if( (vehicle.Hull.currentCell.X!=x || vehicle.Hull.currentCell.Z!=z) && terrainController.Map!=null)
		{

			BlockController cell = terrainController.Map[vehicle.Hull.currentCell.X,vehicle.Hull.currentCell.Z];
			if(cell!=null)
			{
				terrainController.Map[vehicle.Hull.currentCell.X,vehicle.Hull.currentCell.Z].ObjectsCache.Remove(vehicle);
				terrainController.Map[x,z].ObjectsCache.Add(vehicle);
				vehicle.Hull.currentCell = new MapPoint(x,z);
			}
		}
	}


	public void RemoveObjectFromCellCache(VehicleController vehicle)
	{
		BlockController cell = terrainController.Map[vehicle.Hull.currentCell.X,vehicle.Hull.currentCell.Z];
		if(cell!=null)
			cell.ObjectsCache.Remove(vehicle);
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
				//b.WriteEx(d.Prototype);
				d.SaveUid(b);
			}

			terrainController.Save(b);

			b.Write(BuildingsRegistry.Count);
			foreach(BlockController bc in BuildingsRegistry.Keys)
			{
				BuildingController savingBuilding = BuildingsRegistry[bc];



				b.Write(bc.GetUID());
				b.Write(savingBuilding.LocalName);
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
				VehicleProt vehicleProt = b.ReadVehicle(this);
				VehicleController vehicleController = vehicleProt.Instantiate(terrainController.transform)
					.GetComponent<VehicleController>();
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

	public VehicleController CreateVehicle(string name, Vector3 position)
	{
		GameObject veh = GameD.VehiclesByName[name].Instantiate(terrainController.transform);
		VehicleController vcontroller = veh.GetComponent<VehicleController>();
		VehiclesRegistry.Add(vcontroller);
		veh.transform.position = position;
		return vcontroller;
	}
}
