using UnityEngine;
using System.Collections.Generic;

public class PanelGenerator : MeshGenerator{

	int segments=3;

	public enum Type{
		Near,Far,Right,Left,Top, Bottom
	}

	private Color Col(float v)
	{
		return new Color(v,v,v);
	}

	public PanelGenerator(int segments, Type orientation, float lb,float lhor, float lvert, float l3) 
	{
		if(segments<2)
			throw new UnityException("segments parameter should be equal or greater than 2");
		this.segments = segments;

		switch(orientation)
		{
		case Type.Left:
			GenerateGrid(new Vector3(0,0,0),new Vector3(0,0,1), new Vector3(0,1,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Right:
			GenerateGrid(new Vector3(1,0,0), new Vector3(0,1,0),new Vector3(0,0,1),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Far:
			GenerateGrid(new Vector3(0,0,1),new Vector3(1,0,0), new Vector3(0,1,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Near:
			GenerateGrid(new Vector3(0,0,0),new Vector3(0,1,0), new Vector3(1,0,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Top:
			GenerateGrid(new Vector3(0,1,0), new Vector3(0,0,1),new Vector3(1,0,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Bottom:
			GenerateGrid(new Vector3(0,0,0), new Vector3(0,0,1),new Vector3(1,0,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		}
	}

	private void GenerateGrid(Vector3 b, Vector3 hor, Vector3 vert,
	                          Color cb,Color chor, Color cvert, Color c3)
	{



		for(int x=0;x<segments+1;x++)
		{
			float dx=x/(float)segments;
	
			for(int y=0;y<segments+1;y++)
			{

				float dy = y/(float)segments;
				vertices.Add(b+hor*dy+vert*dx);
				AddUV(dx*0.5f,dy,dx,dy);

				Color cl = dy*cvert+(1-dy)*cb;
				Color cr = dy*c3+(1-dy)*chor;

				Color col = cr*dx+(1-dx)*cl;
				colors.Add(col);

				if(x>0 && y<segments)
				{
					int pos = x*(segments+1)+y;
					triangles.Add(pos);
					triangles.Add(pos-segments-1);
					triangles.Add(pos-segments);

					triangles.Add(pos);
					triangles.Add(pos-segments);
					triangles.Add(pos+1);
				}
			}
		}
	}
}
