using UnityEngine;
using System.Collections.Generic;

public class PanelGenerator : MeshGenerator{



	public class PanelSettings{
		public Map map;
		public IntVector3 Base;
		public int Segments;
		public float LightMultiplier = 1;
		public PanelSettings (Map map, IntVector3 Base, int segments)
		{
			this.map = map;
			this.Base = Base;

			this.Segments = segments;
		}

		
	}
	public enum Type{
		Near,Far,Right,Left,Top, Bottom
	}

	private Color Col(float v)
	{
		return new Color(v,v,v);
	}

	PanelSettings settings;
	public PanelGenerator(PanelSettings settings, Type orientation, float lb,float lhor, float lvert, float l3) 
	{
		this.settings=settings;
		if(settings.Segments<2)
			throw new UnityException("segments parameter should be equal or greater than 2");

		switch(orientation)
		{
		case Type.Left:
			GenerateGridEven(new IntVector3(0,0,0),new IntVector3(0,0,1), new IntVector3(0,1,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Right:
			GenerateGridEven(new IntVector3(settings.Segments,0,0), new IntVector3(0,1,0),new IntVector3(0,0,1),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Far:
			GenerateGridEven(new IntVector3(0,0,settings.Segments),new IntVector3(1,0,0), new IntVector3(0,1,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Near:
			GenerateGridEven(new IntVector3(0,0,0),new IntVector3(0,1,0), new IntVector3(1,0,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Top:
			GenerateGridEven(new IntVector3(0,settings.Segments,0), new IntVector3(0,0,1),new IntVector3(1,0,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		case Type.Bottom:
			GenerateGridEven(new IntVector3(0,0,0), new IntVector3(0,0,1),new IntVector3(1,0,0),
			             Col (lb),Col(lhor),Col (lvert),Col (l3));
			break;
		}
	}

    

	private void GenerateGridEven(IntVector3 b, IntVector3 hor, IntVector3 vert,
	                              Color cb,Color chor, Color cvert, Color c3)
	{
		float delta=1.0f/(float)settings.Segments;
		for(int x=0;x<=settings.Segments;x++)
		{
			float dx=x/(float)settings.Segments;
			for(int y=0;y<=settings.Segments;y++)
			{
				float dy = y/(float)settings.Segments;

				IntVector3 pos = b+hor*y+vert*x;
				IntVector3 globalPos = new IntVector3(pos.X+settings.Base.X,pos.Y,pos.Z+settings.Base.Z);
				vertices.Add(pos*delta+settings.map.MapVertexes[globalPos.X,globalPos.Y,globalPos.Z]);

				AddUV(dx*0.5f,dy,dx,dy);
				
				Color cl = dy*cvert+(1-dy)*cb;
				Color cr = dy*c3+(1-dy)*chor;
				
				Color col = cr*dx+(1-dx)*cl;
				Vector3 lightColor = settings.map.GetLightAmount(globalPos)+new Vector3(col.r,col.g,col.b);
                col = ClampLight(lightColor);//new Color(0.2f,0.2f,0.2f)+lightColor*1.5f;
				//col+=lightColor;
				col*=settings.LightMultiplier;

				colors.Add(col);

				if(x>0 && y<settings.Segments)
				{
					int p = x*(settings.Segments+1)+y;
					triangles.Add(p);
					triangles.Add(p-settings.Segments-1);
					triangles.Add(p-settings.Segments);
					
					triangles.Add(p);
					triangles.Add(p-settings.Segments);
					triangles.Add(p+1);
				}
			}
		}
		//settings.Grid.get
	}
	private void GenerateGrid(Vector3 b, Vector3 hor, Vector3 vert,
	                          Color cb,Color chor, Color cvert, Color c3)
	{



		for(int x=0;x<settings.Segments+1;x++)
		{
			float dx=x/(float)settings.Segments;
	
			for(int y=0;y<settings.Segments+1;y++)
			{

				float dy = y/(float)settings.Segments;
				//Vector3 noise = settings.Grid[settings.BaseI+];
				vertices.Add(b+hor*dy+vert*dx);
				AddUV(dx*0.5f,dy,dx,dy);

				Color cl = dy*cvert+(1-dy)*cb;
				Color cr = dy*c3+(1-dy)*chor;

				Color col = cr*dx+(1-dx)*cl;
				colors.Add(col);

				if(x>0 && y<settings.Segments)
				{
					int pos = x*(settings.Segments+1)+y;
					triangles.Add(pos);
					triangles.Add(pos-settings.Segments-1);
					triangles.Add(pos-settings.Segments);

					triangles.Add(pos);
					triangles.Add(pos-settings.Segments);
					triangles.Add(pos+1);
				}
			}
		}
	}
}
