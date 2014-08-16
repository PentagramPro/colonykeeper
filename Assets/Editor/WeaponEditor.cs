using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(WeaponController))]
public class WeaponEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		WeaponController w = (WeaponController)target;
		EditorGUILayout.LabelField("Weapon DPS",w.DPS.ToString("0.00"));
	}
}


