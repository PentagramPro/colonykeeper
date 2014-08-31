using UnityEngine;
using System.Collections;

public class PointerArrowController : MonoBehaviour {

	public float Lifetime = 4f;
	public float Period = 2f;
	public float Amplitude = 0.4f;
	public Transform Arrow;

	float time = 0;
	Vector3 startPos;
	void Awake()
	{
		startPos = Arrow.localPosition;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Arrow.localPosition = startPos+new Vector3( 0,-Amplitude*Mathf.Sin(Period*time),0);
		time+=Time.smoothDeltaTime;
		if(time>Lifetime)
			Destroy(gameObject);
	}
}
