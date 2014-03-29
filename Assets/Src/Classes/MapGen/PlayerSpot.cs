using UnityEngine;

public class PlayerSpot : MapSpot
{

	public PlayerSpot(Manager m,int x, int y) : base(m,x,y)
	{
		width=3;
		height=3;
	}

	#region implemented abstract members of MapSpot
	public override void Generate (BlockController[,] map, bool editMode)
	{
	
		for(int i=y;i<y+height;i++)
		{
			for(int j=x;j<x+width;j++)
			{
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

			PutVehicle(map,x,y,"Drone");
			PutVehicle(map,x+2,y,"Drone");
			PutVehicle(map,x+1,y+2,"Defender Drone");
		}
	}
	#endregion
		
}


