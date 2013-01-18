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
	
	private const float TURN_AROUND_COEFFICIENT = 0.5f;
	private const float DECELERATION_TWEAKER = 100.0f; 
	
	private float _panicDistance = 30.0f;
	private float _wanderRadius = 8.0f;
	private float _wanderDistance = 8.0f;
	private float _wanderJitter = 1.5f;
	private Vector2 _wanderTarget = Vector2.zero;
	
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
	
	Vector2 Evade(Boid scaryBoid)
	{
		var toPursuer = scaryBoid.Position - this._boid.Position;
		var lookAheadTime = toPursuer.magnitude / (this._boid.MaxSpeed + scaryBoid.Velocity.magnitude);
		return Flee(scaryBoid.Position + scaryBoid.Velocity * lookAheadTime);
	}
	
	Vector2 Flee(Vector2 scarePoint)
	{
		// Only flee if within panic distance
		if((this._boid.Position - scarePoint).sqrMagnitude > _panicDistance*_panicDistance)
		{
			return Vector2.zero;
		}
		
		var desiredVelocity = (this._boid.Position - scarePoint).normalized * this._boid.MaxSpeed;
		return desiredVelocity - this._boid.Velocity;
	}
	
	Vector2 Pursuit(Boid targetBoid)
	{
		var toTarget = targetBoid.Position - this._boid.Position;
		var relativeHeadingAngle = Vector2.Dot(this._boid.Heading, targetBoid.Heading);
		
		if(Vector2.Dot(toTarget, this._boid.Heading) > 0 && relativeHeadingAngle < -0.95) // acos(-0.95) = 18 degrees 
		{
			// Target is is ahead and is facing the agent, seek directly to the boid
			return Seek(targetBoid.Position);
		}
		// Else we predict where the targetBoid is heading
		var lookAheadTime = toTarget.magnitude / (this._boid.MaxSpeed + targetBoid.Velocity.magnitude); 
		//lookAheadTime += TurnAroundTime(this._boid, targetBoid.Position);	
		return Seek(targetBoid.Position + targetBoid.Velocity * lookAheadTime);
	}
	
	Vector2 Seek(Vector2 targetPosition)
	{
		var desiredVelocity = (targetPosition - this._boid.Position).normalized * this._boid.MaxSpeed;
		return desiredVelocity - this._boid.Velocity;
	}
	
	Vector2 Wander()
	{
		// Jitter the wander target - this might be better if we moved around the circle, rather than randomly and then projected back to the circle
		_wanderTarget = _wanderTarget + new Vector2(2*(Random.value-0.5f)*_wanderJitter, 2*(Random.value-0.5f)*_wanderJitter);
		// Project onto circle
		_wanderTarget = _wanderRadius * _wanderTarget.normalized;
		// Project in front of boid by _wanderDistance 
		var worldTarget = _wanderTarget + _wanderDistance * this._boid.Heading + this._boid.Position;
		/// Steer towards it
		return worldTarget - this._boid.Position;	
	}
	
	#endregion
	
	public Vector2 Calculate()
	{
		// Type the Behaviour you wish to test here!
		// TODO: UI interface for testing behaviours
		return Wander();
	}
	
	#region Helper Functions
	
	float TurnAroundTime(Boid boid, Vector2 targetPosition)
	{
		// Note the TURN_AROUND_COEFFICIENT has 0.5 => 1 second turn around time
		// TODO: Ideally we would calculate this from MaxForce versus mass of the boid, rather than this fudge
		
		// The dot product gives a value of 1 if the target is directly ahead and -1 if it is directly behind.
		// Subtracting 1 and multiplying by the negative of the coefficient gives a positive value proportional
		// to the rotational displacement of the vehicle and target.
		return (Vector2.Dot(boid.Heading, (targetPosition - boid.Position).normalized) - 1.0f) * -TURN_AROUND_COEFFICIENT;
	}
	
	#endregion
}
