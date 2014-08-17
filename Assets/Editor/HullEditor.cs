using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(HullController))]
public class HullEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		HullController h = (HullController)target;
		EditorGUILayout.LabelField("Cur HP",h.CurHP.ToString("0.00"));
		EditorGUILayout.LabelField("Max HP",h.MaxHP.ToString("0.00"));
	}
}


