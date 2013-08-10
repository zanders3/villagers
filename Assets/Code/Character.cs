using UnityEngine;
using System.Collections;

public abstract class Character : MonoBehaviour 
{
	public float MaxForce = 100;
	public float MaxSpeed = 1.0f;
	
	public Vector2 velocity;
	
	protected abstract Vector2 Steer();
	
    void Start()
    {
        IsAssigned.RegisterAssignment(gameObject);
    }

	protected virtual void Update()
	{
		Vector2 steeringForce = Vector2.ClampMagnitude(Steer(), MaxForce);
		rigidbody.AddForce(new Vector3(steeringForce.x, 0.0f, steeringForce.y));
		velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.z);
		
		Vector2 overSpeed = Vector2.ClampMagnitude(velocity, MaxSpeed) - velocity;
		rigidbody.AddForce(new Vector3(overSpeed.x, 0.0f, overSpeed.y) / Time.deltaTime);
	}
}
