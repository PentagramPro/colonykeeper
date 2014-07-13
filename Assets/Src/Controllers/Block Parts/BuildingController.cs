using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(TapController))]
public class BuildingController : BaseManagedController, IStorable{

	public string Name;
	public string LocalName
	{
		get
		{
			return Name;
		}
	}

	[HideInInspector]
	public bool ToRegistryOnStart = false;

	public BlockController nativeBlock = null;

	private List<StaticLight> staticLights = null;

	HullController hull;
	public HullController Hull{
		get
		{
			return hull;
		}
	}
	public Manager.Sides Side = Manager.Sides.Player;
	float halfCell = TerrainMeshGenerator.CELL_SIZE/2;
	// Use this for initialization
	void Start () {
	


		hull = GetComponent<HullController>();
		if(ToRegistryOnStart && nativeBlock!=null)
		{
			M.BuildingsRegistry.Add(nativeBlock,this);
		}

		PrepareLights();

		hull.LocalName = LocalName;
	}

	void PrepareLights()
	{
		if(staticLights==null)
		{
			staticLights = new List<StaticLight>();
			var lights = GetComponentsInChildren<StaticLightController>() as StaticLightController[];
			foreach(StaticLightController l in lights)
			{
				StaticLight light = new StaticLight(this,l.transform.position-transform.position,l.Color);
				light.Falloff = l.Falloff;
                light.Multiplier = l.Multiplier;
				staticLights.Add(light);
			}
            Debug.Log("found " + lights.GetLength(0) + " lights");
		}
	}



	void OnDrawGizmos()
	{
		Vector3 center = transform.position+new Vector3(0,0.5f,0);
		Gizmos.DrawWireCube(center,new Vector3(1,1,1));
		Gizmos.DrawWireSphere(transform.position,0.2f);
	}

	public Vector3 Position
	{
		get{
			return transform.position+new Vector3(halfCell,0,halfCell);
		}
	}

	public void OnBuilded(BlockController blockController)
	{
		collider.enabled = true;
		nativeBlock = blockController;


		PrepareLights();

		foreach(StaticLight l in staticLights)
			blockController.AddLight(l);


	}

	void OnDestroy()
	{
		if(nativeBlock!=null)
			nativeBlock.RemoveAllLights(this);
	}


	// Update is called once per frame
	void Update () {
	
	}

	#region IInteractive implementation
	public void OnSelected()
	{
		
	}
	
	public void OnDeselected()
	{
		
	}

	public void OnDrawSelectionGUI ()
	{
		GUILayout.Label(LocalName);
		GUILayout.Space(10);
	}

	#endregion

	#region IStorable implementation
	public override void SaveUid (WriterEx b)
	{
		base.SaveUid (b);
		ComponentsSaveUid(b);
	}

	public override void LoadUid (Manager m, ReaderEx r)
	{
		base.LoadUid (m, r);
		ComponentsLoadUid(m,r);
	}
	public void Save (WriterEx b)
	{
		ComponentsSave(b);
		b.WriteLink(nativeBlock);
	}

	public void Load (Manager m, ReaderEx r)
	{
		ComponentsLoad(m,r);
		nativeBlock = (BlockController)r.ReadLink(m);
	}

	#endregion
}
