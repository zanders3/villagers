using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour 
{
	public ResourceType Type = ResourceType.Wood;
	
	private int resourceAmount;
	
	void Start()
	{
		resourceAmount = GameSettings.ResourceSettings[Type].ResourcePerTile;
	}
	
	public bool Gather()
	{
		if (resourceAmount <= 0)
			return false;
		
		if (animation != null)
			animation.Play();
		
		resourceAmount--;
		if (resourceAmount <= 0)
		{
			Map.Instance.ClearResource(this);
		}
		
		return true;
	}
}