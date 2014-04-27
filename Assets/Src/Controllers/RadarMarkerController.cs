using UnityEngine;
using System.Collections;

public class RadarMarkerController : MonoBehaviour {

	HullController target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(target!=null)
		{
			transform.position = new Vector3(target.CenterPos.x,1.2f,target.CenterPos.z);
		}
	}

	public void SetTarget(HullController t)
	{
		target = t;
	}
}
