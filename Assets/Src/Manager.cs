using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {

	GUIController guiController;
	public GameObject gui;


	public GUIController GetGUIController()
	{
		return guiController;
	}
	// Use this for initialization
	void Start () {
		guiController = gui.GetComponent<GUIController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
