
public class SentrySpot : MapSpot
{
	public SentrySpot (Manager m,int x, int y) : base(m,x,y)
	{
		width = 5;
		height = 5;
	}


	#region implemented abstract members of MapSpot
	public override void Generate (BlockController[,] map, bool editMode)
	{
		for(int i=y+1;i<y+4;i++)
		{
			for(int j=x+1;j<x+4;j++)
			{
				map[i,j].BlockProt = null;
			}
		}
		if(!editMode)
			PutVehicle(map,y+height/2,x+width/2,"Alien Sentry");

		ArrangeOnBorder(map,new string[] {"Ice Ore","Ice Ore","Iron Ore"});

	}
	#endregion
}


public class TowerSpot : MapSpot
{
	public TowerSpot (Manager m,int x, int y) : base(m,x,y)
	{
		width = 7;
		height = 5;
	}
	
	
	#region implemented abstract members of MapSpot
	public override void Generate (BlockController[,] map, bool editMode)
	{
		for(int i=y+1;i<y+4;i++)
		{
			for(int j=x+1;j<x+6;j++)
			{
				map[i,j].BlockProt = null;
			}
		}
		
		if(!editMode){
			PutVehicle(map,y+1,x+2,"Alien Sentry Tower");
			PutVehicle(map,y+3,x+4,"Alien Sentry Tower");
		}

		ArrangeOnBorder(map,new string[] {"Ice Ore","Ice Ore","Iron Ore"});

	}
	#endregion
}

public class EnragerSpot : MapSpot
{
	public EnragerSpot (Manager m,int x, int y) : base(m,x,y)
	{
		width = 5;
		height = 5;
	}
	
	
	#region implemented abstract members of MapSpot
	public override void Generate (BlockController[,] map, bool editMode)
	{
		for(int i=y+1;i<y+4;i++)
		{
			for(int j=x+1;j<x+4;j++)
			{
				map[i,j].BlockProt = null;
			}
		}
		
		if(!editMode){
			PutVehicle(map,y+height/2,x+width/2,"Alien Enrager");
		}
		ArrangeOnBorder(map,new string[] {"Ice Ore","Ice Ore","Iron Ore", "Iron Ore"});
	}
	#endregion
}