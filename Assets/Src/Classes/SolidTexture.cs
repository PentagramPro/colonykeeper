using UnityEngine;
using System.Collections.Generic;

public class SolidTexture
{
	static Dictionary<Color, Texture2D> texCache = new Dictionary<Color, Texture2D>();

	public static Texture2D GetTexture(Color color)
	{

		Texture2D tex = null;
		if(texCache.TryGetValue(color,out tex))
			return tex;
		tex = new Texture2D(1,1);
		tex.SetPixel(0,0,color);
		tex.wrapMode = TextureWrapMode.Repeat;
		tex.Apply();
		texCache.Add(color,tex);
		return tex;

	}
}


