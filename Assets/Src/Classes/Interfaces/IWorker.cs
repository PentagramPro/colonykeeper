using UnityEngine;
using System.Collections;

public interface IWorker  {


	void SetCallbacks(IJob job);

	void CancelCurrentJob();

	void DriveTo(Vector3 dest);

	BlockController.DigResult Dig(BlockController block);

	void Unload();

	void Load(Item itemType, float maxQuantity);

	void OnJobCompleted();

	void Feed(IInventory inv);

	void Pick(IInventory inv, Item itemType, float quantity);
}
