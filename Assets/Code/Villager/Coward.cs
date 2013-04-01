using UnityEngine;
using System.Collections;

//The coward returns home at nightfall, and runs away from scary characters. This is the base class for most characters.
[RequireComponent(typeof(PathingCharacter), typeof(Villager))]
public abstract class Coward : MonoBehaviour
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
	
	private IEnumerator RunNighttime()
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
			currentState = "Homeless - waiting by campfire";
			yield return StartCoroutine(WaitByCampfire());
		}
	}
	
	protected abstract IEnumerator RunDaytime();
	
	void OnGUI()
	{
		if (!string.IsNullOrEmpty(currentState))
		{
			Vector3 screenPos = Camera.mainCamera.WorldToScreenPoint(transform.position);
			GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200.0f, 20.0f), currentState);
		}
	}
	
	protected IEnumerator WaitByCampfire()
	{
		//Wait by the campfire
		Vector3 targetPos = Building.TownHall.transform.position + new Vector3(0.5f, 0.0f, 2.0f);
		
		if (Vector3.Distance(targetPos, transform.position) > 2.0f)
			yield return StartCoroutine(GetComponent<PathingCharacter>().PathTo(Building.TownHall.Tx + Random.Range(0, 2), Building.TownHall.Ty + Random.Range(2,5)));
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
