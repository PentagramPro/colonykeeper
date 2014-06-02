﻿using UnityEngine;
using System;

public class TerrainMeshGenerator : MeshGenerator {


	public const float CELL_SIZE = 1;
	public static Color AMBIENT_LIGHT = new Color(0.2f,0.2f,0.2f);
	protected Map map;
	Manager M;
	//int[,] pat = new int[3,3];


	public TerrainMeshGenerator(Manager m) 
	{
		M=m;
	}

	public BlockController.Accessibility GetAccessibility(int i, int j)
	{
		if(map[i,j].Discovered==false)
			return BlockController.Accessibility.Unknown;
		if (i < map.Height - 1 && map [i + 1, j].Digged)
			return BlockController.Accessibility.Cliff;
		if (j < map.Width - 1 && map [i, j + 1].Digged)
			return BlockController.Accessibility.Cliff;
		if (i > 0 && map [i - 1, j].Digged)
			return BlockController.Accessibility.Cliff;
		if (j > 0 && map [i, j - 1].Digged)
			return BlockController.Accessibility.Cliff;
		return BlockController.Accessibility.Enclosed;
	}

	void AttachHRect(Vector2 p1, Vector2 p2, float z, int[,] pat)
	{
		int idx = vertices.Count;
		//0
		AddVertex(p1.x,z,p1.y);
		AddUV(0.0f, 0.0f, 0.0f, 0.0f);

		//1
		AddVertex(p2.x,z,p1.y);
		AddUV(0.5f, 0.0f, 0.0f, 0.0f);

		//3
		AddVertex(p1.x,z,p2.y);
		AddUV(0.0f, 1.0f, 0.0f, 0.0f);

		//2
		AddVertex(p2.x,z,p2.y);
		AddUV(0.5f, 1.0f, 0.0f, 0.0f);
		
		triangles.Add(idx);
		triangles.Add(idx+2);
		triangles.Add(idx+1);
		
		triangles.Add(idx+1);
		triangles.Add(idx+2);
		triangles.Add(idx+3);
	
		float colorBase = z==0?0.4f:1;
		Color color = new Color(colorBase,colorBase,colorBase);

		int turnOnFog = M.settings.FogOfWar?1:0;

		colors.Add(color*(1-pat[0,0]*pat[1,0]*pat[0,1]*pat[1,1]*turnOnFog));
		colors.Add(color*(1-pat[0,2]*pat[1,2]*pat[0,1]*pat[1,1]*turnOnFog));
		colors.Add(color*(1-pat[2,0]*pat[1,0]*pat[2,1]*pat[1,1]*turnOnFog));
		colors.Add(color*(1-pat[2,1]*pat[2,2]*pat[1,2]*pat[1,1]*turnOnFog));


		
	}
	void AttachVRect(Vector2 p1, Vector3 p2, float z1, float z2)
	{
		int idx = vertices.Count;
		AddVertex(p1.x,z1,p1.y);
		AddUV(0.0f, 0.0f, 0.0f, 0.0f);

		AddVertex(p2.x,z1,p2.y);
		AddUV(0.5f, 0.0f, 0.0f, 0.0f);

		AddVertex(p1.x,z2,p1.y);
		AddUV(0.0f, 1.0f, 0.0f, 0.0f);

		AddVertex(p2.x,z2,p2.y);
		AddUV(0.5f, 1.0f, 0.0f, 0.0f);
		
		triangles.Add(idx);
		triangles.Add(idx+1);
		triangles.Add(idx+2);
		
		triangles.Add(idx+1);
		triangles.Add(idx+3);
		triangles.Add(idx+2);

		colors.Add(new Color(1,1,1));
		colors.Add(new Color(1,1,1));
		colors.Add(new Color(0.4f,0.4f,0.4f));
		colors.Add(new Color(0.4f,0.4f,0.4f));
		
	}

	int[,] PreparePattern(int i, int j)
	{
		int[,] p = new int[,] {{1,1,1},{1,1,1},{1,1,1}};

		int h = map.Height-1;
		int w = map.Width-1;


		int il = i-1;
		int jl = j-1;
		int ih = i<h?i+1: h;
		int jh = j<w?j+1: w;
		for(i = il>0?il:0;i<=ih;i++)
		{
			for(j = jl>0?jl:0;j<=jh;j++)
			{
				p[i-il,j-jl] = (map[i,j].Digged && map[i,j].Discovered)? 0 : 1;
			}
		}

		return p;
	}
	public Mesh Generate (Map map,int i, int j)
	{
		Mesh mesh = new Mesh();
		
		this.map = map;

		Clear();
		
		//int h = map.GetUpperBound(0);
		//int w = map.GetUpperBound(1);


	
		BlockController c = map[i,j];

		float level = (c.Digged && (c.Discovered || !M.settings.FogOfWar))?0:CELL_SIZE;
		int turnOnFog = M.settings.FogOfWar?1:0;



		int[,] pat = PreparePattern(i,j);

		float tone = 0.4f;
		PanelGenerator.PanelSettings psettings = new PanelGenerator.PanelSettings(map.MapVertexes,new IntVector3(j*map.Segments,0,i*map.Segments),map.Segments);
		if(pat[1,1]==1)
		{

			Append(new PanelGenerator(psettings,PanelGenerator.Type.Top,
			                          1-pat[0,0]*pat[1,0]*pat[0,1]*turnOnFog,
			                          1-pat[0,2]*pat[1,2]*pat[0,1]*turnOnFog,
			                          1-pat[2,0]*pat[1,0]*pat[2,1]*turnOnFog,
			                          1-pat[2,1]*pat[2,2]*pat[1,2]*turnOnFog));
			if(pat[1,0]==0)
			{
				Append(new PanelGenerator(psettings,PanelGenerator.Type.Left,tone,1,tone,1));
			}

			if(pat[0,1]==0)
			{
				Append(new PanelGenerator(psettings,PanelGenerator.Type.Near,tone,tone,1,1));

			}
			
			if(pat[1,2]==0)
			{
				Append(new PanelGenerator(psettings,PanelGenerator.Type.Right,tone,tone,1,1));

			}
			
			if(pat[2,1]==0)
			{
				Append(new PanelGenerator(psettings,PanelGenerator.Type.Far,tone,1,tone,1));

			}
		}
		else
		{
			Append(new PanelGenerator(psettings,PanelGenerator.Type.Bottom,tone,tone,tone,tone));
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.uv2 = uvs2.ToArray();
		mesh.colors = colors.ToArray();
		mesh.RecalculateNormals();
		

		
	
		return mesh;
	}
}
