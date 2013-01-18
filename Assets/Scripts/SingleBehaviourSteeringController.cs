using UnityEngine;
using Deceleration = SteeringBehaviours.Deceleration;

// Class for configuring on the fly what steering behaviours is in use and with what parameters
// This is for observing / playing with individual behaviours
public class SingleBehaviourSteeringController : ISteeringController
{	
	public enum SteeringBehaviour 
	{
		Arrive,
		Evade,
		Flee,
		Pursuit,
		Seek,
		Wander,
	}
	
	public SteeringBehaviours steeringBehaviours; // Public so we don't have to expose the variables for configuration
	public SteeringBehaviour currentBehaviour = SteeringBehaviour.Wander;
	public Vector2 targetPosition = Vector2.zero;
	public Vector2 fleePosition = Vector2.zero;
	public Deceleration deceleration = Deceleration.Fast;
	public Boid boidToEvade = null;
	public Boid boidToPursue = null;
		
	public SingleBehaviourSteeringController(SteeringBehaviours steeringBehaviours)
	{
		this.steeringBehaviours = steeringBehaviours;
	}
	
	public Vector2 Calculate()
	{
		switch(currentBehaviour)
		{
		case SteeringBehaviour.Arrive:
			return this.steeringBehaviours.Arrive(targetPosition, deceleration);
		case SteeringBehaviour.Evade:
			return this.steeringBehaviours.Evade(boidToEvade);
		case SteeringBehaviour.Flee:
			return this.steeringBehaviours.Flee(fleePosition);
		case SteeringBehaviour.Pursuit:
			return this.steeringBehaviours.Pursuit(boidToPursue);
		case SteeringBehaviour.Seek:
			return this.steeringBehaviours.Seek(targetPosition);
		case SteeringBehaviour.Wander:
			return this.steeringBehaviours.Wander();
		default:
			return Vector2.zero;
		}
	}	
}
