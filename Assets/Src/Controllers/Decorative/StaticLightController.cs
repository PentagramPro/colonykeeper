using UnityEngine;
using System.Collections;

public class StaticLightController: MonoBehaviour {

	public Color Color;
	public float Falloff = 3;
    public float Multiplier = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position,0.2f);
	}
}
