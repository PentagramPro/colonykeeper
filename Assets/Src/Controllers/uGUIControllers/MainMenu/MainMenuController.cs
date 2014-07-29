using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {

	public LevelsMenu levelsMenu;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnStartGame()
	{
		levelsMenu.gameObject.SetActive(true);
	}

	public void OnCloseLevelsMenu()
	{
		levelsMenu.gameObject.SetActive(false);
	}

	public void OnExitGame()
	{
		Application.Quit();
	}
}
