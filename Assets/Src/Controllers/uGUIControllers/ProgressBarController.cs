using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour {

	public Image Background;
	public Image Bar;

	float progress=0.5f;
	public float Progress
	{
		get{
			return progress;
		}
		set{
			progress = Mathf.Clamp(value,0,1);
			Vector3 s = Bar.transform.localScale;
			Bar.transform.localScale = new Vector3(progress,s.y,s.z);

		}
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
