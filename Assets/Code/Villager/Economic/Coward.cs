using UnityEngine;
using System.Collections;

//The coward returns home at nightfall, and runs away from scary characters. This is the base class for the economic villagers (gatherers, builders, etc)
public abstract class Coward : VillagerAIMode
{
	protected override IEnumerator RunNighttime()
	{
		if (villager.Home != null)
		{
			currentState = "Returning home";
			
			Vector3 targetPos = villager.Home.transform.position;
			yield return StartCoroutine(character.PathTo(Mathf.FloorToInt(targetPos.x), Mathf.FloorToInt(targetPos.z) + villager.Home.GetComponent<Building>().Height));
			
			currentState = "Going inside";
			character.FinalTarget = targetPos;
		}
		else
		{
			yield return StartCoroutine(WaitByCampfire("Homeless"));
		}
	}
}
