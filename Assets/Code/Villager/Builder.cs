
using UnityEngine;
using System.Collections;
using System.Linq;

//The builder is the default villager behaviour
// Waits by the campfire below the village hall until 
[RequireComponent(typeof(PathingCharacter))]
public class Builder : Coward 
{
	protected override IEnumerator RunDaytime()
	{	
		yield return new WaitForSeconds(1.0f);
		
		while (true)
		{
			//Is there any building work available?
			float nearestBuilding = float.MaxValue;
			Building buildingToBuild = null;
			foreach (Building building in GameObject.FindSceneObjectsOfType(typeof(Building)).Cast<Building>())
				if (!building.IsBuilt)
				{
					float distance = Vector3.Distance(building.transform.position, transform.position);
					if (distance < nearestBuilding)
					{
						buildingToBuild = building;
						nearestBuilding = distance;
					}
				}
			
			if (buildingToBuild == null)
			{
				currentState = "Waiting by campfire";
				yield return StartCoroutine(WaitByCampfire());
			}
			else
			{
				currentState = "Moving to building";
				//Go to the building
				int tx = buildingToBuild.Tx, ty = buildingToBuild.Ty;
				yield return StartCoroutine(character.PathTo((x,y) =>
					{
						return Mathf.Abs(tx - x) == 1 && Mathf.Abs(ty - y) == 1;
					},
					(cx, cy) => (Mathf.Abs(tx - cx) + Mathf.Abs(ty - cy)) * 10,
					new Vector2(buildingToBuild.transform.position.x, buildingToBuild.transform.position.z)
				));
				
				//Build it
				currentState = "Building";
				while (!buildingToBuild.IsBuilt)
				{
					buildingToBuild.Build();
					yield return new WaitForSeconds(2.0f);
				}
			}
		}
	}
}
