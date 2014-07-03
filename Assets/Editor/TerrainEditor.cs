using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TerrainController))]
public class TerrainEditor : Editor {

	enum SelType
	{
		Block,Building
	}

	string selItem = "";
	SelType selType = SelType.Block;
	bool setDiscover=false;

	private Vector3 mouseHitPos;
	bool edit = false;


	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();

		TerrainController tc = (TerrainController) target;


		tc.MapX = EditorGUILayout.IntField("Size of map, X:",tc.MapX);
		tc.MapX = Mathf.Clamp(tc.MapX,10,200);



		tc.MapZ = EditorGUILayout.IntField("Size of map, Z:",tc.MapZ);
		tc.MapZ = Mathf.Clamp(tc.MapZ,10,200);



		if(GUILayout.Button("Generate!")
		   &&
		   EditorUtility.DisplayDialog("Map generator","This will clear current map. Are you sure?","Yes", "Cancel"))
		{

			tc.Init();
			tc.CreateRandomMap(tc.MapX,tc.MapZ);

		}

		if(GUILayout.Button("Fill map!")
		   &&
		   EditorUtility.DisplayDialog("Map generator","This will clear current map. Are you sure?","Yes","Cancel"))

		{
			tc.Init();
			tc.CreateFilledMap(tc.MapX,tc.MapZ,"Ground");
		}

		if(GUILayout.Button(edit? "Disable editor": "Enable editor"))
		{
			edit = !edit;
			if(edit)
			{
				tc.PrepareMapForEditor();
			}
			else
			{
				tc.PrepareMapAfterEditor();
			}
		}

