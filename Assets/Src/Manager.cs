using UnityEngine;
using System.Collections;
using System.IO;

public class Manager : MonoBehaviour {

	GUIController guiController;
	public GameObject gui;
	public GameDictionary GameD;
	public JobManager JobManager = new JobManager();

	public GUIController GetGUIController()
	{
		if(guiController==null)
			guiController = gui.GetComponent<GUIController>();
		return guiController;
	}
	// Use this for initialization
	void Start () {

	}

	public void LoadResources()
	{
		string path = Path.Combine(Application.dataPath, "Resources/gamedictionary.xml");
		Debug.Log("XML path: "+path);
		GameD = GameDictionary.Load(path);
	// 	GameD.Save(path);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
