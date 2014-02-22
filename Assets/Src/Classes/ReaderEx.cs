//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.17929
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------
using System.IO;
using System;
using UnityEngine;

public class ReaderEx : BinaryReader
{

	public void CheckMagic()
	{
		if(ReadInt32()!=WriterEx.MAGIC)
			throw new UnityException("Magic check failed! There is an error in Save/Load code or game file is broken.");
	}
	public ReaderEx(Stream stream) : base(stream)
	{

	}

	public object ReadEnum(Type enumType)
	{
		string name = ReadString();
		if(name=="")
			throw new UnityException("Enum name cannot be empty!");
		return Enum.Parse (enumType,name);
	}

	public Item ReadItem(Manager m)
	{
		Item res=null;
		m.GameD.Items.TryGetValue(ReadString(),out res);
		return (Item)res;
	}

	public Vehicle ReadVehicle(Manager m)
	{
		Vehicle res = null;
		m.GameD.VehiclesByName.TryGetValue(ReadString(), out res);
		return res;
	}

	public Recipe ReadRecipe(Manager m)
	{
		Recipe res = null;
		m.GameD.RecipesByName.TryGetValue(ReadString(),out res);
		return res;
	}
	public Building ReadBuilding(Manager m)
	{
		Building res = null;
		m.GameD.BuildingsByName.TryGetValue(ReadString(),out res);
		return res;
	}

	public Vector3 ReadVector3()
	{
		return new Vector3(
			(float)ReadDouble(),
			(float)ReadDouble(),
			(float)ReadDouble());
	}

	public object ReadLink(Manager m)
	{
		object res = null;
		m.LoadedLinks.TryGetValue(ReadInt32(),out res);
		return res;
	}
}


