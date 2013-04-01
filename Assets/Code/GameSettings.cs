using System.Collections.Generic;

public class ResourceSettings
{
	public float TimeToGatherSeconds;
	public int ResourcePerStockpileBlock, ResourcePerTile, ResourceCarried;
	public string ResourcePrefab;
}

//A central place to keep all game balancing settings and magic numbers!
public static class GameSettings
{
	// ------ Town Economy -------
	public static readonly Dictionary<ResourceType, ResourceSettings> ResourceSettings = new Dictionary<ResourceType, ResourceSettings>()
	{
		{ 
			ResourceType.Wood, 
			new ResourceSettings()
			{
				TimeToGatherSeconds = 2.0f,
				ResourceCarried = 5,
				ResourcePerTile = 20,
				ResourcePerStockpileBlock = 10,
				ResourcePrefab = "Resources/Wood"
			}
		},
		{
			ResourceType.Stone,
			new ResourceSettings()
			{
				TimeToGatherSeconds = 4.0f,
				ResourceCarried = 3,
				ResourcePerTile = 9,
				ResourcePerStockpileBlock = 6,
				ResourcePrefab = "Resources/Rock"
			}
		},
		{
			ResourceType.Pike,
			new ResourceSettings()
			{
				TimeToGatherSeconds = 20.0f,
				ResourceCarried = 1,
				ResourcePerStockpileBlock = 1,
				ResourcePrefab = "Resources/Pike"
			}
		}
	};
	
	public static readonly Dictionary<Tile, Dictionary<ResourceType, int>> BuildingCost = new Dictionary<Tile, Dictionary<ResourceType, int>>()
	{
		{ 
			Tile.Stockpile, 
			new Dictionary<ResourceType, int>()
			{
				{ ResourceType.Wood, 5 }
			}
		},
		{
			Tile.House,
			new Dictionary<ResourceType, int>()
			{
				{ ResourceType.Wood, 40 }
			}
		},
		{
			Tile.Poleturner,
			new Dictionary<ResourceType, int>()
			{
				{ ResourceType.Wood, 40 }
			}
		}
	};
}
