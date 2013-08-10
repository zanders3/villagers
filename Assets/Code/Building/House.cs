using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Building))]
public class House : MonoBehaviour 
{
	const int VillagersPerHouse = 3;
	
	int numVillagers = 0;
	const float VillagerCreateTime = 20.0f;
	float villagerCreateTimer = VillagerCreateTime;
	
	bool isBuilt = false;
	
	void Destroy()
	{
        Stockpile.RemoveResource("Villagers", VillagersPerHouse);
	}
	
	void Update()
	{
		if (!GetComponent<Building>().IsBuilt)
			return;
		else if (!isBuilt)
		{
			Debug.Log("Add pop count");
            Stockpile.AddResource("Villagers", VillagersPerHouse);
			
			//Town halls creates the first villager instantly!
			if (GetComponent<Building>().BuildingType == Tile.TownHall)
				CreateVillager();
			
			isBuilt = true;
		}
		
		if (villagerCreateTimer <= 0.0f && CanAddVillager())
		{
			villagerCreateTimer += VillagerCreateTime;
			
			CreateVillager();
		}
		else if (villagerCreateTimer > 0.0f)
			villagerCreateTimer -= Time.deltaTime;
	}
	
	public bool CanAddVillager()
	{
		return numVillagers < VillagersPerHouse;
	}
	
	public void RemoveVillager()
	{
		if (numVillagers > 0)
			numVillagers--;
	}
	
	void CreateVillager()
	{
		Debug.Log("CreateVillager");
		if (!CanAddVillager())
			return;
		
		numVillagers++;
		
        AILoader.CreateCharacter("Villager", "Villager.Production", new Vector3(transform.position.x, 0.0f, transform.position.z));
	}
}
