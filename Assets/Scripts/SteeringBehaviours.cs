using UnityEngine;

public class SteeringBehaviours 
{
	#region Declarations and Constructors
	
	public enum Deceleration 
	{
		Slow = 3,
		Normal = 2,
		Fast = 1,
	}
	
	private const float PANIC_DISTANCE = 30.0f;
	private const float DECELERATION_TWEAKER = 100.0f; 
	
	private readonly Boid _boid;
	
	public SteeringBehaviours(Boid boid)
	{
		this._boid = boid;
	}
	
	#endregion
	
	#region Steering Behaviours
	
	Vector2 Arrive(Vector2 targetPosition, Deceleration deceleration)
	{
		var toTarget = (targetPosition - this._boid.Position);
		var distanceToTarget = toTarget.magnitude;
		
		if(distanceToTarget > Mathf.Epsilon)
		{
			var speed = distanceToTarget / ((float)deceleration * DECELERATION_TWEAKER);
			speed = Mathf.Min(speed, this._boid.MaxSpeed); 
			var desiredVelocity = toTarget * speed / distanceToTarget;
			return desiredVelocity - this._boid.Velocity;
		}
		
		return Vector2.zero;
	}
	
	Vector2 Flee(Vector2 scarePoint)
	{
		// Only flee if within panic distance
		if((this._boid.Position - scarePoint).sqrMagnitude > PANIC_DISTANCE*PANIC_DISTANCE)
		{
			return Vector2.zero;
		}
		
		var desiredVelocity = (this._boid.Position - scarePoint).normalized * this._boid.MaxSpeed;
		return desiredVelocity - this._boid.Velocity;
	}
	
	Vector2 Seek(Vector2 targetPosition)
	{
		var desiredVelocity = (targetPosition - this._boid.Position).normalized * this._boid.MaxSpeed;
		return desiredVelocity - this._boid.Velocity;
	}
	
	#endregion
	
	public Vector2 Calculate()
	{
		// Type the Behaviour you wish to test here!
		// TODO: UI interface for testing behaviours
		return Flee(Vector2.zero);
	}
}
