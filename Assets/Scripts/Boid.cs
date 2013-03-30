using UnityEngine;
using System.Collections;

public class Boid : MonoBehaviour 
{
	#region Declarations
	
	public SteeringControllers initialSteeringControllerType = SteeringControllers.SingleBehaviour; // Controller Injection, to be used with prefabs only
	private float _mass = 2.0f;
	private Vector2 _velocity = Vector2.zero;	// Going to handle velocity myself for now, arguably should use a rigidbody component
	private float _maxSpeed = 1.0f;
	//private float _maxForce = 0.1f; 			// Future Variables!
	//private float _maxTurnRate = 0.01f;		// Future Variables!
	private ISteeringController _steeringController;
	
	// Game World Access to go here!
	
	#endregion
	
	#region Public Accessors
	
	public ISteeringController Controller
	{
		get { return this._steeringController; }
	}
	
	public Vector2 Heading
	{
		get { return this.transform.forward; }
	}
	
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
	
	public float Mass
	{
		get { return this._mass; }
	}
	
	#endregion
	
	#region MonoBehaviour Members
	
	void Awake()
	{
		// TODO: ideally would outsource this creation
		switch(initialSteeringControllerType)
		{
			case SteeringControllers.SingleBehaviour:
				this._steeringController = new SingleBehaviourSteeringController(new SteeringBehaviours(this));
				break;
		}
	}
	
	void Update () 
	{
		// Calculate updated Position
		var steeringForce = this._steeringController.Calculate();
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
	
	#region Set Methods 
	// These are for variables we're only exposing for the purposes of playing with in realtime to 
	// settle upon a feel - ideally they'd be completely private, hence I'm using methods rather
	// than public varaiables or accessors
	
	void SetMass(float mass)
	{
		this._mass = mass;
	}
	
	void SetMaxSpeed(float maxSpeed)
	{
		this._maxSpeed = maxSpeed;
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
