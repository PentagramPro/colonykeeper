using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IListItemAdapter
{
    void SetListItem(IListItem item);
    void Activate();
    void Deactivate();
	void Select();
	void Deselect();
}

