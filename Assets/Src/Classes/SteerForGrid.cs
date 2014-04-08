#define ANNOTATE_NAVMESH
using UnityEngine;
using UnitySteer;
using UnitySteer.Helpers;
using Pathfinding;

/// <summary>
/// Steers a vehicle to stay on the grid
/// Currently only supports the Default layer
/// </summary>
[AddComponentMenu("UnitySteer/Steer/... for Grid")]
public class SteerForGrid : Steering {
	#region Private fields
	[SerializeField]
	float _avoidanceForceFactor = 0.75f;

	[SerializeField]
	float _minTimeToCollision = 2;
	
	[SerializeField]
	bool _offMeshCheckingEnabled = true;
	
	[SerializeField]
	Vector3 _probePositionOffset = new Vector3(0, 0.2f, 0);
	
	[SerializeField]
	float _probeRadius = 0.1f;

	Vector3 lastWalkableNode, lastUnwalkableNode, lastAvoidance, lastMove;

	// TODO navmesh layer selection -> CustomEditor -> GameObjectUtility.GetNavMeshLayerNames() + Popup
	#endregion
	
	#region Public properties
	/// <summary>
	/// Multiplier for the force applied on avoidance
	/// </summary>
	/// <remarks>If his value is set to 1, the behavior will return an
	/// avoidance force that uses the full brunt of the vehicle's maximum
	/// force.</remarks>
	public float AvoidanceForceFactor {
		get {
			return this._avoidanceForceFactor;
		}
		set {
			_avoidanceForceFactor = value;
		}
	}

	/// <summary>
	/// Minimum time to collision to consider
	/// </summary>
	public float MinTimeToCollision {
		get {
			return this._minTimeToCollision;
		}
		set {
			_minTimeToCollision = value;
		}
	}
	
	/// <summary>
	/// Switch if off-mesh checking should be done or not.
	/// </summary>
	/// <remarks>Off-mesh chekcing, checks if the Vehicle is currently on the navmesh or not.
	/// If not, a force is calculated to bring it back on it.
	/// </remarks>
	public bool OffMeshChecking {
		get {
			return _offMeshCheckingEnabled;
		}
		set {
			_offMeshCheckingEnabled = value;
		}
	}
	
	/// <summary>
	/// Offset where to place the off-mesh checking probe, relative to the Vehicle position
	/// </summary>
	/// <remarks>This should be as close to the navmesh height as possible. Normally 
	/// it's slightly floating above the ground (0.2 with default settings on a simple plain).
	/// </remarks>
	public Vector3 ProbePositionOffset {
		get { 
			return this._probePositionOffset;
		}
		set {
			_probePositionOffset = value;
		}
	}
	
	/// <summary>
	/// Offset where to place the off-mesh checking probe, relative to the Vehicle position
	/// </summary>
	/// <remarks>The radius makes it possible to compensate slight variations in the navmesh
	/// heigh. However, this setting  affects the horizontal tolerance as well. This means,
	/// the larger the radius, the later the vehicle will be considered off mesh.
	/// </remarks>
	public float ProbeRadius {
		get {
			return this._probeRadius;
		}
		set {
			_probeRadius = value;
		}
	}
	#endregion
	
	private int _navMeshLayerMask;
	
	protected override void Start() {
		base.Start();
		_navMeshLayerMask = 1 << NavMesh.GetNavMeshLayerFromName("Default");
	}
	
	
	public override bool IsPostProcess 
	{ 
		get { return true; }
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(lastWalkableNode, 0.3f);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(lastUnwalkableNode, 0.3f);

		Gizmos.color = Color.white;
		Gizmos.DrawLine(Vehicle.Position, Vehicle.Position + lastMove);

		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(Vehicle.Position, Vehicle.Position + lastAvoidance);

	}
	/// <summary>
	/// Calculates the force necessary to stay on the navmesh
	/// </summary>
	/// <returns>
	/// Force necessary to stay on the navmesh, or Vector3.zero
	/// </returns>
	/// <remarks>
	/// If the Vehicle is too far off the navmesh, Vector3.zero is retured.
	/// This won't lead back to the navmesh, but there's no way to determine
	/// a way back onto it.
	/// </remarks>
	protected override Vector3 CalculateForce()
	{
		GridGraph g =  AstarPath.active.astarData.gridGraph;
	




		/*
		 * While we could just calculate line as (Velocity * predictionTime) 
		 * and save ourselves the substraction, this allows other vehicles to
		 * override PredictFuturePosition for their own ends.
		 */
		Vector3 futurePosition = Vehicle.PredictFuturePosition(_minTimeToCollision);
		Vector3 movement = futurePosition - Vehicle.Position;
		NNInfo info = g.GetNearest(futurePosition);

		if (info.node.Walkable == false)
		{

			Vector3 pos = (Vector3)info.node.position;//new Vector3(info.node.position.x/1000,info.node.position.y/1000,info.node.position.z/1000);
			lastUnwalkableNode = pos;
			Debug.LogWarning("Unwalkable node in :"+Vector3.Distance(pos,futurePosition));

			NNConstraint nn = new NNConstraint();
			nn.constrainWalkability = true;
			nn.constrainDistance=false;
			nn.constrainTags=false;
			nn.walkable = true;

			info = g.GetNearestForce(futurePosition,nn);

			lastWalkableNode = (Vector3)info.node.position;

			Vector3 avoidance = ((Vector3)info.node.position)-Vehicle.Position;

			Vector3 moveDirection = Vehicle.Velocity.normalized;

			avoidance=OpenSteerUtility.perpendicularComponent(avoidance, moveDirection);
			avoidance.Normalize();

			avoidance *= Vehicle.MaxForce * _avoidanceForceFactor;
			lastAvoidance = avoidance;
			lastMove = moveDirection;

			return avoidance;

		}

		return Vector3.zero;


	}	
}
