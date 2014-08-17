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
		EditorGUILayout.LabelField("Fire damage",w.fireDamage.ToString("0.00"));
		EditorGUILayout.LabelField("Rotation speed",w.rotationSpeed.ToString("0.00"));
	}
}


