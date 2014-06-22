using System.Collections.Generic;
using UnityEngine;

public class MapGen
{
	List<MapSpot> spots = new List<MapSpot>();

	List<Cell> suitableCells = new List<Cell>();
	public class Cell{
		public bool busy = false;
		public int x, z;
		public Cell(int x, int z)
		{
			this.x=x;
			this.z=z;
		}
	}
	Manager M;


	public MapGen (Manager m)
	{
		M= m;
	}

	public void AddSpot(MapSpot s)
	{
		if(!spots.Contains(s))
			spots.Add(s);
	}


	void PrepareBlocks(ref List<Block> blocksToPlace, ref List<Block> fillers)
	{
		foreach (Block b in M.GameD.Blocks)
		{
			if(b.Breakable==false)
				continue;
			
			int count = (int)b.Freq;
			if(count>0)
			{
				for(int n=0;n<count;n++)
				{
					blocksToPlace.Add(b);
				}
			}
			else
			{
				fillers.Add(b);
			}
		}
	}


	public void GenerateMap(Map map, bool editMode)
	{
		////////////////////
		// preparing block lists
		List<Block> blocksToPlace = new List<Block>(), fillers = new List<Block>();
		PrepareBlocks(ref blocksToPlace, ref fillers);
		Debug.Log("total blocks to place: "+blocksToPlace.Count+", filler types: "+fillers.Count);

		// filling map
		for(int x=0;x<map.Width;x++)
		{
			for(int z=0;z<map.Height;z++)
			{
				if(x==0 || z==0 || z==map.Height-1 || x==map.Width-1)
				{
					map[x,z].BlockProt = M.GameD.Blocks[0];
					continue;
				}

				map[x,z].BlockProt =fillers[UnityEngine.Random.Range(0,fillers.Count-1)];
			}
		}


		////////////////////
		// placing spots
		Debug.Log("total spots: "+spots.Count);

		Cell[,] cellMap = GenerateSpotsInternal(map.Width,map.Height);

		Debug.Log("total spots left after generation: "+spots.Count);
	
		////////////////////
		// generating spots
		foreach(MapSpot s in spots)
		{
			s.Generate(map,editMode);
		}


		////////////////////
		// generating resources
		suitableCells = GetSuitableCells(cellMap,1,1);


		List<Cell> cells = new List<Cell>(suitableCells);

		foreach(Block b in blocksToPlace)
		{
			Cell c = cells[UnityEngine.Random.Range(0, cells.Count-1)];
			map[c.x,c.z].BlockProt = b;
			cells.Remove(c);
		}

	}

	Cell[,] PrepareMap(int width, int height)
	{
		Cell[,] map = new Cell[width,height];
		for(int z=0;z<height;z++)
			for(int x=0;x<width;x++)
				map[x,z] = new Cell(x,z);
		for(int x=0;x<width;x++)
		{
			map[x,0].busy = true;
			map[x,height-1].busy = true;
		}
		for(int z=0;z<height;z++)
		{
			map[0,z].busy = true;
			map[width-1,z].busy = true;
		}
		return map;
	}


	Cell[,] GenerateSpotsInternal(int width, int height)
	{

		Cell[,] map = PrepareMap(width,height);

		List<MapSpot> toRemove = new List<MapSpot>();

		foreach(MapSpot spot in spots)
		{
			bool spotSet = false;
			if(spot.X>=0 && spot.Z>=0)
			{
				spotSet = spot.Grow(map,spot.X,spot.Z);
			}
			else
			{
				List<Cell> cells = GetSuitableCells(map,spot.Width,spot.Height);
				if(cells.Count>0)
				{
					Cell c = cells[UnityEngine.Random.Range(0,cells.Count-1)];
					spotSet = spot.Grow(map,c.x,c.z);
				}
			}

			if(!spotSet)
			{
				toRemove.Add(spot);
			}
		}

		Debug.Log("Spots to remove: "+toRemove.Count);

		foreach(MapSpot spot in toRemove)
			spots.Remove(spot);


		return map;

	}



	List<Cell> GetSuitableCells(Cell[,] map,int width, int height)
	{
		List<Cell> res = new List<Cell>();
		for(int x=0;x<map.GetLength(0)-width+1;x++)
		{
			for(int z=0;z<map.GetLength(1)-height+1;z++)
			{
				if(!MapSpot.CheckIfBusy(map,x,z,x+width-1,z+height-1))
					res.Add(map[x,z]);
			}
		}
		return res;
	}


	public void DrawDebugGizmos()
	{
		foreach(Cell c in suitableCells)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(new Vector3(c.x+0.5f,0.5f,c.z+0.5f),new Vector3(0.9f,1.1f,0.9f));
		}
	}
}



