using UnityEngine;
using UnitySteer;

/// <summary>
/// Vehicle subclass which automatically applies the steering forces from
/// the components attached to the object.  AutonomousVehicle is characterized
/// for the vehicle always moving in the same direction as its forward vector,
/// unlike Bipeds which are able to side-step.
/// </summary>
[AddComponentMenu("UnitySteer/Vehicle/Tank")]
public class TankVehicle : TickedVehicle
{
	#region Internal state values
	float _speed;
	#endregion
	
	public override float Speed
	{
		get { return _speed; }
		set 
		{ 
			_speed = Mathf.Clamp(value, 0, MaxSpeed); 
			DesiredSpeed = _speed;
		}
	}
	
	/// <summary>
	/// Current vehicle velocity
	/// </summary>
	public override Vector3 Velocity
	{
		get
		{
			return Transform.forward * _speed;
		}
		set
		{
			throw new System.NotSupportedException("Cannot set the velocity directly on AutonomousVehicle");
		}
	}	
	
	#region Speed-related methods
	public override void UpdateOrientationVelocity(Vector3 velocity)
	{
		OrientationVelocity = Mathf.Approximately(_speed, 0) ? Transform.forward : velocity.normalized;

		if(Vector3.Angle(Transform.forward, OrientationVelocity)>90)
		{
			Speed = velocity.magnitude/10;
		}
		else
		{
			Speed = velocity.magnitude;
		}

	}
	
	protected override Vector3 CalculatePositionDelta(float deltaTime)
	{
		return Velocity * deltaTime;
	}
	
	protected override void ZeroVelocity()
	{
		Speed = 0;
	}
	#endregion
}

