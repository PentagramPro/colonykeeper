using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TapController : BaseManagedController {

	public delegate void TapEvent();
	public event TapEvent OnTap;

	protected float panDistance = 10;
	protected float curPanDistance = 0;

	Vector3 lastMousePos;
	bool mouseDown = false;

	public void OnMouseDown()
	{

        if (M.BlockMouseInput)
            return;
		curPanDistance = 0;
		lastMousePos = Input.mousePosition;
		mouseDown = true;
	}
	
	public void OnMouseDrag()
	{
		if(!mouseDown)
			return;

        if (M.BlockMouseInput)
            return;

		Vector2 touchDeltaPosition = new Vector2(lastMousePos.x-Input.mousePosition.x,
		                                         lastMousePos.y-Input.mousePosition.y);
	
		curPanDistance += touchDeltaPosition.sqrMagnitude;
		if (curPanDistance > panDistance)
		{
			M.Scroll(touchDeltaPosition);
		}
		lastMousePos = Input.mousePosition;
	}
	
	public void OnMouseUp()
	{
        if (M.BlockMouseInput)
            return;

		if (curPanDistance < panDistance && OnTap!=null)
			OnTap();

		mouseDown = false;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
