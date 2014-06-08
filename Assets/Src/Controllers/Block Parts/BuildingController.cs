using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TapController))]
public class BuildingController : BaseManagedController, IStorable, IInteractive{

	public Building Prototype = null;
	public BlockController nativeBlock = null;

	public Manager.Sides Side = Manager.Sides.Player;
	float halfCell = TerrainMeshGenerator.CELL_SIZE/2;
	// Use this for initialization
	void Start () {
	
		GetComponent<TapController>().OnTap+=OnTap;
	}

	void OnTap()
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

	public void OnBuilded(BlockController blockController)
	{
		collider.enabled = true;
		nativeBlock = blockController;

		M.terrainController.Map.SetLight(nativeBlock.MapPos,true);
	}

	void OnDestroy()
	{
		if(nativeBlock!=null)
			M.terrainController.Map.SetLight(nativeBlock.MapPos,false);
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
		GUILayout.Label(Prototype.Name);
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
