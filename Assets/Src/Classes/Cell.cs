using UnityEngine;
using System.Collections;

public class Cell {


	//public Mesh mapMesh;
	public Color Lighting = new Color(0,0,0);

	// [data]
	// block attached to this cell
	public BlockController Block;

	// [data]
	// is this celll digged and can be used to place blocks?
	public bool Digged=false;

	// [performance]
	// vertex indexes used by mesh generator 
	public int lt,lb,rt,rb;



}
