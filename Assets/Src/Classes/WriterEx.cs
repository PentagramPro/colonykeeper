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

public class WriterEx : BinaryWriter
{
	public WriterEx(Stream stream) : base(stream)
	{
		
	}

	// val can be null, in that case empty string is written
	public void WriteEx(Block val)
	{
		if(val!=null)
			Write (val.Name);
		else 
			Write ("");
	}

	public void WriteEnum(object obj)
	{
		Write (Enum.GetName(obj.GetType(),obj));
	}


}

