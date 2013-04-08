using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//The villager script handles the switching of AI behaviour scripts
public class Villager : MonoBehaviour 
{
	public enum Mode
	{
		None,
		Idle,
		Follower,
		Gatherer,
		Producer,
		Soldier
	}
	
	private static Dictionary<Mode, System.Type> ModeToScript = new Dictionary<Mode, System.Type>()
	{
		{ Mode.Idle, typeof(Idle) },
		{ Mode.Follower, typeof(Follower) },
		{ Mode.Gatherer, typeof(Gatherer) },
		{ Mode.Producer, typeof(Producer) },
		{ Mode.Soldier, typeof(Soldier) }
	};
	
	public VillagerItems Inventory;
	
	public House Home;
	private Mode mode = Mode.None;
	
	public Mode InitialMode = Mode.Idle;
	
	void Start()
	{
		Inventory = new VillagerItems(transform);
		
		Stockpile.Resources[ResourceType.Villagers]++;
		SetMode(InitialMode);
	}
	
	void Destroy()
	{
		Stockpile.Resources[ResourceType.Villagers]--;
		if (Home != null)
			Home.RemoveVillager();
	}
	
	public static void SetDaytime(bool isDaytime)
	{
		foreach (VillagerAIMode aiMode in GameObject.FindObjectsOfType(typeof(VillagerAIMode)).Cast<VillagerAIMode>())
			aiMode.SetDaytime(isDaytime);
	}
	
	public void SetMode(Mode mode)
	{
		if (this.mode != mode)
		{
			Inventory.ClearItems();
			
			if (this.mode != Mode.None)
			{
				Component oldMode = GetComponent(ModeToScript[this.mode]);
				Destroy(oldMode);
			}
			
			gameObject.AddComponent(ModeToScript[mode]);
			this.mode = mode;
		}
	}
}
