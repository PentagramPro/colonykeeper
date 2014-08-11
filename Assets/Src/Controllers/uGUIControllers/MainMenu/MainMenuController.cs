using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {
	public enum LoadMainMenuState
	{
		DefeatScreen, VictoryScreen, MainMenu
	}
	public LevelsMenu levelsMenu;
	public GameResultScreen victoryScreen;
	public GameResultScreen defeatScreen;

	static LoadMainMenuState nextMenuState = LoadMainMenuState.MainMenu;

	// Use this for initialization
	void Start () {
		switch (nextMenuState)
		{
			case LoadMainMenuState.DefeatScreen:
				OnGameResult(false);
				break;
			case LoadMainMenuState.VictoryScreen:
				OnGameResult(true);
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnStartGame()
	{
		levelsMenu.gameObject.SetActive(true);
	}

	void OnGameResult(bool victory)
	{
		if(victory)
			victoryScreen.gameObject.SetActive(true);
		else
			defeatScreen.gameObject.SetActive(true);
	}

	public void OnCloseGameResult()
	{
		victoryScreen.gameObject.SetActive(false);
		defeatScreen.gameObject.SetActive(false);
	}

	public void OnCloseLevelsMenu()
	{
		levelsMenu.gameObject.SetActive(false);
	}


	public void OnExitGame()
	{
		Application.Quit();
	}

	public static void LoadMainMenu(LoadMainMenuState state)
	{
		nextMenuState = state;
		Application.LoadLevel("MainMenu");


	}
}
