using UnityEngine;
using System.Collections.Generic;

//Tracks use of items that the villager is carrying, displaying it visually. Swords, Rocks, Trees, ALL THE THINGS.
public class VillagerItems
{
	private Transform parent;
	private Dictionary<ResourceType, GameObject> inventory = new Dictionary<ResourceType, GameObject>();
	
	public VillagerItems(Transform parent)
	{
		this.parent = parent;
	}
	
	public void AddItem(ResourceType item)
	{
		if (!inventory.ContainsKey(item))
		{
			GameObject itemPrefab = (GameObject)GameObject.Instantiate(Resources.Load("Items/" + item), parent.position, parent.rotation);
			itemPrefab.transform.parent = parent;
			inventory.Add(item, itemPrefab);
		}
	}
	
	public void RemoveItem(ResourceType item)
	{
		GameObject itemPrefab;
		if (inventory.TryGetValue(item, out itemPrefab))
		{
			GameObject.Destroy(itemPrefab);
			inventory.Remove(item);
		}
	}
	
	public void ClearItems()
	{
		foreach (var pair in inventory)
			GameObject.Destroy(pair.Value);
		
		inventory.Clear();
	}
}
