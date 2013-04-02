using UnityEngine;
using System.Collections;

//Soldiers gather at guard posts or the town campfire.
public class Soldier : VillagerAIMode
{
	public ResourceType Weapon = ResourceType.None;
	private bool hasWeapon = false;
	
	protected override IEnumerator RunDaytime()
	{
		//Day and night behaviour are the same
		yield return StartCoroutine(RunNighttime());
	}
	
	void Destroy()
	{
		if (hasWeapon)
			Stockpile.DepositResource(1, Weapon);
	}
	
	protected override IEnumerator RunNighttime()
	{
		yield return new WaitForSeconds(1.0f);
		
		if (!hasWeapon)
		{
			//Take a weapon from a nearby stockpile
			currentState = "Getting a " + Weapon;
			Stockpile stockpile = null;
			while (stockpile == null)
			{
				stockpile = Stockpile.GetNearestStockpileWithResource(Weapon, transform.position);
				if (stockpile == null)
					villager.SetMode(Villager.Mode.Builder);
					
				yield return MoveToStockpile(stockpile);
				
				int amount = 1;
				stockpile.Withdraw(ref amount, Weapon);
				if (amount == 1)
				{
					stockpile = null;
					continue;
				}
				else
				{
					hasWeapon = true;
					villager.Inventory.AddItem(Weapon);
				}
			}
		}
		
		//Wait by the campfire
		currentState = "Soldier - Waiting by campfire";
		yield return WaitByCampfire();
	}
}
