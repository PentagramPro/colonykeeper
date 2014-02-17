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
	public GameDictionary GameD;

	//Job manager holds list of jobs
	public JobManager JobManager = new JobManager();

	public DictionaryEx<BlockController,BuildingController> BuildingsRegistry = new DictionaryEx<BlockController, BuildingController>();

	public CameraController cameraController;

	public Dictionary<int, Object> LoadedLinks = new Dictionary<int, Object>();

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
			terrainController.Save(b);

			b.Write(BuildingsRegistry.Count);
			foreach(BlockController bc in BuildingsRegistry.Keys)
			{
				b.Write(bc.UID);
				b.Write(BuildingsRegistry[bc].Prototype.Name);
				BuildingsRegistry[bc].Save(b);
			}
		}
	}

	public void LoadGame()
	{
		LoadedLinks.Clear();
		using(ReaderEx b = new ReaderEx(File.Open("savegame",FileMode.Open)))
		{
			foreach(BuildingController building in BuildingsRegistry.Values)
			{
				GameObject.Destroy(building.gameObject);
			}

			BuildingsRegistry.Clear();
			terrainController.Load(this, b);

			int count = b.ReadInt32();
			for(int i=0;i<count;i++)
			{
				BlockController bc = (BlockController)LoadedLinks[b.ReadInt32()];
				Building building = GameD.BuildingsByName [b.ReadString()];
				BuildingController loadedBuilding = building.Instantiate().GetComponent<BuildingController>();
				bc.BuildOn(loadedBuilding);
				loadedBuilding.Load(this,b);

			}
		}
	}
}
