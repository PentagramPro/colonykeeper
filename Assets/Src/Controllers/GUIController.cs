using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIController : BaseManagedController {

	//List<Block> blocks = new List<Block>();


	public delegate void PickedDelegate(GameObject pickedPrefab);

	public event PickedDelegate ItemPicked;

	public IInteractive SelectedObject;

	public float LeftPanelW = 150;
	public float RightPanelW = 150;

	// Use this for initialization
	void Start () {
		/*Object[] prefabs = Resources.LoadAll("Prefabs/Blocks");


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
		}*/
	}

	void OnGUI()
	{
		float pos=0,height=80;
		Building selected = null;

		foreach(Building b in M.GameD.Buildings)
		{
			if(GUI.Button(new Rect(0,pos,150,height),b.Name))
			{
				selected = b;	
				
			}
			pos+=height;
		}


		if(selected!=null)
		{
			GameObject obj = selected.Instantiate();
			if(ItemPicked!=null)
				ItemPicked(obj);
	//		mode = Modes.Place;
	//		pickedItem = CreatePickedObject (selected);
			
		}

		if (SelectedObject != null)
		{
			GUILayout.BeginArea(new Rect(Screen.width-LeftPanelW,0,Screen.width,Screen.height));
			SelectedObject.OnDrawSelectionGUI();
			GUILayout.EndArea();
		}
		
	}
	// Update is called once per frame
	void Update () {
	
	}


}
