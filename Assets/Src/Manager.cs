using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class Manager : MonoBehaviour {

	public delegate void UpdatedDelegate();

	public TerrainController terrainController;

	GUIController guiController;

	//Object that stores GUI controller
	public GameObject gui;

	//Game dictionary contains prototypes for blocks and items
	// loaded from resources
	public GameDictionary GameD;

	//Job manager holds list of jobs
	// has to be stored and loaded
	public JobManager JobManager = new JobManager();

	//has to be stored and loaded
	public DictionaryEx<BlockController,BuildingController> BuildingsRegistry = new DictionaryEx<BlockController, BuildingController>();

	//has to be stored and loaded
	public List<VehicleController> DronesRegistry = new List<VehicleController>();


	public CameraController cameraController;

	//cache for references to objects. Used during loading of game
	public Dictionary<int, object> LoadedLinks = new Dictionary<int, object>();

	public GUIController GetGUIController()
	{
		if(guiController==null)
			guiController = gui.GetComponent<GUIController>();
		return guiController;
	}
	// Use this for initialization
	void Start () {
		if(terrainController==null)
			throw new UnityException("terrainController must not be null");
	}

	void Update()
	{
		JobManager.UpdateJobs();
	}

	public void LoadResources()
	{
		string path = Path.Combine(Application.dataPath, "Resources/gamedictionary.xml");
		Debug.Log("XML path: "+path);
		GameD = GameDictionary.Load(path);
	// 	GameD.Save(path);
	}
	
	public void SaveGame()
	{

		using(WriterEx b = new WriterEx(File.Open("savegame",FileMode.Create)))
		{


			terrainController.SaveUid(b);

			JobManager.SaveUid(b);

			b.Write(DronesRegistry.Count);
			foreach(DroneController d in DronesRegistry)
			{
				//d.SaveUid(b);
			}

			terrainController.Save(b);

			b.Write(BuildingsRegistry.Count);
			foreach(BlockController bc in BuildingsRegistry.Keys)
			{
				BuildingController savingBuilding = BuildingsRegistry[bc];



				b.Write(bc.GetUID());
				b.Write(savingBuilding.Prototype.Name);
				savingBuilding.Save(b);

				savingBuilding.SaveUid(b);
			}

			JobManager.Save(b);
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
			
			BuildingsRegistry.Clear();


			terrainController.LoadUid(this,b);

			JobManager.LoadUid(this,b);
		
			int count = b.ReadInt32();
			for(int i=0;i<count;i++)
			{
			}

			terrainController.Load(this, b);

			int buildingsCount = b.ReadInt32();
			for(int i=0;i<buildingsCount;i++)
			{
				BlockController bc = (BlockController)LoadedLinks[b.ReadInt32()];
				Building building = GameD.BuildingsByName [b.ReadString()];
				BuildingController loadedBuilding = building.Instantiate().GetComponent<BuildingController>();
				bc.BuildOn(loadedBuilding);
				loadedBuilding.Load(this,b);

				loadedBuilding.LoadUid(this,b);
			}

			JobManager.Load(this,b);
		}
	}
}
