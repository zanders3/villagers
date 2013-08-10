using UnityEngine;
using System.Collections;

//The mayor class implements mayor movement controls and handles mouse clicks
[RequireComponent(typeof(BuildMenu))]
public class Mayor : Character
{
	protected override Vector2 Steer()
	{
		Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
		Vector3 forward = Camera.main.transform.forward;
		Vector3 right = Camera.main.transform.right;
		
		Vector2 cameraRelMove = new Vector2(
			Vector2.Dot(move, new Vector2(right.x, right.z)),
			Vector2.Dot(move, new Vector2(forward.x, forward.z))
		);
		
		if (cameraRelMove.sqrMagnitude > 0.0f)
			return cameraRelMove * MaxForce;
		else
			return -velocity / Time.deltaTime;
	}
	
	protected override void Update()
	{	
		base.Update();
		
		if (Input.GetMouseButtonDown(0))
		{
			if (GetComponent<BuildMenu>().OnMouseDown())
				return;
		}
	}
}
