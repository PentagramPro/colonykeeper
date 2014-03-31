using UnityEngine;

public class PlayerSpot : MapSpot
{

	public PlayerSpot(Manager m,int x, int y) : base(m,x,y)
	{
		width=5;
		height=5;
	}

	#region implemented abstract members of MapSpot
	public override void Generate (BlockController[,] map, bool editMode)
	{
	
		for(int i=y;i<y+height;i++)
		{
			for(int j=x;j<x+width;j++)
			{
				if(i>y && i<y+height-1 && j>x && j<x+width-1)
					map[i,j].BlockProt = null;
				map[i,j].Discovered = true;
			}
		}

		BlockController c = map[y+height/2,x+width/2];

		if(!editMode)
		{
			GameObject mainBuilding = M.GameD.BuildingsByName["Headquarters"].Instantiate();
			
			c.BuildOn(mainBuilding.GetComponent<BuildingController>());
			StorageController st = mainBuilding.GetComponent<StorageController>();
			foreach(PileXML item in M.GameD.StartItemsList)
			{
				st.Put(M.GameD.Items[item.Name],item.Quantity);
			}

			PutVehicle(map,x+1,y+1,"Drone");
			PutVehicle(map,x+3,y+1,"Drone");
			PutVehicle(map,x+2,y+3,"Defender Drone");
		}
	}
	#endregion
		
}


