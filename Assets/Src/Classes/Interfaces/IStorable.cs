using System.IO;

public interface IStorable
{
	void SaveUid(WriterEx b);
	void LoadUid(Manager m, ReaderEx r);
	void Save(WriterEx b);
	void Load(Manager m, ReaderEx r);
}

