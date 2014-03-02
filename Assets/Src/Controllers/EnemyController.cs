using UnityEngine;
using System.Collections;

public class EnemyController : BaseManagedController {

	enum Modes {
		Inactive,Sentry,Attack
	}
	Modes state = Modes.Inactive;

	public TargeterController targeter;
	public WeaponController weapon;
	// Use this for initialization
	void Start () {
		if(targeter==null)
			throw new UnityException("Targeter must not be null");

		if(weapon==null)
			throw new UnityException("Weapon must not be null");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
