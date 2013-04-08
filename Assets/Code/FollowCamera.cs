using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour 
{
	public Vector3 offset;
	public Mayor Mayor;
	
	void LateUpdate() 
	{
		Vector2 targetPos = new Vector2(Mayor.transform.position.x, Mayor.transform.position.z);
		
		Vector2 currentPos = new Vector2(transform.position.x - offset.x, transform.position.z - offset.z);
		currentPos += (targetPos - currentPos) * Time.deltaTime;
		transform.position = new Vector3(currentPos.x, 0.0f, currentPos.y) + offset;
	}
}
