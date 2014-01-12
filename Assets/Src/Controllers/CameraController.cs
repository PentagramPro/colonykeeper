using UnityEngine;
using System.Collections;

public class CameraController : BaseController {
	float scrollArea=60;
	float scrollSpeed=5;
//	float dragSpeed=5;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Transform myTransform = transform;

		var mPosX = Input.mousePosition.x;
		var mPosY = Input.mousePosition.y;
		
		// Do camera movement by mouse position
		if (mPosX < scrollArea) {myTransform.Translate(Vector3.right * -scrollSpeed * Time.deltaTime);}
		if (mPosX >= Screen.width-scrollArea) {myTransform.Translate(Vector3.right * scrollSpeed * Time.deltaTime);}
		if (mPosY < scrollArea) {myTransform.Translate(Vector3.forward * -scrollSpeed * Time.deltaTime);}
		if (mPosY >= Screen.height-scrollArea) {myTransform.Translate(Vector3.forward * scrollSpeed * Time.deltaTime);}
		
		/*// Do camera movement by keyboard
		myTransform.Translate(new Vector3(Input.GetAxis("EditorHorizontal") * scrollSpeed * Time.deltaTime,
		                              Input.GetAxis("EditorVertical") * scrollSpeed * Time.deltaTime, 0) );
		
		// Do camera movement by holding down option                 or middle mouse button and then moving mouse
		if ( (Input.GetKey("left alt") || Input.GetKey("right alt")) || Input.GetMouseButton(2) ) {
			myTransform.Translate(-new Vector3(Input.GetAxis("Mouse X")*dragSpeed, Input.GetAxis("Mouse Y")*dragSpeed, 0) );
		}*/
	}
}
