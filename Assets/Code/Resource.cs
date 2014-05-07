using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour 
{
	private int resourceAmount = 5;
	
	public bool Gather()
	{
		if (resourceAmount <= 0)
			return false;
		
		if (animation != null)
			animation.Play(PlayMode.StopSameLayer);
		
		resourceAmount--;
		if (resourceAmount <= 0)
		{
			Map.Instance.ClearResource(this);
		}
		
		return true;
	}
}
