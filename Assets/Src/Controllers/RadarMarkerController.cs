using UnityEngine;
using System.Collections;

public class RadarMarkerController : BaseManagedController {

	HullController target;
	public Renderer markerRenderer;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(target!=null)
		{
			transform.position = new Vector3(target.Center.x,1.2f,target.Center.z);
			if(markerRenderer!=null)
				markerRenderer.enabled = !M.IsCellDiscovered(target.currentCell);
		}
	}

	public void SetTarget(HullController t)
	{
		target = t;
	}
}
