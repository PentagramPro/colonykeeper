using UnityEngine;
using System;
using System.Collections.Generic;

public class MapGenerator
{

	static List<Block> blocksToPlace = new List<Block>();

	public static Block[,] GenerateBlocksPattern(Manager m, int iLen, int jLen, Func<int, int, int> placeHeadquarters )
	{
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
			int count = (int)(blocksCount/freqSum*b.Freq+1);
			for(int n=0;n<count;n++)
			{
				blocksToPlace.Add(b);
			}
		}

		int iMid = iLen / 2;
		int jMid = jLen / 2;


		for (int i=0; i<iLen; i++)
		{
			for (int j=0; j<jLen; j++)
			{
				if(i==0 || j==0 || i==iLen-1 || j==jLen-1)
				{
					map[i,j] = m.GameD.Blocks[0];
					continue;
				}

				if(i>=iMid-1 && i<=iMid+1 && j>=jMid-1 && j<=jMid+1)
				{
					if(i==iMid && j==jMid)
						placeHeadquarters(i,j);
					continue;
				}

				int pick = UnityEngine.Random.Range(0,blocksToPlace.Count-1);
				map[i,j] = blocksToPlace[pick];
				blocksToPlace.RemoveAt(pick);
			}
		}

		return map;
	}
}


