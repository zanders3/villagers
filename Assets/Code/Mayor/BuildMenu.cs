using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class BuildingDefinition
{	
	public bool CanAfford()
	{
		Debug.Log ("CanAfford: " + Prefab.BuildingType);
		foreach (var resourceCost in GameSettings.BuildingCost[Prefab.BuildingType])
			if (Stockpile.Resources[resourceCost.Key] < resourceCost.Value)
				return false;
		
		return true;
	}
	
	public void Place(int tx, int ty)
	{
		if (!CanAfford())
			return;
		
		foreach (var resourceCost in GameSettings.BuildingCost[Prefab.BuildingType])
			Stockpile.WithdrawResource(resourceCost.Value, resourceCost.Key);
		
		GameObject.Instantiate(Prefab, new Vector3(tx, 0.0f, ty), Quaternion.identity);
	}
	
	public Building Prefab;
	public string Name;
}

public class BuildMenu : MonoBehaviour
{
	const float ButtonSize = 80.0f;
	enum BuildState
	{
		Closed,
		Open,
		Placing
	}
	
	public BuildingDefinition[] Buildings = new BuildingDefinition[0];
	
	BuildState currentState = BuildState.Closed;
	BuildingDefinition buildingToPlace = null;
	
	bool mouseOverGUI = false;
	
	void OnGUI()
	{
		if(Event.current.type == EventType.Repaint)
		{
			//mouseOverGUI = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
        }
		
		string resources = string.Join("\n", Stockpile.Resources.Select(pair => pair.Key + ":\t" + pair.Value).ToArray());
		GUI.Label(new Rect(10.0f, 10.0f, 300.0f, 200.0f), resources);
		
		switch (currentState)
		{
		case BuildState.Closed:
			if (GUI.Button(new Rect(Screen.width - ButtonSize, Screen.height - ButtonSize, ButtonSize, ButtonSize), "Build"))
			{
				currentState = BuildState.Open;
			}
			break;
			
		case BuildState.Open:
			if (GUI.Button(new Rect(Screen.width - ButtonSize, Screen.height - ButtonSize, ButtonSize, ButtonSize), "Cancel"))
			{
				currentState = BuildState.Closed;
			}
			
			for (int i = 0; i<Buildings.Length; i++)
			{
				if (GUI.Button(new Rect(Screen.width - (ButtonSize * (i + 2)), Screen.height - ButtonSize, ButtonSize, ButtonSize), Buildings[i].Name) && Buildings[i].CanAfford())
				{
					currentState = BuildState.Placing;
					buildingToPlace = Buildings[i];
				}
			}
			break;
			
		case BuildState.Placing:
			if (GUI.Button(new Rect(Screen.width - ButtonSize, Screen.height - ButtonSize, ButtonSize, ButtonSize), "Cancel"))
			{
				currentState = BuildState.Closed;
			}
			break;
		}
	}
	
	public bool OnMouseDown()
	{
		if (mouseOverGUI)
			return true;
		
		if (currentState == BuildState.Placing && buildingToPlace != null)
		{
			int tx, ty;
			if (GetMousePos(out tx, out ty))
			{
				bool canPlace = Map.Instance.CanPlaceBuilding(buildingToPlace.Prefab.BuildingType, tx, ty, buildingToPlace.Prefab.Width, buildingToPlace.Prefab.Height);
				if (canPlace)
				{
					currentState = BuildState.Closed;
					buildingToPlace.Place(tx, ty);
				}
			}
			return true;
		}
		
		return false;
	}
	
	bool GetMousePos(out int tx, out int ty)
	{
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		Plane ground = new Plane(Vector3.up, 0.0f);
		
		float rayIntersection;
		if (ground.Raycast(mouseRay, out rayIntersection))
		{
			tx = Mathf.RoundToInt(mouseRay.origin.x + mouseRay.direction.x * rayIntersection);
			ty = Mathf.RoundToInt(mouseRay.origin.z + mouseRay.direction.z * rayIntersection);
			return true;
		}
		else
		{
			tx = 0;
			ty = 0;
			return false;
		}
	}
	
	void OnDrawGizmos()
	{
		if (currentState == BuildState.Placing && buildingToPlace != null)
		{
			int tx, ty;
			if (GetMousePos(out tx, out ty))
			{
				bool canPlace = Map.Instance.CanPlaceBuilding(buildingToPlace.Prefab.BuildingType, tx, ty, buildingToPlace.Prefab.Width, buildingToPlace.Prefab.Height); 
				
				Gizmos.color = canPlace ? Color.blue : Color.red;
				float offsetX = (buildingToPlace.Prefab.Width - 1) * 0.5f, offsetZ = (buildingToPlace.Prefab.Height - 1) * 0.5f;
				Gizmos.DrawCube(new Vector3(tx + offsetX, 0.0f, ty + offsetZ), new Vector3(buildingToPlace.Prefab.Width, 0.01f, buildingToPlace.Prefab.Height));
			}
		}
	}
}
