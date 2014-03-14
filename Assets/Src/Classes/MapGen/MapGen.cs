using System.Collections.Generic;
using UnityEngine;

public class MapGen
{
	List<MapSpot> spots = new List<MapSpot>();

	public class Cell{
		public bool busy = false;
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

	//step 1
	public Block[,] GenerateBlocksPattern(int iLen, int jLen )
	{
		Manager m = M;
		List<Block> blocksToPlace = new List<Block>();
		Block[,] map = new Block[iLen, jLen];
		
		
		
		float freqSum = 0;
		float blocksCount = (iLen - 1) * (jLen - 1)-9;
		if (blocksCount < 9)
			throw new UnityException("Wrong map size!");
		
		foreach (Block b in m.GameD.Blocks)
		{
			if(b.Breakable==false)
				continue;
			
			freqSum+=b.Freq;
		}
		
		foreach (Block b in m.GameD.Blocks)
		{
			if(b.Breakable==false)
				continue;
			
			int count = (int)(blocksCount/freqSum*b.Freq+1);
			for(int n=0;n<count;n++)
			{
				blocksToPlace.Add(b);
			}
		}
		
	
		
		
		for (int i=0; i<iLen; i++)
		{
			for (int j=0; j<jLen; j++)
			{
				if(i==0 || j==0 || i==iLen-1 || j==jLen-1)
				{
					map[i,j] = m.GameD.Blocks[0];
					continue;
				}
				

				
				int pick = UnityEngine.Random.Range(0,blocksToPlace.Count-1);
				map[i,j] = blocksToPlace[pick];
				blocksToPlace.RemoveAt(pick);
			}
		}
		
		return map;
	}

	// step 2
	public void GenerateSpots(BlockController[,] map, bool editMode)
	{

		Debug.Log("total spots: "+spots.Count);

		GenerateSpotsInternal(map.GetLength(1),map.GetLength(0));

		Debug.Log("total spots left after generation: "+spots.Count);
	
		// place spots
		foreach(MapSpot s in spots)
		{
			s.Generate(map,editMode);
		}
	}

	void GenerateSpotsInternal(int width, int height)
	{

		Cell[,] map = new Cell[height,width];
		for(int i=0;i<height;i++)
			for(int j=0;j<width;j++)
				map[i,j] = new Cell();
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

		List<MapSpot> toRemove = new List<MapSpot>();

		foreach(MapSpot spot in spots)
		{

			int posI = spot.Y>=0? spot.Y : Random.Range(1,height-2);
			int posJ = spot.X>=0? spot.X : Random.Range(1,width-2);
			bool spotSet = false;

			for(int k=0;k<10;k++)
			{
				if(spot.Grow(map,posI,posJ))
				{
					spotSet = true;
					break;
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




	}


}



