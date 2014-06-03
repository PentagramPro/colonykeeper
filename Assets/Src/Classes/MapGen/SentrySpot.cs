
public class SentrySpot : MapSpot
{
	public SentrySpot (Manager m,int x, int z) : base(m,x,z)
	{
		width = 5;
		height = 5;
	}


	#region implemented abstract members of MapSpot
	public override void Generate (Map map, bool editMode)
	{
		for(int i=x+1;i<x+4;i++)
		{
			for(int j=z+1;j<z+4;j++)
			{
				map[i,j].BlockProt = null;
			}
		}
		if(!editMode)
			PutVehicle(map,x+width/2,z+height/2,"Alien Sentry");

		ArrangeOnBorder(map,new string[] {"Ice Ore","Ice Ore","Iron Ore", "Copper Ore","Copper Ore"});

	}
	#endregion
}


public class TowerSpot : MapSpot
{
	public TowerSpot (Manager m,int x, int z) : base(m,x,z)
	{
		width = 7;
		height = 5;
	}

	
	#region implemented abstract members of MapSpot
	public override void Generate (Map map, bool editMode)
	{
		for(int i=x+1;i<x+4;i++)
		{
			for(int j=z+1;j<z+6;j++)
			{
				map[i,j].BlockProt = null;
			}
		}
		
		if(!editMode){
			PutVehicle(map,x+1,z+2,"Alien Sentry Tower");
			PutVehicle(map,x+3,z+4,"Alien Sentry Tower");
		}

		ArrangeOnBorder(map,new string[] {"Ice Ore","Ice Ore","Ice Ore","Iron Ore","Cobalt Ore"});

	}
	#endregion
}

public class EnragerSpot : MapSpot
{
	public EnragerSpot (Manager m,int x, int z) : base(m,x,z)
	{
		width = 5;
		height = 5;
	}
	
	
	#region implemented abstract members of MapSpot
	public override void Generate (Map map, bool editMode)
	{
		for(int i=x+1;i<x+4;i++)
		{
			for(int j=z+1;j<z+4;j++)
			{
				map[i,j].BlockProt = null;
			}
		}
		
		if(!editMode){
			PutVehicle(map,x+width/2,z+height/2,"Alien Enrager");
		}
		ArrangeOnBorder(map,new string[] {"Ice Ore","Ice Ore","Iron Ore", "Cobalt Ore"});
	}
	#endregion
}