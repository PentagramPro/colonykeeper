using UnityEngine;
using System.Collections;

public interface IWorker  {


	void SetCallbacks(IJob job);

	void CancelCurrentJob();

	void DriveTo(Vector3 dest);

	BlockController.DigResult Dig();

	void Unload();

	void Load(Item itemType, float maxQuantity);

	void OnJobCompleted();
}
