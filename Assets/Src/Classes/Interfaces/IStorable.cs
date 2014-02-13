using System.IO;

public interface IStorable
{
	void Save(BinaryWriter b);
	void Load(BinaryReader r);
}

