using UnityEngine;

public class PlayerSpot : MapSpot
{

	public PlayerSpot(Manager m,int x, int z) : base(m,x,z)
	{
		width=5;
		height=5;
	}

	#region implemented abstract members of MapSpot
	public override void Generate (Map map, bool editMode)
	{
	
		for(int i=x;i<x+width;i++)
		{
			for(int j=z;j<z+height;j++)
			{
				if(i>x && i<x+width-1 && j>z && j<z+height-1)
					map[i,j].BlockProt = null;
				map[i,j].Discovered = true;
			}
		}

		BlockController c = map[x+width/2,z+height/2];

		if(!editMode)
		{
			GameObject mainBuilding = M.GameD.BuildingsByName["Headquarters"].Instantiate();
			
			c.BuildImmediate(mainBuilding.GetComponent<BuildingController>());


			PutVehicle(map,x+1,z+1,"Drone");
			PutVehicle(map,x+3,z+1,"Drone");
			PutVehicle(map,x+2,z+3,"Defender Drone");

			ArrangeOnBorder(map,new string[] {"Iron Ore", "Iron Ore","Copper Ore"});
		}
	}
	#endregion
		
}


