//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1022
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;

public class UidContainer : IStorable
{
	private static int nextUid=0;
	private int uid;
	private object owner;
	public int UID{
		get{
			return uid;
		}
	}

	public UidContainer (object owner)
	{
		uid = nextUid++;
		this.owner=owner;
	}

	#region IStorable implementation
	public void SaveUid(WriterEx b)
	{
	
	}
	
	public void LoadUid(Manager m, ReaderEx r)
	{
	
	}

	public void Save (WriterEx b)
	{
		b.Write(UID);
	}

	public void Load (Manager m, ReaderEx r)
	{
		int loadeduid = r.ReadInt32();
		m.LoadedLinks.Add(loadeduid, owner);
	}

	#endregion
}

