using UnityEngine;
using System.Collections;

public class MoveToTargetController : BaseController {

    public Transform Target;
    DroneController droneController;
	// Use this for initialization
	void Start () {
        droneController = GetComponent<DroneController>();
	}
	
	// Update is called once per frame
	void Update () {
        if (droneController != null && Target!=null)
        {
            if (droneController.State == DroneController.Modes.Idle)
            {
                droneController.DriveTo(Target.position);
                Target = null;
            }
        }
	}
}
