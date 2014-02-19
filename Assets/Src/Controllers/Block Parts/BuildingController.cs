using UnityEngine;
using System.Collections;

public class BuildingController : BaseManagedController, IStorable{

	public Building Prototype = null;
	float halfCell = TerrainMeshGenerator.CELL_SIZE/2;
	// Use this for initialization
	void Start () {
	
	}

	public void OnMouseUpAsButton()
	{
		M.GetGUIController().SelectedObject = gameObject;
			
		
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

	public void OnBuilded()
	{
		collider.enabled = true;
	}


	// Update is called once per frame
	void Update () {
	
	}

	#region IStorable implementation
	public override void SaveUid (WriterEx b)
	{
		base.SaveUid (b);
		Component[] components = GetComponents<Component>();
		foreach(Component c in components)
		{
			if(c is IStorable && c!=this)
			{
				((IStorable)c).SaveUid(b);
			}
		}
	}

	public override void LoadUid (Manager m, ReaderEx r)
	{
		base.LoadUid (m, r);
		Component[] components = GetComponents<Component>();
		foreach(Component c in components)
		{
			if(c is IStorable && c!=this)
			{
				((IStorable)c).LoadUid(m,r);
			}
		}
	}
	public void Save (WriterEx b)
	{
		Component[] components = GetComponents<Component>();
		foreach(Component c in components)
		{
			if(c is IStorable && c!=this)
			{
				((IStorable)c).Save(b);
			}
		}
	}

	public void Load (Manager m, ReaderEx r)
	{
		Component[] components = GetComponents<Component>();
		foreach(Component c in components)
		{
			if(c is IStorable && c!=this)
			{
				((IStorable)c).Load(m,r);
			}
		}
	}

	#endregion
}
