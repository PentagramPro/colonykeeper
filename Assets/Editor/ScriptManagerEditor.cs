using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(ScriptManager))]
public class ScriptManagerEditor : Editor
{
	Type[] conditions = new Type[]{typeof(SCMined),
		typeof(SCTipClosed),
		typeof(SCItemProduced),
		typeof(SCBlockExplored),
		typeof(SCDestroyed)
	};


	bool[] condToggles;

	Type[] actions = new Type[]{typeof(SAEnable),
		typeof(SAEndLevel),
		typeof(SAGoal),
		typeof(SATip)
	};
	bool[] actToggles;

	string controllerName = "";

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		ScriptManager sm = target as ScriptManager;

		if(condToggles==null || condToggles.Length!=conditions.Length)
			condToggles = new bool[conditions.Length];
		if(actToggles==null || actToggles.Length!=actions.Length)
			actToggles = new bool[actions.Length];

		GUILayout.Space(20);
		controllerName = EditorGUILayout.TextField("Name: ",controllerName );
		EditorGUILayout.LabelField("Conditions:");
		int i=0;
		foreach(Type t in conditions)
		{
			condToggles[i] = EditorGUILayout.Toggle(t.ToString(),condToggles[i]);
			i++;
		}

		EditorGUILayout.LabelField("Actions:");
		i=0;
		foreach(Type t in actions)
		{
			actToggles[i] = EditorGUILayout.Toggle(t.ToString(),actToggles[i]);
			i++;
		}

		if(GUILayout.Button("Add controller"))
		{
			GameObject o = new GameObject();
			o.transform.parent = sm.transform;
			o.name = controllerName;
			i=0;
			foreach(Type t in conditions)
			{
				if(condToggles[i])
					o.AddComponent(t);
				i++;
			}
			i=0;
			foreach(Type t in actions)
			{
				if(actToggles[i])
					o.AddComponent(t);
				i++;
			}

		}
	}
}


