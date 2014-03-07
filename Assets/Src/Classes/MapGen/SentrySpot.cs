
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

		PutVehicle(map,y+height/2,x+width/2,"Alien Sentry");
	}
	#endregion
}


