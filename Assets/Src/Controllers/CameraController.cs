using UnityEngine;
using System.Collections;

public class CameraController : BaseController {
	public Vector3 targetPosition = new Vector3();
	public Rect bounds = new Rect();
	public float ScrollFactor = 0.3f;
//	float dragSpeed=5;
	// Use this for initialization
	void Start () {
	
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(targetPosition+transform.position,0.5f);
	}

	public void ShowPoint(Vector3 mapPoint)
	{
		transform.position = mapPoint-targetPosition;
	}


	public void Scroll(Vector2 delta)
	{


		transform.position+=new Vector3(delta.x,0,delta.y)*ScrollFactor;
	}
	// Update is called once per frame
	void Update () {


	}
}
