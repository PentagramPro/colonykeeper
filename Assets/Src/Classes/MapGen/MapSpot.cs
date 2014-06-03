using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class MapSpot
{
	protected int x,z;
	public int X{
		get{
			return x;
		}
	}
	public int Z{
		get{
			return z;
		}
	}
	protected int width,height;
	public int Width{
		get{
			return width;
		}
	}
	public int Height{
		get{
			return height;
		}
	}
	protected Manager M;
	public MapSpot (Manager m,int x, int z)
	{
		this.x=x;
		this.z=z;
		M=m;
	}

	public abstract void Generate(Map map, bool editMode);


	protected void PutVehicle(Map map, int x, int z,string name)
	{
		M.CreateVehicle(name,map[x,z].Position);
		
	}
	public bool Grow(MapGen.Cell[,] map, int posX, int posZ)
	{
		int curX = posX, curZ = posZ;
		int curH = 1,curW = 1;

		bool blockLeft = false;
		bool blockRight = false;
		bool blockTop = false;
		bool blockBottom = false;

		while(true)
		{
			if(!blockLeft)
			{
				if(CheckIfBusy(map,curX-1,curZ,curX-1,curZ+curH))
				{
					blockLeft = true;
				}

				if(!blockLeft)
				{
					curX--;
					curW++;

				}
			}
			if(curW==width)
				blockLeft=blockRight=true;
			if(!blockRight)
			{
				if(CheckIfBusy(map,curX+curW+1,curZ,curX+curW+1,curZ+curH))
				{
					blockRight = true;
				}
				
				if(!blockRight)
				{
					curW++;

				}
			}
			if(curW==width)
				blockLeft=blockRight=true;

			if(!blockTop)
			{
				if(CheckIfBusy(map,curX,curZ-1,curX+curW,curZ-1))
				{
					blockTop = true;
				}
				
				if(!blockTop)
				{
					curZ--;
					curH++;

				}
			}
			if(curH==height)
				blockTop=blockBottom=true;

			if(!blockBottom)
			{
				if(CheckIfBusy(map,curX,curZ+curH+1,curX+curW,curZ+curH+1))
				{
					blockBottom = true;
				}
				
				if(!blockBottom)
				{
					curH++;

				}
			}
			if(curH==height)
				blockTop=blockBottom=true;




			if(blockTop && blockBottom && blockLeft && blockRight)
			{
				x = curX;
				z = curZ;
				bool res = curH==height && curW==width;
				if(res)
				{
					SetBusy(map,x,z,x+width-1,z+height-1);
				}
				return res;
			}
		}
		
		
	}

	public static bool CheckIfBusy(MapGen.Cell[,] map, int x1, int z1, int x2, int z2)
	{
		for(int x=x1;x<=x2;x++)
		{
			for(int z=z1;z<=z2;z++)
			{
				if(map[x,z].busy)
					return true;
			}
		}
		return false;
	}


	public static void SetBusy(MapGen.Cell[,] map, int x1, int z1, int x2, int z2)
	{
		for(int x=x1;x<=x2;x++)
		{
			for(int z=z1;z<=z2;z++)
			{
				map[x,z].busy = true;
			}
		}
	}


	protected void ArrangeOnBorder(Map map, string[] blockNames)
	{
		List<BlockController> cells = new List<BlockController>();
		for (int i =x; i<x+width; i++)
		{
			cells.Add(map[i,z]);
			cells.Add(map[i,z+height-1]);
		}

		for (int j=z+1; j<z+height-1; j++)
		{
			cells.Add(map[x,j]);
			cells.Add(map[x+width-1,j]);
		}

		foreach (string name in blockNames)
		{
			Block block = null;
			if(cells.Count==0)
				break;

			if(!M.GameD.BlocksByName.TryGetValue(name,out block))
				continue;

			BlockController bc = cells[UnityEngine.Random.Range(0,cells.Count)];
			bc.BlockProt = block;
			cells.Remove(bc);
		}
	}
}

