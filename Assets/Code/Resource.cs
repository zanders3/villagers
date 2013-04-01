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
			animation.Play(AnimationPlayMode.Mix);
		
		resourceAmount--;
		if (resourceAmount <= 0)
		{
			Map.Instance.ClearResource(this);
			Destroy(gameObject);
		}
		
		return true;
	}
}
