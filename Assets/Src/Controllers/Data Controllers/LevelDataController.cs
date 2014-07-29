using UnityEngine;
using System.Collections;

public class LevelDataController : MonoBehaviour , IListItem{

	public string LevelName;
	public string LevelDescription;
	public string SceneName;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#region IListItem implementation

	public string GetName ()
	{
		return LevelName;
	}

	#endregion
}
