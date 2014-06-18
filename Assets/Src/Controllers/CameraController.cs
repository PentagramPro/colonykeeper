using UnityEngine;
using System.Collections;

public class CameraController : BaseController {
	public delegate void ScrollDelegate(Vector3 delta);
	public event ScrollDelegate OnScrolled;

	public Vector3 targetPosition = new Vector3();
	public Rect bounds = new Rect();

	float ScrollFactor = 0.3f;
//	float dragSpeed=5;
	// Use this for initialization
	void Start () {
	
		ScrollFactor = Mathf.Abs(
			(Camera.main.ScreenToWorldPoint(new Vector3(0,0,transform.position.y))
		                -Camera.main.ScreenToWorldPoint(new Vector3(1,0,transform.position.y))).x
			);

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
		Vector3 vec = new Vector3(delta.x,0,delta.y);


		vec*=ScrollFactor;

		transform.position+=vec;
		if(OnScrolled!=null)
			OnScrolled(vec);

	}
	// Update is called once per frame
	void Update () {


	}
}
