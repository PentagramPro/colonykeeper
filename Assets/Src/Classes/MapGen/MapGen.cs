using System.Collections.Generic;
using UnityEngine;

public class MapGen
{
	List<MapSpot> spots = new List<MapSpot>();

	List<Cell> suitableCells = new List<Cell>();
	public class Cell{
		public bool busy = false;
		public int i, j;
		public Cell(int i, int j)
		{
			this.i=i;
			this.j=j;
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


	public void GenerateMap(BlockController[,] map, bool editMode)
	{
		////////////////////
		// preparing block lists
		List<Block> blocksToPlace = new List<Block>(), fillers = new List<Block>();
		PrepareBlocks(ref blocksToPlace, ref fillers);
		Debug.Log("total blocks to place: "+blocksToPlace.Count+", filler types: "+fillers.Count);

		// filling map
		for(int i=0;i<map.GetLength(0);i++)
		{
			for(int j=0;j<map.GetLength(1);j++)
			{
				if(i==0 || j==0 || i==map.GetUpperBound(0) || j==map.GetUpperBound(1))
				{
					map[i,j].BlockProt = M.GameD.Blocks[0];
					continue;
				}

				map[i,j].BlockProt = fillers[UnityEngine.Random.Range(0,fillers.Count-1)];
			}
		}


		////////////////////
		// placing spots
		Debug.Log("total spots: "+spots.Count);

		Cell[,] cellMap = GenerateSpotsInternal(map.GetLength(1),map.GetLength(0));

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
			map[c.i,c.j].BlockProt = b;
			cells.Remove(c);
		}

	}

	Cell[,] PrepareMap(int width, int height)
	{
		Cell[,] map = new Cell[height,width];
		for(int i=0;i<height;i++)
			for(int j=0;j<width;j++)
				map[i,j] = new Cell(i,j);
		for(int i=0;i<height;i++)
		{
			map[i,0].busy = true;
			map[i,width-1].busy = true;
		}
		for(int j=0;j<width;j++)
		{
			map[0,j].busy = true;
			map[height-1,j].busy = true;
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
			if(spot.X>=0 && spot.Y>=0)
			{
				spotSet = spot.Grow(map,spot.Y,spot.X);
			}
			else
			{
				List<Cell> cells = GetSuitableCells(map,spot.Width,spot.Height);
				if(cells.Count>0)
				{
					Cell c = cells[UnityEngine.Random.Range(0,cells.Count-1)];
					spotSet = spot.Grow(map,c.i,c.j);
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
		for(int i=0;i<map.GetLength(0)-height+1;i++)
		{
			for(int j=0;j<map.GetLength(1)-width+1;j++)
			{
				if(!MapSpot.CheckIfBusy(map,i,j,i+height-1,j+width-1))
					res.Add(map[i,j]);
			}
		}
		return res;
	}


	public void DrawDebugGizmos()
	{
		foreach(Cell c in suitableCells)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireCube(new Vector3(c.j+0.5f,0.5f,c.i+0.5f),new Vector3(0.9f,1.1f,0.9f));
		}
	}
}



