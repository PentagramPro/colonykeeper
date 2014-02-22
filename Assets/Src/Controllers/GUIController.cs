using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIController : BaseManagedController {

	//List<Block> blocks = new List<Block>();
	enum Modes {
		Idle, Choose, Place
	}
	private Modes state = Modes.Idle;

	public delegate void PickedDelegate(Building pickedBuilding);

	public event PickedDelegate ItemPicked;

	public GameObject SelectedObject;

	public GUISkin Skin;

	float panelWidth;
	float mapHeight;
	float toolbarHeight;
	float pad = 10;
	Vector2 buildingScrollPos = new Vector2();

	// Use this for initialization
	void Start () {
		panelWidth = Screen.width *0.25f;
		mapHeight = Screen.height * 0.25f;
		toolbarHeight = Screen.height*0.1f;


	}

	void OnGUI()
	{
		GUI.skin = Skin;

		GUI.Box(new Rect(0,Screen.height-mapHeight-toolbarHeight,panelWidth,mapHeight+toolbarHeight),"");

		Rect rct = new Rect(0,0,panelWidth,Screen.height-mapHeight-toolbarHeight);

		if(state== Modes.Idle)
		{
			if(GUI.Button(new Rect(
				0+pad,Screen.height - mapHeight-toolbarHeight+pad,
				panelWidth-pad*2,toolbarHeight-pad*2),"Build"))
			{
				state = Modes.Choose;
			}

			GUI.Box(rct,"");
			rct.x+=pad;
			rct.y+=pad;
			rct.width-=pad*2;
			rct.height-=pad*2;
			
			GUILayout.BeginArea(rct);

			if (SelectedObject != null)
			{
				Component[] items = SelectedObject.GetComponents<Component>();

				foreach(Component item in items)
				{
					if(item is IInteractive)
						((IInteractive)item).OnDrawSelectionGUI();
				}

			}
			else
			{
				if(GUILayout.Button("Save"))
				{
					M.SaveGame();
				}
				if(GUILayout.Button("Load"))
				{
					M.LoadGame();
				}

			}
			GUILayout.EndArea();
		}
		else if(state == Modes.Choose)
		{

			GUI.Box(rct,"");
			rct.x+=pad;
			rct.y+=pad;
			rct.width-=pad*2;
			rct.height-=pad*2;
			GUILayout.BeginArea(rct);
			GUILayout.BeginScrollView(buildingScrollPos);

			Building selected = null;
			
			
			foreach(Building b in M.GameD.Buildings)
			{
				if(GUILayout.Button(b.Name))
				{
					selected = b;	
				}
			}


			GUILayout.EndScrollView();
			GUILayout.EndArea();

			if(selected!=null)
			{
				
				if(ItemPicked!=null)
					ItemPicked(selected);
				state = Modes.Place;
				
			}

		}
		/*
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

			if(ItemPicked!=null)
				ItemPicked(selected);

			
		}

		if (SelectedObject != null)
		{
			GUILayout.BeginArea(new Rect(Screen.width-LeftPanelW,0,LeftPanelW,Screen.height));
			SelectedObject.OnDrawSelectionGUI();
			GUILayout.EndArea();
		}
		*/
		
	}

	public void OnPlaced()
	{
		if(state==Modes.Place)
			state= Modes.Idle;
	}

	public void OnDeselect()
	{
		if(state==Modes.Idle)
			SelectedObject = null;
	}
	// Update is called once per frame
	void Update () {
	
	}


}
