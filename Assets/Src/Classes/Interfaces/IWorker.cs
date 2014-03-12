using UnityEngine;
using System.Collections;

public interface IWorker  {


	void SetCallbacks(IJob job);

	void CancelCurrentJob();

	void DriveTo(Vector3 dest);

	BlockController.DigResult Dig(BlockController block);

	bool Unload();

	bool Load(Item itemType, int maxQuantity);

	void OnJobCompleted();

	void Feed(IInventory inv);

	void Pick(IInventory inv, Item itemType, int quantity);


}
