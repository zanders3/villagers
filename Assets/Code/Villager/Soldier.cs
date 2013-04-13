using UnityEngine;
using System.Collections;

//Soldiers gather at guard posts or the town campfire.
public class Soldier : VillagerAIMode
{
	public ResourceType Weapon = ResourceType.Pike;
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
                {
                    if (Stockpile.Resources[Weapon] > 0)
                    {
                        hasWeapon = true;
                        villager.Inventory.AddItem(Weapon);
                        break;
                    }
                    else
    					villager.SetMode(Villager.Mode.Idle);
                }
					
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
                    break;
				}
			}
		}
		
		//Wait by the campfire
		while (true)
			yield return StartCoroutine(WaitByCampfire("Soldier"));
	}
}
