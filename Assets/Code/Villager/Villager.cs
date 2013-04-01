using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Villager : MonoBehaviour 
{
	public enum Mode
	{
		None,
		Builder,
		Follower,
		Gatherer,
		Producer
	}
	
	private static Dictionary<Mode, System.Type> ModeToScript = new Dictionary<Mode, System.Type>()
	{
		{ Mode.Builder, typeof(Builder) },
		{ Mode.Follower, typeof(Follower) },
		{ Mode.Gatherer, typeof(Gatherer) },
		{ Mode.Producer, typeof(Producer) }
	};
	
	public House Home;
	private Mode mode = Mode.None;
	
	void Start()
	{
		Stockpile.Resources[ResourceType.Villagers]++;
		SetMode(Mode.Builder);
	}
	
	void Destroy()
	{
		Stockpile.Resources[ResourceType.Villagers]--;
		if (Home != null)
			Home.RemoveVillager();
	}
	
	public static void SetDaytime(bool isDaytime)
	{
		foreach (Coward coward in GameObject.FindObjectsOfType(typeof(Coward)).Cast<Coward>())
			coward.SetDaytime(isDaytime);
	}
	
	public void SetMode(Mode mode)
	{
		if (this.mode != mode)
		{
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
