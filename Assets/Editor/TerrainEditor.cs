using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TerrainController))]
public class TerrainEditor : Editor {




	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();

		TerrainController tc = (TerrainController) target;

		tc.MapX = EditorGUILayout.IntField("Size of map, X:",tc.MapX);
		if(tc.MapX<10)
			tc.MapX=10;
		else if(tc.MapX>200)
			tc.MapX=200;

		tc.MapZ = EditorGUILayout.IntField("Size of map, Z:",tc.MapZ);
		if(tc.MapZ<10)
			tc.MapZ=10;
		else if(tc.MapZ>200)
			tc.MapZ=200;


		if(GUILayout.Button("Generate!"))
		{
			tc.Init();
			tc.PrepareTerrain(tc.MapX,tc.MapZ);
		}
	}
}
