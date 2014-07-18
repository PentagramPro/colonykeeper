using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

public interface IListItemAdapter
{
    void SetListItem(IListItem item);
	Button GetButton();
    void Activate();
    void Deactivate();
	void Select();
	void Deselect();
}

