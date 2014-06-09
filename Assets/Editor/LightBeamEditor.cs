using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(DecLightBeam))]
public class LightBeamEditor : Editor
{

	public override void OnInspectorGUI()
	{
		//DrawDefaultInspector();
		bool update = false;

		DecLightBeam b = (DecLightBeam)target;
		float angle = EditorGUILayout.FloatField("Angle:",b.angle);
		if(angle<5)
			angle=5;
		else if(angle>179)
			angle=179;

		float near = EditorGUILayout.FloatField("Near distance:",b.nearDistance);
		float far  = EditorGUILayout.FloatField("Far distance:",b.farDistance);

		if(far<0.01f)
			far=0.01f;
		if(near>far)
			near=far-0.01f;
		if(near<0)
			near=0;

		
		if(b.angle!=angle || b.nearDistance!=near || b.farDistance!=far)
		{
			b.angle=angle;
			b.nearDistance=near;
			b.farDistance=far;
			//update
			b.RedrawMesh();
		}


	}
}



