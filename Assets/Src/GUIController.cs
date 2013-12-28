using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIController : BaseController {

	List<GameObject> blocks = new List<GameObject>();

	public delegate void PickedDelegate(GameObject pickedPrefab);

	public event PickedDelegate ItemPicked;
	// Use this for initialization
	void Start () {
		Object[] prefabs = Resources.LoadAll("Prefabs/Blocks");
		
		foreach(Object o in prefabs)
		{
			if(o.GetType()==typeof(GameObject))
			{
				BlockController bc = ((GameObject)o).GetComponent<BlockController>();
				if(bc!=null)
				{
					blocks.Add((GameObject)o);					
				}
			}
			Debug.Log("object! "+o.GetType()+" ");
		}
	}

	void OnGUI()
	{
		float pos=0,height=80;
		GameObject selected = null;

		foreach(GameObject b in blocks)
		{
			
			BlockController bc = b.GetComponent<BlockController>();
			
			if(GUI.Button(new Rect(0,pos,150,height),bc.Name))
			{
				selected = b;	
			}
			pos+=height;
		}
		if(selected!=null)
		{
			if(ItemPicked!=null)
				ItemPicked(selected);
	//		mode = Modes.Place;
	//		pickedItem = CreatePickedObject (selected);
			
		}
		
	}
	// Update is called once per frame
	void Update () {
	
	}
}
