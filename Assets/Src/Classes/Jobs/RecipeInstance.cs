
using System.Collections.Generic;
public class RecipeInstance : IStorable
{
	UidContainer uidc;
	public List<Pile> Ingredients = new List<Pile>();
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

	public int GetIngredient(Item itemType)
	{
		foreach (Pile p in Ingredients)
		{
			if(p.ItemType==itemType)
				return p.Quantity;
		}
		
		return 0;
	}

	#region IStorable implementation

	public void SaveUid (WriterEx b)
	{
		uidc.SaveUid(b);
	}

	public void LoadUid (Manager m, ReaderEx r)
	{
		uidc.LoadUid(m,r);
	}

	public void Save (WriterEx b)
	{
		b.WriteEx(Prototype);
		b.Write(Ingredients.Count);
		foreach(Pile p in Ingredients)
		{
			p.Save(b);
		}
	}

	public void Load (Manager m, ReaderEx r)
	{
		Prototype = r.ReadRecipe(m);
		Ingredients.Clear();
		int count = r.ReadInt32();
		for(int i=0;i<count;i++)
		{
			Pile p = new Pile(null);
			p.Load(m,r);
			Ingredients.Add(p);
		}

	}

	public int GetUID ()
	{
		return uidc.UID;
	}

	#endregion
}

