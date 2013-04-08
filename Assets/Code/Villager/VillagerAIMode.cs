using UnityEngine;
using System.Linq;
using System.Collections;

//Base class for all villager AI modes. Contains methods that implemented behaviours that villagers share.
[RequireComponent(typeof(PathingCharacter), typeof(Villager))]
public abstract class VillagerAIMode : MonoBehaviour
{
	protected PathingCharacter character;
	protected Villager villager;
	protected string currentState = string.Empty;
	
	void Start()
	{
		villager = GetComponent<Villager>();
		character = GetComponent<PathingCharacter>();
		
		StartCoroutine(RunDaytime());
	}
	
	public void SetDaytime(bool isDaytime)
	{
		StopAllCoroutines();
		if (isDaytime)
			StartCoroutine(RunDaytime());
		else
			StartCoroutine(RunNighttime());
	}
	
	protected abstract IEnumerator RunNighttime();
	
	protected abstract IEnumerator RunDaytime();
	
	void OnGUI()
	{
		if (!string.IsNullOrEmpty(currentState))
		{
			Vector3 screenPos = Camera.mainCamera.WorldToScreenPoint(transform.position);
			GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200.0f, 20.0f), currentState);
		}
	}
	
	protected IEnumerator WaitByCampfire(string parentMode)
	{
		yield return new WaitForSeconds(1.0f);
		
		while (true)
		{
			//Is there any building work available?
			float nearestBuilding = float.MaxValue;
			Building buildingToBuild = null;
			foreach (Building building in GameObject.FindSceneObjectsOfType(typeof(Building)).Cast<Building>())
			{
				if (!building.IsBuilt)
				{
					float distance = Vector3.Distance(building.transform.position, transform.position);
					if (distance < nearestBuilding)
					{
						buildingToBuild = building;
						nearestBuilding = distance;
					}
				}
			}
			
			if (buildingToBuild == null)
			{
				currentState = parentMode + " - Waiting by campfire";
				//Wait by the campfire
				Vector3 targetPos = Building.TownHall.transform.position + new Vector3(0.5f, 0.0f, 2.0f);
				
				if (Vector3.Distance(targetPos, transform.position) > 2.0f)
					yield return StartCoroutine(GetComponent<PathingCharacter>().PathTo(Building.TownHall.Tx + Random.Range(0, 2), Building.TownHall.Ty + Random.Range(2,5)));
				yield return new WaitForSeconds(1.0f);
			}
			else
			{
				currentState = parentMode + " - Moving to building";
				//Go to the building
				yield return StartCoroutine(character.PathToBuilding(buildingToBuild));
				
				//Build it
				currentState = parentMode + " - Building";
				while (!buildingToBuild.IsBuilt)
				{
					buildingToBuild.Build();
					yield return new WaitForSeconds(2.0f);
				}
			}
		}
	}
	
	protected IEnumerator MoveToStockpile(Stockpile stockpile)
	{
		int tx = Mathf.RoundToInt(stockpile.transform.position.x), ty = Mathf.RoundToInt(stockpile.transform.position.z);
		yield return StartCoroutine(GetComponent<PathingCharacter>().PathTo((x,y) =>
			{
				return Mathf.Abs(tx - x) == 1 && Mathf.Abs(ty - y) == 1;
			},
			(cx, cy) => (Mathf.Abs(tx - cx) + Mathf.Abs(ty - cy)) * 10,
			new Vector2(stockpile.transform.position.x, stockpile.transform.position.z)
		));
		
		int timeSpentWaiting = 0;
		while (Vector3.Distance(transform.position, stockpile.transform.position) > 1.0f)
		{
			yield return new WaitForSeconds(0.1f);
			
			timeSpentWaiting++;
			if (timeSpentWaiting > 40)
				break;
		}
		
		yield return new WaitForSeconds(1.0f);
	}
	
	protected IEnumerator DepositAtStockpile(ResourceType type, System.Func<IEnumerator> noStockpileSpace, int resourceAmount)
	{
		Stockpile currentStockpile = null;
		while (currentStockpile == null)
		{
			currentStockpile = Stockpile.GetFullestStockpile(type);
			
			//No empty stockpiles available!
			if (currentStockpile == null)
			{
				yield return StartCoroutine(noStockpileSpace());
			}
			else
			{
				//Path to the current stockpile
				yield return StartCoroutine(MoveToStockpile(currentStockpile));
				
				//Deposit stuff
				currentStockpile.Deposit(ref resourceAmount, type);
				if (resourceAmount > 0)
					currentStockpile = null;
			}
		}
	}
}
