using UnityEngine;
using System.Collections;

public class RadarMarkerController : MonoBehaviour {

	Transform target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(target!=null)
		{
			transform.position = new Vector3(target.position.x,1.2f,target.position.z);
		}
	}

	public void SetTarget(Transform t)
	{
		target = t;
	}
}
