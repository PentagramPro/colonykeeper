using UnityEngine;
using System.Collections;

public class SiloController : StorageController {

	Vector3 minLevel,maxLevel;
	public Transform oreLevel;
	int oldQ=0;

	protected override void Awake ()
	{
		base.Awake ();
		minLevel = oreLevel.localPosition;
		maxLevel = minLevel+new Vector3(0,0.3f,0);
	}


	void Update()
	{
		if(oldQ!=Quantity)
		{
			float k = Quantity/(float)MaxQuantity;
			Vector3 level =  minLevel*(1-k)+maxLevel*k;
			oreLevel.localPosition = level;
			if(Quantity>0)
			{
				oreLevel.gameObject.renderer.material.color = FirstPile.Properties.color;
				oreLevel.gameObject.renderer.enabled = true;
			}
			else
			{
				oreLevel.gameObject.renderer.enabled = false;
			}

			oldQ = Quantity;
		}
	}

}
