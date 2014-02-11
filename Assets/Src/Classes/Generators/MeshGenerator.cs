﻿using UnityEngine;
using System.Collections.Generic;

public abstract class MeshGenerator {

	protected List<Vector3> vertices = new List<Vector3>();
	protected List<int> triangles = new List<int>();
	protected List<Vector2> uvs = new List<Vector2>();
	protected List<Vector2> uvs2 = new List<Vector2>();
	protected List<Color> colors = new List<Color>();


	protected void AddVertex(float x, float y, float z)
	{
		vertices.Add(new Vector3(x,y,z));
	}
	protected void AddUV(float u, float v)
	{
		uvs.Add(new Vector2(u,v));
	}
	protected void AddUV(float u, float v, float u2, float v2)
	{
		uvs.Add(new Vector2(u,v));
		uvs2.Add(new Vector2(u2,v2));
	}

	protected void Clear()
	{
		vertices.Clear();
		triangles.Clear();
		uvs.Clear();
		uvs2.Clear();
		colors.Clear();
	}
	public abstract Mesh Generate(int i, int j);
}