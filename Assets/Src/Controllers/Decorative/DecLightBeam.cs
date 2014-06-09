using UnityEngine;
using System.Collections;

public class DecLightBeam : MonoBehaviour {

	public float angle=45;
	public float nearDistance=2;
	public float farDistance = 4;
	// Use this for initialization
	void Start () {
		RedrawMesh();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void RedrawMesh()
	{
		MeshFilter mf = GetComponent<MeshFilter>();
		Mesh m = mf.sharedMesh;
		if (m == null){
			mf.mesh = new Mesh();
			m = mf.sharedMesh;
		}

		Vector3[] verts = new Vector3[4];

		Vector3 nearV = new Vector3(0,nearDistance,0);
		Vector3 farV = new Vector3(0,farDistance,0);


		float radAng = Mathf.Deg2Rad *angle/2;
		Vector3 nearH = new Vector3(nearDistance*Mathf.Sin(radAng),0,0);
		Vector3 farH = new Vector3(farDistance*Mathf.Sin(radAng),0,0);

		m.Clear();




		verts[0] = nearV+nearH;
		verts[1] = nearV-nearH;

		verts[2] = farV+farH;
		verts[3] = farV-farH;

		m.vertices = verts;

		m.triangles = new int[]{
			0,1,2,
			2,1,3
		};

		m.uv = new Vector2[]{
			new Vector2(0,0),
			new Vector2(1,0),
			new Vector2(0,1),
			new Vector2(1,1)
		};

		m.RecalculateNormals();
		m.RecalculateBounds();

		mf.sharedMesh = m;
	}
}

