using UnityEngine;
using System.Collections.Generic;

public abstract class MeshGenerator {

	protected List<Vector3> vertices = new List<Vector3>();
	protected List<int> triangles = new List<int>();
	protected List<Vector2> uvs = new List<Vector2>();
	protected List<Color> colors = new List<Color>();

	public abstract Mesh Generate();
}
