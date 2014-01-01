using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

	GUIController guiController;
	public GameObject gui;
	public GameDictionary GameD;

	public GUIController GetGUIController()
	{
		if(guiController==null)
			guiController = gui.GetComponent<GUIController>();
		return guiController;
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
