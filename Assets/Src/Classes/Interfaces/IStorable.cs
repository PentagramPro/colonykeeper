using System.IO;

public interface IStorable
{
	void Save(WriterEx b);
	void Load(Manager m, ReaderEx r);
}

