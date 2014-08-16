using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WeaponPanelController: BaseManagedController {

	public WeaponController TargetWeapon;
	public Text DPSLabel;
	public ProgressBarController ReloadProgress;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(TargetWeapon!=null)
		{
			DPSLabel.text = TargetWeapon.DPS.ToString("0.00");
			ReloadProgress.Progress = TargetWeapon.ReloadProgress;
		}
	}

	void OnDisable()
	{
		TargetWeapon = null;
	}
}
