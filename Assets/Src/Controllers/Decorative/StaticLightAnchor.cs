﻿using UnityEngine;
using System.Collections.Generic;

// this controller allows any object to reflect static lighting in a basic way
// used on drones
public class StaticLightAnchor : BaseManagedController {

	Renderer[] cache;
	Map map;
	public Vector3 AmbientLight = Vector3.zero;
	// Use this for initialization
	void Start () {
	
		cache = GetComponentsInChildren<Renderer>();
		map = M.terrainController.Map;
	}
	
	// Update is called once per frame
	void Update () {
	
		//Color c = Color.white*(map.GetLightAmount(transform.position)*0.8f+new Color(0.2f,0.2f,0.2f));
		Vector3 bigc = map.GetLightAmount(transform.position)+AmbientLight;
        //Vector3 c = 
        Color c = PanelGenerator.ClampLight(bigc);
        if(renderer!=null)
		    renderer.material.SetColor("_Color",c);
		foreach(Renderer r in cache)
			r.material.SetColor("_Color",c);
	}
}
