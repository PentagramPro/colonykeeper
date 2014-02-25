using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIController : BaseManagedController {

	//List<Block> blocks = new List<Block>();
	enum Modes {
		Idle, BuildChoose, BuildPlace, Info
	}

	enum ToolButton{
		None, Build,Info
	}
	private Modes state = Modes.Idle;

	public delegate void PickedDelegate(Building pickedBuilding);

	public event PickedDelegate ItemPicked;

	public GameObject SelectedObject;

	public GUISkin Skin;

	Rect windowRect;
	float panelWidth;
	float mapHeight;
	float toolbarHeight;
	float pad = 10;
	Vector2 buildingScrollPos = new Vector2();
	Vector2 infoWindowScroll = new Vector2();

	// Use this for initialization
	void Start () {
		panelWidth = Screen.width *0.25f;
		mapHeight = Screen.height * 0.25f;
		toolbarHeight = Screen.height*0.1f;

		windowRect=new Rect(Screen.width*0.1f,Screen.height*0.1f, Screen.width*0.8f, Screen.height*0.8f);
	}

	void OnGUI()
	{
		ToolButton toolBtn = ToolButton.None;
		GUI.skin = Skin;

		//Map and toolbar box
		GUI.Box(new Rect(0,Screen.height-mapHeight-toolbarHeight,panelWidth,mapHeight+toolbarHeight),"");

		//Left panel rect
		Rect rct = new Rect(0,0,panelWidth,Screen.height-mapHeight-toolbarHeight);

		//Toolbar rect
		Rect toolRect = new Rect(
			0+pad,Screen.height - mapHeight-toolbarHeight+pad,
			panelWidth-pad*2,toolbarHeight-pad*2);


		GUILayout.BeginArea(toolRect);
		GUILayout.BeginHorizontal();

		if(GUILayout.Button("Build"))
			toolBtn = ToolButton.Build;
		
		if(GUILayout.Button("Info"))
			toolBtn = ToolButton.Info;

		GUILayout.EndHorizontal();
		GUILayout.EndArea();



		switch(state)
		{
		case Modes.Idle:
		{

			if(toolBtn==ToolButton.Build)
				state = Modes.BuildChoose;
			else if(toolBtn==ToolButton.Info)
				state = Modes.Info;

			//Left panel box
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
			break;
		case Modes.BuildChoose:
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
				if(b.Hide)
					continue;
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
				state = Modes.BuildPlace;
				
			}
		}
			break;
		case Modes.BuildPlace:
		{
		}
			break;
		case Modes.Info:
		{
			GUILayout.Window(0,windowRect,OnDrawInfoWindow,"Information");
		}
			break;
		}

		
	}

	public void OnDrawInfoWindow(int id)
	{
		infoWindowScroll = GUILayout.BeginScrollView(infoWindowScroll);


		foreach(Item i in M.Stat.Items.Keys)
		{

			GUILayout.BeginHorizontal();
			GUILayout.Label(i.Name);
			GUILayout.Label( (M.Stat.Items[i]/100.0f).ToString("n2") );
			GUILayout.EndHorizontal();

		}
		GUILayout.EndScrollView();
		if(GUILayout.Button("Close"))
			state = Modes.Idle;
	}

	public void OnPlaced()
	{
		if(state==Modes.BuildPlace)
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
