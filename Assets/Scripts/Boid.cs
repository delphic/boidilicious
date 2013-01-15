using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour 
{
	#region Declarations
	
	private float _mass = 2.0f;
	private Vector2 _velocity = Vector2.zero; // Going to handle velocity myself for now, arguably should use a rigidbody component
	private float _maxSpeed = 10.0f;
	private float _maxForce = 0.1f;
	private float _maxTurnRate = 0.01f;
	private SteeringBehaviours _steeringBehaviours;
	
	// Game World Access to go here!
	
	#endregion
	
	#region Public Accessors
	
	public Vector2 Velocity
	{
		get { return this._velocity; }
	}
	
	public Vector2 Position 
	{
		get { return (Vector2)this.transform.position; }
	}
	
	public float MaxSpeed
	{
		get { return this._maxSpeed; }
	}
	
	#endregion
	
	#region MonoBehaviour Members
	
	void Awake()
	{
		this._steeringBehaviours = new SteeringBehaviours(this);
	}
	
	void Update () 
	{
		// Calculate updated Position
		var steeringForce = this._steeringBehaviours.Calculate();
		var acceleration = steeringForce/_mass;
		this._velocity = this._velocity + (acceleration * Time.deltaTime);
		this._velocity = Vector2.ClampMagnitude(_velocity, _maxSpeed);
		var newPosition = this.transform.position + (Vector3)_velocity;
		
		// Update heading
		if(this._velocity.sqrMagnitude > Mathf.Epsilon) 
		{
			this.transform.LookAt(newPosition);
		}
		
		// Wrap Position
		newPosition = WrapPosition(newPosition);
		
		// Update position
		this.transform.position = newPosition;
	}
	
	#endregion
	
	#region Private Helper Functions
	
	Vector2 WrapPosition(Vector2 position)
	{
		Bounds screenSpace = new Bounds(
			Camera.mainCamera.transform.position, 
			new Vector3(
				2*((float)Screen.width/(float)Screen.height)*Camera.mainCamera.orthographicSize,
				2*Camera.mainCamera.orthographicSize, 
				Mathf.Infinity));
		if(!screenSpace.Contains(position))
		{
			if(position.x < screenSpace.min.x)
			{
				position = position + new Vector2(screenSpace.size.x, 0);
			}
			else if (position.x > screenSpace.max.x)
			{
				position = position - new Vector2(screenSpace.size.x, 0);
			}
			
			if (position.y < screenSpace.min.y)
			{
				position = position + new Vector2(0, screenSpace.size.y);
			}
			else if (position.y > screenSpace.max.y)
			{
				position = position - new Vector2(0, screenSpace.size.y);
			}
		}
		
		return position;
	}
	
	#endregion
}