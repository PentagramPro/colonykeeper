using UnityEngine;
using System.Collections.Generic;

public class PanelGenerator {

	readonly int segments=3;

	public enum Type{
		VRight,VLeft,HRight,HLeft,Top,Bottom
	}

	protected List<Vector3> vertices = new List<Vector3>();
	protected List<int> triangles = new List<int>();

	public PanelGenerator(Type orientation)
	{
		switch(orientation)
		{
		case Type.Left:
			GenerateGrid(new Vector3(0,0,0), new Vector3(0,1,0),new Vector3(0,0,-1));
			break;
		case Type.Right:
			GenerateGrid(new Vector3(1,0,0), new Vector3(0,0,-1),new Vector3(0,1,0));
			break;
		case Type.Far:
			GenerateGrid(new Vector3(0,0,0), new Vector3(0,1,0),new Vector3(0,0,-1));
			break;
		case Type.Near:
			GenerateGrid(new Vector3(0,0,0), new Vector3(0,1,0),new Vector3(0,0,-1));
			break;
		case Type.Top:
			GenerateGrid(new Vector3(0,0,0), new Vector3(0,1,0),new Vector3(0,0,-1));
			break;
		case Type.Bottom:
			GenerateGrid(new Vector3(0,0,0), new Vector3(0,1,0),new Vector3(0,0,-1));
			break;
		}
	}

	private void GenerateGrid(Vector3 b, Vector3 hor, Vector3 vert)
	{
		// first layer
		for(int y=0;y<segments+1;y++)
		{
			vertices.Add(b+hor*y/(float)segments);
		}

		// remaining layers
		for(int x=1;x<segments+1;x++)
		{
			for(int y=0;y<segments+1;y++)
			{
				vertices.Add(b+hor*y/(float)segments+vert*x/(float)segments);
				if(y<segments)
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
