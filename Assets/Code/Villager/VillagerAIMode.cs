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

	private bool isFollowing = false;

	void Start()
	{
		villager = GetComponent<Villager>();
		character = GetComponent<PathingCharacter>();

		OnStateChange();
	}
	
	public void OnStateChange()
	{
		StopAllCoroutines();

		if (isFollowing)
			StartCoroutine(FollowMayor());
		else if (DayNightCycle.IsDaytime)
			StartCoroutine(RunDaytime());
		else
			StartCoroutine(RunNighttime());
	}

	public void SetFollowing(bool isFollowing)
	{
		this.isFollowing = isFollowing;
		OnStateChange();
	}
	
	protected abstract IEnumerator RunNighttime();
	
	protected abstract IEnumerator RunDaytime();
	
	void OnGUI()
	{
		if (!string.IsNullOrEmpty(currentState))
		{
			Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
			GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200.0f, 20.0f), currentState);
		}
	}
	
	protected IEnumerator WaitByCampfire(string parentMode)
	{
		//Is there any building work available?
		float nearestBuilding = float.MaxValue;
		Building buildingToBuild = null;
		foreach (Building building in GameObject.FindObjectsOfType<Building>())
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
				yield return new WaitForSeconds(GameSettings.ConstructionHealthPerSecond);
			}
		}

		yield return new WaitForSeconds(1.0f);
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
		while (Vector3.Distance(transform.position, stockpile.transform.position) > 0.8f)
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

	private IEnumerator FollowMayor()
	{
        currentState = "Following mayor";
        Mayor mayor = (Mayor)GameObject.FindObjectOfType(typeof(Mayor));

		while (true) 
		{
			//Villagers try to follow about 1 tile away
			Vector3 direction = (mayor.transform.position - transform.position).normalized;
			character.FinalTarget = mayor.transform.position - direction;

			if (Vector3.Distance(villager.transform.position, mayor.transform.position) > 1.5f) 
			{
				yield return StartCoroutine(character.PathTo(Mathf.RoundToInt(mayor.transform.position.x), Mathf.RoundToInt(mayor.transform.position.z)));
				yield return new WaitForSeconds(0.2f);
			}

			yield return new WaitForEndOfFrame();
		}
	}
}
