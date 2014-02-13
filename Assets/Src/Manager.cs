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
		using(BinaryWriter b = new BinaryWriter(File.Open("savegame",FileMode.Create)))
		{
			terrainController.Save(b);
		}
	}

	public void LoadGame()
	{
		using(BinaryReader b = new BinaryReader(File.Open("savegame",FileMode.Open)))
		{
			terrainController.Load(b);
		}
	}
}
