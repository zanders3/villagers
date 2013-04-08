using UnityEngine;
using System.Collections.Generic;

public class MayorCommands : MonoBehaviour
{
	private List<Villager> followingVillagers = new List<Villager>();
	private bool guiClicked = false;
	
	void OnGUI()
	{
		if (followingVillagers.Count > 0 && GUI.Button(new Rect(0.0f, Screen.height - 50.0f, 50.0f, 50.0f), "Dismiss Followers"))
		{
			guiClicked = true;
			
			foreach (Villager villager in followingVillagers)
				villager.SetMode(Villager.Mode.Idle);
			
			followingVillagers.Clear();
		}
	}
	
	public bool OnMouseDown()
	{
		if (guiClicked)
		{
			guiClicked = false;
			return true;
		}
		
		RaycastHit hit;
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(mouseRay, out hit))
		{
			{
				Villager villager = hit.collider.GetComponent<Villager>();
				if (villager != null)
				{
					if (!followingVillagers.Contains(villager))
					{
						villager.SetMode(Villager.Mode.Follower);
						followingVillagers.Add(villager);
					}
					return true;
				}
			}
			
			if (followingVillagers.Count == 0)
				return false;
			
			Building building = GetComponentInHierarchy<Building>(hit.collider);
			if (building != null)
			{
				Debug.Log("Clicked a building: " + building.BuildingType);
				if (!building.IsBuilt)
				{
					PopVillager().SetMode(Villager.Mode.Idle);
					return true;
				}
				else if (building is ProductionBuilding)
				{
					ProductionBuilding prodBuilding = (ProductionBuilding)building;
					if (prodBuilding.Villager == null)
					{
						PopVillager().SetMode(Villager.Mode.Producer);
						return true;
					}
				}
				else if (building.BuildingType == Tile.Stockpile)
				{
					
				}
			}
			
			Resource resource = GetComponentInHierarchy<Resource>(hit.collider);
			if (resource != null)
			{
				//Debug.Log("Found a resource!");
				Villager villager = PopVillager();
				villager.SetMode(Villager.Mode.Gatherer);
				villager.GetComponent<Gatherer>().Type = resource.Type;
				return true;
			}
		}
		
		return false;
	}
	
	private T GetComponentInHierarchy<T>(Collider collider) where T : Component
	{
		T component = collider.GetComponent<T>();
		if (component == null && collider.transform.parent != null)
			component = collider.transform.parent.GetComponent<T>();
		return component;
	}
	
	private Villager PopVillager()
	{
		Villager villager = followingVillagers[0];
		followingVillagers.RemoveAt(0);
		return villager;
	}
}
