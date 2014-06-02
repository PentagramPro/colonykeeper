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
		int curX = posX, curY = posZ;
		int curH = 1,curW = 1;

		bool blockLeft = false;
		bool blockRight = false;
		bool blockTop = false;
		bool blockBottom = false;

		while(true)
		{
			if(!blockLeft)
			{
				if(CheckIfBusy(map,curY,curX-1,curY+curH,curX-1))
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
				if(CheckIfBusy(map,curY,curX+curW+1,curY+curH,curX+curW+1))
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
				if(CheckIfBusy(map,curY-1,curX,curY-1,curX+curW))
				{
					blockTop = true;
				}
				
				if(!blockTop)
				{
					curY--;
					curH++;

				}
			}
			if(curH==height)
				blockTop=blockBottom=true;

			if(!blockBottom)
			{
				if(CheckIfBusy(map,curY+curH+1,curX,curY+curH+1,curX+curW))
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
				y = curY;
				bool res = curH==height && curW==width;
				if(res)
				{
					SetBusy(map,y,x,y+height-1,x+width-1);
				}
				return res;
			}
		}
		
		
	}

	public static bool CheckIfBusy(MapGen.Cell[,] map, int i1, int j1, int i2, int j2)
	{
		for(int i=i1;i<=i2;i++)
		{
			for(int j=j1;j<=j2;j++)
			{
				if(map[i,j].busy)
					return true;
			}
		}
		return false;
	}


	public static void SetBusy(MapGen.Cell[,] map, int i1, int j1, int i2, int j2)
	{
		for(int i=i1;i<=i2;i++)
		{
			for(int j=j1;j<=j2;j++)
			{
				map[i,j].busy = true;
			}
		}
	}


	protected void ArrangeOnBorder(Map map, string[] blockNames)
	{
		List<BlockController> cells = new List<BlockController>();
		for (int i =y; i<y+height; i++)
		{
			cells.Add(map[i,x]);
			cells.Add(map[i,x+width-1]);
		}

		for (int j=x+1; j<x+width-1; j++)
		{
			cells.Add(map[y,j]);
			cells.Add(map[y+height-1,j]);
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

