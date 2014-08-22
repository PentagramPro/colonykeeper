using UnityEngine;
using System.Collections;

public interface IWorker  {


	void SetCallbacks(IJob job);

	void CancelCurrentJob();

	void DriveTo(Vector3 dest);
	void DriveTo(Vector3 dest, Collider collider);

	BlockController.DigResult Dig(BlockController block);

	bool Unload();

	bool Load(PileRequest loadRequest);

	void OnJobCompleted();

	void Feed(IInventory inv);

	void Pick(IInventory inv, PileRequest pickRequest);


}
