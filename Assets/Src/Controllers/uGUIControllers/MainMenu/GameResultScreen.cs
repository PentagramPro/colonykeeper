using UnityEngine;
using System.Collections;

public class GameResultScreen : MonoBehaviour {

	public MainMenuController mainMenu;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public  void OnNext()
	{
		mainMenu.OnCloseGameResult();
	}
}
