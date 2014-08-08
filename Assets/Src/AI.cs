using UnityEngine;
using System.Collections.Generic;

public class AI : BaseManagedController {

	bool hasTargets = false;
	public bool HasTargets{
		get{
			return hasTargets;
		}
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RemoveTarget(AITarget target)
	{
		if(target!=null)
			Destroy(target);
		AITarget[] targets = GetComponentsInChildren<AITarget>();
		if(targets.GetLength(0)==0)
			hasTargets = false;
	}

	public AITarget CreateTarget(HullController target)
	{
		AITarget[] targets = GetComponentsInChildren<AITarget>();
		foreach(AITarget t in targets)
			if (t.Target==target)
				return t;
		GameObject ai = new GameObject();
		AITarget ait = ai.AddComponent<AITarget>();
		ai.transform.parent = transform;
		ait.Target = target;
		hasTargets = true;
		return ait;
	}

	public AITarget GetFirstTarget()
	{
		AITarget[] targets = GetComponentsInChildren<AITarget>();
		if(targets!=null && targets.GetLength(0)>0)
			return targets[0];
		return null;
	}
}
