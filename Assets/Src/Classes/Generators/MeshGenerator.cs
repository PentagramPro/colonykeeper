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

	public void Append(MeshGenerator source)
	{
		int shift = vertices.Count;
		vertices.AddRange(source.vertices);
		foreach(int v in source.triangles)
			triangles.Add(v+shift);

		uvs.AddRange(source.uvs);
		uvs2.AddRange(source.uvs2);
		colors.AddRange(source.colors);
	}

	public static Color ClampLight(Vector3 light)
	{
		float k = 2f;
		return new Color(
			1-k/(light.x+k),
			1-k/(light.y+k),
			1-k/(light.z+k)
			);
	}
	//public abstract Mesh Generate(Map map,int i, int j);
}
