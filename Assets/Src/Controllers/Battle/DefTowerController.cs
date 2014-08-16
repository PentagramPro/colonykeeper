using UnityEngine;
using System.Collections;

public class DefTowerController : BaseManagedController, IInteractive {

	public TargeterController targeter;
	public WeaponController weapon;

	HullController hull;

	// Use this for initialization
	void Start () {
		if(targeter==null)
			throw new UnityException("Targeter must not be null");
		
		if(weapon==null)
			throw new UnityException("Weapon must not be null");

		targeter.OnFound += OnFound;

		weapon.OnTargetDestroyed += OnTargetDestroyed;
		weapon.OnTargetLost += OnTargetLost;

		hull = GetComponent<HullController>();

		targeter.Search(Manager.Sides.Player);
	}

	void OnFound(VisualContact contact)
	{
		weapon.Attack(hull, contact);
	}

	void OnTargetLost()
	{
		targeter.Search(Manager.Sides.Player);
	}

	void OnTargetDestroyed()
	{
		targeter.Search(Manager.Sides.Player);
	}
	// Update is called once per frame
	void Update () {
	
	}

	#region IInteractive implementation

	public void OnDrawSelectionGUI ()
	{

	}

	public void OnSelected ()
	{
		M.GUIController.WeaponPanel.TargetWeapon = weapon;
		M.GUIController.WeaponPanel.gameObject.SetActive(true);
	}

	public void OnDeselected ()
	{
		M.GUIController.WeaponPanel.gameObject.SetActive(false);
	}

	#endregion
}
