using UnityEngine;
using System.Collections.Generic;

public abstract class MeshGenerator {

	protected List<Vector3> vertices = new List<Vector3>();
	protected List<int> triangles = new List<int>();
	protected List<Vector2> uvs = new List<Vector2>();

	public abstract Mesh Generate();
}
