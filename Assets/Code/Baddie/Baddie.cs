using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

//The baddie by default attacks the town hall, attacking anything within 3 tiles on the way.
[RequireComponent(typeof(PathingCharacter))]
public class Baddie : MonoBehaviour
{
	PathingCharacter character;
	
	Component currentTarget = null;
	
	public int Attack = 1;
	public int Health = 3;
	
	void Start()
	{
		character = GetComponent<PathingCharacter>();
	}
	
	IEnumerable Run()
	{
		while (true)
		{
			if (currentTarget == null)
			{
				//Select nearest target (either villager or building)
				Villager villager = GetNearest<Villager>();
				Building building = GetNearest<Building>();
				
				if (villager != null && building != null)
					if (Random.Range(0, 100) > 50)
						SetTarget(villager);
					else
						SetTarget(building);
				else if (villager != null)
					SetTarget(villager);
				else if (building != null)
					SetTarget(building);
			}
			
			yield return new WaitForSeconds(1.0f);
		}
	}
	
	T GetNearest<T>() where T : Component
	{
		float nearestTarget = 5.0f * 5.0f;
		T currentTarget = null;
		foreach (T target in GameObject.FindObjectsOfType(typeof(T)).Cast<T>())
		{
			float distance = (target.transform.position - transform.position).sqrMagnitude;
			if (nearestTarget > distance)
			{
				currentTarget = target;
				nearestTarget = distance;
			}
		}
		
		return currentTarget;
	}
	
	void SetTarget<T>(T target) where T : Component
	{
		StopAllCoroutines();
		StartCoroutine(AttackTarget(target));
	}
	
	IEnumerator AttackTarget<T>(T target) where T : Component
	{
		currentTarget = target;
		
		Building building = target as Building;
		if (building != null)
		{
			yield return character.PathToBuilding(building);
			
			while (building.Health > 0)
				building.Damage(Attack);
		}
		
		currentTarget = null;
	}
}
