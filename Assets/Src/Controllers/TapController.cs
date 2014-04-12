using UnityEngine;
using System.Collections;

public class TapController : BaseManagedController {

	public delegate void TapEvent();
	public event TapEvent OnTap;

	protected float panDistance = 10;
	protected float curPanDistance = 0;

	Vector3 lastMousePos;

	void OnMouseDown()
	{
		curPanDistance = 0;
		lastMousePos = Input.mousePosition;

	}
	
	void OnMouseDrag()
	{
		Vector2 touchDeltaPosition = new Vector2(lastMousePos.x-Input.mousePosition.x,
		                                         lastMousePos.y-Input.mousePosition.y);
	
		curPanDistance += touchDeltaPosition.sqrMagnitude;
		if (curPanDistance > panDistance)
		{
			M.Scroll(touchDeltaPosition);
		}
		lastMousePos = Input.mousePosition;
	}
	
	void OnMouseUp()
	{
		if (curPanDistance < panDistance && OnTap!=null)
			OnTap();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
