using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PathingCharacter : Character 
{
	public bool MovingToPath = false;
	private List<PathNode> path = new List<PathNode>();
	private int mapVersion = -1;
	public Vector3 FinalTarget;
	
	void Start()
	{
		FinalTarget = transform.position;
	}
	
	protected override Vector2 Steer()
	{
		Vector3 direction = path.Count > 0 ? (path[0].Position - transform.position) : (FinalTarget - transform.position);
		direction.y = 0.0f;
		
		float distance = direction.magnitude;
		if (distance < 1.0f && path.Count > 0)
			path.RemoveAt(0);
		
		Vector2 desiredVelocity = (new Vector2(direction.x, direction.z) * MaxSpeed) - (velocity * 0.4f);
		return desiredVelocity * MaxForce;
	}
	
	public IEnumerator PathTo(int tx, int ty)
	{
		//Debug.Log("Pathing to: " + tx + ", " + ty);
		FinalTarget = new Vector3(tx, 0.0f, ty);
		
		int ctx = Mathf.RoundToInt(transform.position.x), cty = Mathf.RoundToInt(transform.position.z);
		//Have we already arrived?
		if (tx == ctx && ty == cty)
		{
			//Debug.Log("Already arrived");
			MovingToPath = true;
			yield break;
		}
		yield return StartCoroutine(PathTo(
			(cx, cy) => cx == tx && cy == ty,
			(cx, cy) => (Math.Abs(tx - cx) + Math.Abs(ty - cy)) * 10,
			new Vector2(tx, ty)
		));
	}
	
	public IEnumerator PathTo(Func<int,int,bool> foundTile, Func<int,int,int> calculateCost, Vector2 finalTarget)
	{
		while (Map.Instance == null)
			yield return new WaitForEndOfFrame();
		
		FinalTarget = finalTarget.ToVector3();
		
		//Path to the new location
		Action repath = () =>
		{
			mapVersion = Map.Instance.MapVersion;
			path = Map.Instance.Pathfinder.PathTo(
				Mathf.RoundToInt(transform.position.x),
				Mathf.RoundToInt(transform.position.z),
				foundTile,
				calculateCost
			);
			//Debug.Log("Path: " + path.Count);
			MovingToPath = path.Count > 0;
			
			if (FinalTarget == Vector3.zero)
				FinalTarget = path[path.Count-1].Position;
		};
		
		repath();
		
		while (path.Count > 1)
		{
			yield return new WaitForSeconds(0.5f);
			
			if (Map.Instance.MapVersion != mapVersion)
				repath();
		}
	}
	
	public IEnumerator PathToBuilding(Building building)
	{
		int tx = building.Tx, ty = building.Ty;
		yield return StartCoroutine(PathTo((x,y) =>
			{
				return Mathf.Abs(tx - x) == 1 && Mathf.Abs(ty - y) == 1;
			},
			(cx, cy) => (Mathf.Abs(tx - cx) + Mathf.Abs(ty - cy)) * 10,
			new Vector2(building.transform.position.x, building.transform.position.z)
		));
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		for (int i = 0; i<path.Count; i++)
			Gizmos.DrawCube(path[i].Position + new Vector3(0.0f, 0.3f, 0.0f), new Vector3(0.1f, 0.1f, 0.1f));
		for (int i = 1; i<path.Count; i++)
			Gizmos.DrawLine(path[i-1].Position + new Vector3(0.0f, 0.3f, 0.0f), path[i].Position + new Vector3(0.0f, 0.3f, 0.0f));
	}
}
