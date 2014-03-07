using UnityEngine;
using System;


public abstract class MapSpot
{
	protected int x,y;
	public int X{
		get{
			return x;
		}
	}
	public int Y{
		get{
			return y;
		}
	}
	protected int width,height;
	protected Manager M;
	public MapSpot (Manager m,int x, int y)
	{
		this.x=x;
		this.y=y;
		M=m;
	}

	public abstract void Generate(BlockController[,] map, bool editMode);


	protected void PutVehicle(BlockController[,] map, int i, int j,string name)
	{
		GameObject veh = M.GameD.VehiclesByName[name].Instantiate();
		M.VehiclesRegistry.Add(veh.GetComponent<VehicleController>());
		veh.transform.position = map[i,j].Position;
	}
	public bool Grow(MapGen.Cell[,] map, int posI, int posJ)
	{
		int curX = posJ, curY = posI;
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
					SetBusy(map,curY,curX,curY+curH,curX);
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
					SetBusy(map,curY,curX+curW,curY+curH,curX+curW);
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
					SetBusy(map,curY,curX,curY,curX+curW);
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
					SetBusy(map,curY+curH,curX,curY+curH,curX+curW);
				}
			}
			if(curH==height)
				blockTop=blockBottom=true;




			if(blockTop && blockBottom && blockLeft && blockRight)
			{
				x = curX;
				y = curY;
				return curH==height && curW==width;
			}
		}
		
		
	}

	public static bool CheckIfBusy(MapGen.Cell[,] map, int i1, int j1, int i2, int j2)
	{
		for(int i=i1;i<=i2;i++)
		{
			for(int j=j1;j<j2;j++)
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
			for(int j=j1;j<j2;j++)
			{
				map[i,j].busy = true;
			}
		}
	}
}

