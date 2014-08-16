
using System.Collections.Generic;
public class RecipeInstance : IStorable
{
	UidContainer uidc;
	public List<PileRequest> Ingredients = new List<PileRequest>();
	public Recipe Prototype;

	public List<Pile> ResultsLinks{
		get{
			return Prototype.ResultsLinks;
		}
	}


	public string Name{
		get{
			return Prototype.Name;
		}
	}
	public RecipeInstance()
	{
		uidc = new UidContainer(this);
	}

	public int GetIngredient(PileRequest itemType)
	{
		foreach (PileRequest p in Ingredients)
		{
			if(p.IsSameItem(itemType))
				return p.Quantity;
		}
		
		return 0;
	}

	public ItemProps GenerateItemProps()
	{
		ItemProps res = new ItemProps();


		int index=0;
		foreach(Ingredient i in Prototype.Ingredients)
		{
			PileRequest thatPile = Ingredients[index];
			foreach(PropertyTransfer transfer in i.Properties)
			{
				if(transfer.EmptyPropertyNames)
					continue;
				
				if(transfer.DestinationProperty=="Color")
					res.color = thatPile.Properties.color;
				else if (transfer.DestinationProperty=="Color2")
					res.secondaryColor = thatPile.Properties.secondaryColor;
				else
				{
					string src = transfer.SourceProperty;
					string dst = transfer.DestinationProperty;
					if(transfer.EqualPropertyNames)
						src = transfer.DestinationProperty;
					
					res[dst]=thatPile.Properties[src]*transfer.Multiplier;
				}
			}
			index++;
		}
		return res;
	}

	#region IStorable implementation

	public void SaveUid (WriterEx b)
	{
		uidc.Save(b);
	}

	public void LoadUid (Manager m, ReaderEx r)
	{
		uidc.Load(m,r);
	}

	public void Save (WriterEx b)
	{
//		b.WriteEx(Prototype);
//		b.Write(Ingredients.Count);
//		foreach(Pile p in Ingredients)
//		{
//			p.Save(b);
//		}
	}

	public void Load (Manager m, ReaderEx r)
	{
//		Prototype = r.ReadRecipe(m);
//		Ingredients.Clear();
//		int count = r.ReadInt32();
//		for(int i=0;i<count;i++)
//		{
//			Pile p = new Pile(null);
//			p.Load(m,r);
//			Ingredients.Add(p);
//		}

	}

	public int GetUID ()
	{
		return uidc.UID;
	}

	#endregion
}

