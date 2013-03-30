using UnityEngine;

public interface ISteeringController
{
	Vector2 Calculate();
}

// This can be used in conjunction with prefabs / editor  
// configuration to allow a degree of dependency injection
// even with MonoBehaviour classes
public enum SteeringControllers
{
	SingleBehaviour,
}