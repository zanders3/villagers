using UnityEngine;
using System.Collections;
using System.Linq;

// The producer waits at assigned production building (e.g. Poleturner, Fletcher)
// + while (resource on stockpile)
//	+ Paths to nearest resource on stockpile (e.g. wood, stone)
//	+ Picks up that resource
//	+ Paths back to assigned production building
//	+ Produces item (e.g. bow, pike)
//	+ Paths to fullest stockpile for that resource, or nearest empty stockpile
[RequireComponent(typeof(PathingCharacter))]
public class Producer : Coward 
{
	ProductionBuilding building = null;
	
	void Destroy()
	{
		if (building != null)
			building.Villager = null;
	}
	
	protected override IEnumerator RunDaytime()
	{	
		//Find the nearest available production building
		float nearestBuilding = float.MaxValue;
		foreach (ProductionBuilding building in GameObject.FindObjectsOfType(typeof(ProductionBuilding)).Cast<ProductionBuilding>())
		{
			if (building.IsBuilt && building.Villager == null)
			{
				float distance = Vector3.Distance(building.transform.position, transform.position);
				if (distance < nearestBuilding)
				{
					this.building = building;
					nearestBuilding = distance;
				}
			}
		}
		
		//Failed to find a building to produce
		if (building == null)
		{
			currentState = "Failed to find an available production building";
			GetComponent<Villager>().SetMode(Villager.Mode.Builder);
		}
		else
		{
			building.Villager = GetComponent<Villager>();
		}
		
		while (true)
		{
			//Wait at production building
			currentState = "Wait at production building";
			yield return new WaitForSeconds(1.0f);
			yield return StartCoroutine(GoToProductionBuilding());
			
			while (true)
			{
				Stockpile stockpile = Stockpile.GetNearestStockpileWithResource(building.ResourceTaken, transform.position);
				if (stockpile == null)
					break;
				
				//Get resource from stockpile
				currentState = "Get " + building.ResourceTaken + " from stockpile";
				yield return StartCoroutine(MoveToStockpile(stockpile));
				
				int amount = 1;
				stockpile.Withdraw(ref amount, building.ResourceTaken);
				if (amount != 0)
				{
					Debug.Log("Resource already taken, finding another");
					continue;
				}
				
				villager.Inventory.AddItem(building.ResourceTaken);
				
				//Return to production building
				currentState = "Return to production building";
				yield return StartCoroutine(GoToProductionBuilding());
				
				villager.Inventory.ClearItems();
				
				//Produce item
				currentState = "Producing item";
				yield return new WaitForSeconds(GameSettings.ResourceSettings[building.ResourceProduced].TimeToGatherSeconds);
				
				villager.Inventory.AddItem(building.ResourceProduced);
				
				//Deposit item at stockpile
				currentState = "Deposit " + building.ResourceProduced + " at stockpile";
				yield return StartCoroutine(DepositAtStockpile(building.ResourceProduced, GoToProductionBuilding, 1));
				
				villager.Inventory.ClearItems();
			}
		}
	}
	
	private readonly Vector3 targetOffset = new Vector3(0.0f, 0.0f, 1.0f);
	
	IEnumerator GoToProductionBuilding()
	{
		Vector3 targetPos = building.transform.position + targetOffset;
		if (Vector3.Distance(transform.position, targetPos) > 1.0f)
			yield return StartCoroutine(character.PathTo(Mathf.RoundToInt(targetPos.x), Mathf.RoundToInt(targetPos.z) + 1));
		
		character.FinalTarget = targetPos;
		
		while (Vector3.Distance(transform.position, targetPos) > 0.1f)
			yield return new WaitForSeconds(0.1f);
	}
}
