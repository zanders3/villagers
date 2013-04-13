using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public enum ResourceType
{
	None,
	Wood,
	Stone,
	VillagerLimit,
	Villagers,
	Pike
}

public class Stockpile : Building 
{
	public static Dictionary<ResourceType, int> Resources = new Dictionary<ResourceType, int>()
	{
		{ ResourceType.Stone, 0 },
		{ ResourceType.Wood, 50 },
		{ ResourceType.VillagerLimit, 0 },
		{ ResourceType.Villagers, 0 },
		{ ResourceType.Pike, 1 }
	};
	
	public ResourceType ResourceType
	{
		get { return type; }
	}

	private ResourceType type = ResourceType.None;
	private int amount = 0;
	private Stack<Transform> resourcePrefabs = new Stack<Transform>();
	
	void Destroy()
	{
		if (type != ResourceType.None)
			Resources[type] -= amount;
	}
	
	public int MaxStorage
	{
		get
		{
			return type == ResourceType.None ? int.MaxValue : GameSettings.ResourceSettings[type].ResourcePerStockpileBlock * 8;
		}
	}
	
	public bool IsFull
	{
		get 
		{
			return this.amount >= MaxStorage; 
		}
	}
	
	public bool CanDeposit(ResourceType type)
	{
		return IsBuilt && (this.type == type || this.type == ResourceType.None) && !IsFull;
	}
	
	public void Deposit(ref int amount, ResourceType type)
	{
		if (this.type == ResourceType.None)
			this.type = type;
		else if (this.type != type)
			return;
		
		int amountToDeposit = Mathf.Min(amount, MaxStorage - this.amount);
		this.amount += amountToDeposit;
		Resources[type] += amountToDeposit;
		amount -= amountToDeposit;
		
		UpdateResourcePrefabs();
	}
	
	public void Withdraw(ref int amount, ResourceType type)
	{
		if (this.type != type || amount == 0)
			return;
		
		int amountToWithdraw = Mathf.Min(this.amount, amount);
		amount -= amountToWithdraw;
		this.amount -= amountToWithdraw;
		Resources[type] -= amountToWithdraw;
		
		UpdateResourcePrefabs();
		
		if (this.amount <= 0)
			this.type = ResourceType.None;
	}
	
	public static void WithdrawResource(int amount, ResourceType type)
	{
		foreach (Stockpile stockpile in GameObject.FindObjectsOfType(typeof(Stockpile)).Cast<Stockpile>())
		{
			stockpile.Withdraw(ref amount, type);
			if (amount <= 0)
				return;
		}
		
		//This can happen because you can have starting resources that don't physically exist in the world! D:
		if (amount > 0)
			Resources[type] -= amount;
	}
	
	public static void DepositResource(int amount, ResourceType type)
	{
		foreach (Stockpile stockpile in GameObject.FindObjectsOfType(typeof(Stockpile)).Cast<Stockpile>())
		{
			stockpile.Deposit(ref amount, type);
			if (amount <= 0)
				return;
		}
		
		Resources[type] += amount;
	}
	
	private void UpdateResourcePrefabs()
	{
		if (type == ResourceType.None)
			return;
		
		int numPrefabsRequired = this.amount > 0 ? this.amount / GameSettings.ResourceSettings[type].ResourcePerStockpileBlock : 0;
		int deltaNumRequired = numPrefabsRequired - resourcePrefabs.Count;
		
		if (deltaNumRequired > 0)
		{
			GameObject resourcePrefab = (GameObject)UnityEngine.Resources.Load(GameSettings.ResourceSettings[type].ResourcePrefab);
			while (deltaNumRequired > 0)
			{
				Vector3 resourcePrefabOffset = new Vector3((resourcePrefabs.Count % 4) / 2, resourcePrefabs.Count / 4, resourcePrefabs.Count % 2) * 0.5f;
				
				GameObject newPrefab = ((GameObject)GameObject.Instantiate(resourcePrefab, transform.position + resourcePrefabOffset, Quaternion.identity));
				resourcePrefabs.Push(newPrefab.transform);
				newPrefab.transform.parent = this.transform;
				deltaNumRequired--;
			}
		}
		else
		{
			while (deltaNumRequired < 0)
			{
				Destroy(resourcePrefabs.Pop().gameObject);
				deltaNumRequired++;
			}
		}
	}
	
	public static Stockpile GetFullestStockpile(ResourceType type)
	{
		Stockpile selectedStockpile = null;
		int fullestSoFar = 0;
		
		foreach (Stockpile stockpile in GameObject.FindObjectsOfType(typeof(Stockpile)).Cast<Stockpile>())
		{
			if (stockpile.CanDeposit(type))
			{
				if (fullestSoFar <= stockpile.amount)
				{
					fullestSoFar = stockpile.amount;
					selectedStockpile = stockpile;
				}
			}
		}
		
		return selectedStockpile;
	}
	
	public static Stockpile GetNearestStockpileWithResource(ResourceType type, Vector3 position)
	{
		float nearestStockpile = float.MaxValue;
		Stockpile selectedStockpile = null;
		
		foreach (Stockpile stockpile in GameObject.FindObjectsOfType(typeof(Stockpile)).Cast<Stockpile>())
		{
			if (stockpile.ResourceType == type)
			{
				float distance = Vector3.Distance(stockpile.transform.position, position);
				if (distance < nearestStockpile)
				{
					nearestStockpile = distance;
					selectedStockpile = stockpile;
				}
			}
		}
		
		return selectedStockpile;
	}
	
	void OnGUI()
	{
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 200.0f, 20.0f), "Storing " + type + (this.amount > 0 ? (" " + this.amount + " / " + MaxStorage) : string.Empty));
	}
}
