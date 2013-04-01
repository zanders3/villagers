using UnityEngine;
using System.Collections;
using System.Linq;

//The gatherer gathers wood/stone from trees/rocks and then places them in a stockpile.
[RequireComponent(typeof(PathingCharacter))]
public class Gatherer : Coward
{
	public ResourceType Type = ResourceType.Wood;
	
	protected override IEnumerator RunDaytime()
	{
		Tile tileToGather;
		switch (Type)
		{
		case ResourceType.Stone:
			tileToGather = Tile.Rock;
			break;
		case ResourceType.Wood:
		default:
			tileToGather = Tile.Tree;
			break;
		}
		
		int resourceCarried = 0;
		
		while (true)
		{
			//Is there a stockpile with available space?
			while (Stockpile.GetFullestStockpile(Type) == null)
			{
				currentState = "No stockpiles with space!";
				yield return StartCoroutine(WaitByCampfire());
			}
			
			//1. path towards the nearest resource
			currentState = "Path to resource";
			yield return StartCoroutine(character.PathTo(
				(tx,ty) => Map.Instance.HasNeighbour(tx, ty, tileToGather),
				(cx, cy) => 0,
				Vector2.zero
			));
			
			currentState = "Finding tile to gather from";
			
			//Find the tile to gather from
			Resource resource = Map.Instance.GetNearbyResource(
				Mathf.RoundToInt(character.FinalTarget.x), 
				Mathf.RoundToInt(character.FinalTarget.z),
				tileToGather
			);
			if (resource == null)
			{
				currentState = "Pathing failed!";
				yield return new WaitForSeconds(1.0f);
				continue;
			}
			
			//2. Gather from the resource
			int amountToGather = GameSettings.ResourceSettings[Type].ResourceCarried;
			float timeToGatherSeconds = GameSettings.ResourceSettings[Type].TimeToGatherSeconds;
			character.FinalTarget = resource.transform.position;
			for (int i = 0; i<amountToGather; i++)
			{
				currentState = "Gather from resource (" + i + "/" + amountToGather + ")";
				if (resource.Gather())
				{
					yield return new WaitForSeconds(timeToGatherSeconds);
					resourceCarried++;
				}
				else
					break;				
			}
			
			//3. Find and path to a stockpile
			currentState = "Deposit at stockpile";
			yield return StartCoroutine(DepositAtStockpile(Type, WaitByCampfire, resourceCarried));
			resourceCarried = 0;
		}
	}
}
