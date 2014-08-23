using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TipPanelController : BaseManagedController {

	public Text TipText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetStringName(string name)
	{
		TipText.text = M.S[name];
	}

	public void OnCancel()
	{
		M.GUIController.OnTipClose();
	}
}