		if(edit)
		{
			GUILayout.Label("Blocks");

			//scrollBlocks = EditorGUILayout.BeginScrollView(scrollBlocks);
			if(GUILayout.Button("<Clear, Discovered>"))
			{
				selItem = "";
				setDiscover = true;
				selType= SelType.Block;
			}

			if(GUILayout.Button("<Clear, Undiscovered>"))
			{
				selItem = "";
				setDiscover = false;
				selType= SelType.Block;
			}

			foreach(Block b in	tc.GameD.Blocks)
			{
				if(GUILayout.Button(b.Name))
				{
					selItem = b.Name;
					selType= SelType.Block;
				}
			}
			//EditorGUILayout.EndScrollView();



			GUILayout.Label("Buildings");
			
			//scrollBlocks = EditorGUILayout.BeginScrollView(scrollBlocks);
			if(GUILayout.Button("<Clear>"))
			{
				selType= SelType.Building;
				selItem = "";
			}
			

			
			foreach(Building b in	tc.GameD.Buildings)
			{
				if(GUILayout.Button(b.Name))
				{
					selItem = b.Name;
					selType= SelType.Building;
				}
			}
			//EditorGUILayout.EndScrollView();
			//EditorGUILayout.EndFadeGroup();

		}

	}


	/// <summary>
	/// Lets the Editor handle an event in the scene view.
	/// </summary>
	private void OnSceneGUI()
	{
		if(!edit)
			return;
		TerrainController tc = (TerrainController) target;

		// if UpdateHitPosition return true we should update the scene views so that the marker will update in real time
		if (UpdateHitPosition())
		{
			SceneView.RepaintAll();
		}
		
		// Calculate the location of the marker based on the location of the mouse
		RecalculateMarkerPosition();
		
		// get a reference to the current event
		Event current = Event.current;
		
		// if the mouse is positioned over the layer allow drawing actions to occur
		if (this.IsMouseOnLayer())
		{
			// if mouse down or mouse drag event occurred
			if (current.type == EventType.MouseDown || 
			    current.type == EventType.MouseDrag || 
			    current.type==EventType.MouseUp)
			{
				if (current.button == 1)
				{
					MapPoint mp = GetTilePositionFromMouseLocation();
					switch(selType)
					{
					case SelType.Block:
						PlaceBlock(tc,mp);
						break;
					case SelType.Building:
						PlaceBuilding(tc,mp);
						break;
					}
					current.Use();
				}
			}

		}


		/*
		Handles.BeginGUI();
		GUI.Label(new Rect(10, Screen.height - 90, 100, 100), "LMB: Draw");
		GUI.Label(new Rect(10, Screen.height - 105, 100, 100), "RMB: Erase");
		Handles.EndGUI();
		*/
	}

	void PlaceBlock(TerrainController tc, MapPoint mp)
	{

		BlockController bc = tc.Map[mp];
		// if left mouse button is pressed then we draw blocks
		if(selItem.Length>0)
			bc.BlockProt = tc.GameD.BlocksByName[selItem];
		else
		{
			bc.BlockProt = null;
			if(setDiscover)
				tc.Discover(mp.X,mp.Z);
		}
		bc.Generate(tc.TerrGen,true,false);
	}
	
	void PlaceBuilding(TerrainController tc,MapPoint mp)
	{
		BlockController bc = tc.Map[mp];
		if(selItem.Length>0)
		{
			if(bc.BlockProt!=null || bc.cellBuilding!=null)
			{
				Debug.LogWarning("Cannot build here!");
				return;
			}
			Building building = tc.GameD.BuildingsByName[selItem];
			BuildingController buildingController = building.Instantiate().GetComponent<BuildingController>();

			bc.BuildImmediate(buildingController);
			bc.BuildLightCache();
			bc.Generate(tc.TerrGen,true,false);
		}
		else
		{
			bc.RemoveBuildingImmediate();
		}
	}

	/// <summary>
	/// Recalculates the position of the marker based on the location of the mouse pointer.
	/// </summary>
	private void RecalculateMarkerPosition()
	{
		// get reference to the tile map component
		TerrainController tc = (TerrainController) target;
		
		// store the tile location (Column/Row) based on the current location of the mouse pointer
		var tilepos = GetTilePositionFromMouseLocation();
		

		
		// set the TileMap.MarkerPosition value
		tc.markerPosition = tc.transform.position + new Vector3(tilepos.X+0.5f, 0.5f, tilepos.Z+0.5f);
	}

	/// <summary>
	/// Calculates the position of the mouse over the tile map in local space coordinates.
	/// </summary>
	/// <returns>Returns true if the mouse is over the tile map.</returns>
	private bool UpdateHitPosition()
	{
		// get reference to the tile map component
		TerrainController tc = (TerrainController) target;
		
		// build a plane object that 
		var p = new Plane(tc.transform.TransformDirection(Vector3.up), tc.transform.position+new Vector3(0,1,0));
		
		// build a ray type from the current mouse position
		var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		
		// stores the hit location
		var hit = new Vector3();
		
		// stores the distance to the hit location
		float dist;
		
		// cast a ray to determine what location it intersects with the plane
		if (p.Raycast(ray, out dist))
		{
			// the ray hits the plane so we calculate the hit location in world space
			hit = ray.origin + (ray.direction.normalized * dist);
		}
		
		// convert the hit location from world space to local space
		var value = tc.transform.InverseTransformPoint(hit);
		
		// if the value is different then the current mouse hit location set the 
		// new mouse hit location and return true indicating a successful hit test
		if (value != this.mouseHitPos)
		{
			this.mouseHitPos = value;
			return true;
		}
		
		// return false if the hit test failed
		return false;
	}

	/// <summary>
	/// Returns true or false depending if the mouse is positioned over the tile map.
	/// </summary>
	/// <returns>Will return true if the mouse is positioned over the tile map.</returns>
	private bool IsMouseOnLayer()
	{
		// get reference to the tile map component
		TerrainController tc = (TerrainController) target;
		
		// return true or false depending if the mouse is positioned over the map
		return this.mouseHitPos.x > 0 && this.mouseHitPos.x < (tc.MapX) &&
			this.mouseHitPos.z > 0 && this.mouseHitPos.z < (tc.MapZ);
	}

	/// <summary>
	/// Calculates the location in tile coordinates (Column/Row) of the mouse position
	/// </summary>
	/// <returns>Returns a <see cref="Vector2"/> type representing the Column and Row where the mouse of positioned over.</returns>
	private MapPoint GetTilePositionFromMouseLocation()
	{
		// get reference to the tile map component
		TerrainController tc = (TerrainController) target;

		// round the numbers to the nearest whole number using 5 decimal place precision
		MapPoint pos = new MapPoint(
			(int)System.Math.Round(mouseHitPos.x, 5, System.MidpointRounding.ToEven), 
			(int)System.Math.Round(mouseHitPos.z, 5, System.MidpointRounding.ToEven));


		// do a check to ensure that the row and column are with the bounds of the tile map
		pos.Clamp(tc.MapX,tc.MapZ);
		
		// return the column and row values
		return pos;
	}
}

