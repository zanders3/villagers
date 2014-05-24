using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Map : MonoBehaviour 
{
    private static Map map = null;
	public static Map Instance
    {
        get 
        { 
            if (map == null)
                map = GameObject.FindObjectOfType<Map>();
            return map; 
        }
    }
	
    [SerializeField] private Level level;

#if UNITY_EDITOR
    public Level Level { get { return level; } set { level = value; } }
#endif

	public Pathfinder Pathfinder = null;
	
	public int MapVersion
	{
		get;
		private set;
	}
	
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 0, 0);
        Gizmos.DrawCube(new Vector3(level.Width * 0.5f, 0.0f, level.Height * 0.5f), new Vector3(level.Width, 0.01f, level.Height));
        
        Gizmos.color = Color.white;
        for (int x = 0; x<=level.Width; x++)
            Gizmos.DrawLine(new Vector3(x, 0.0f, 0.0f), new Vector3(x, 0.0f, level.Height));
        
        for (int y = 0; y<=level.Height; y++)
            Gizmos.DrawLine(new Vector3(0.0f, 0.0f, y), new Vector3(level.Width, 0.0f, y));
    }

	//All buildings must have at least 1 square of ground around them
	public bool CanPlaceBuilding(Tile tile, int tx, int ty, int width, int height)
	{
		if (tx < 1 || ty < 1 || tx >= level.Width - 1 || ty >= level.Height - 1)
			return false;
		
        return false;
		//You can only build on tiles adjacent/diagonal to ground/tile being placed
		/*for (int x = -1; x <= 1; x++)
			for (int y = -1; y <= 1; y++)
				if (tiles[tx + x, ty + y] != Tile.Ground && tiles[tx + x, ty + y] != tile)
					return false;
		
		//You cannot build diagonal to the same tile
		if (tiles[tx-1,ty-1] == tile || tiles[tx-1,ty+1] == tile || tiles[tx+1,ty+1] == tile || tiles[tx+1,ty-1] == tile)
			return false;
		
		//You can only build adjacent to the same tile along the same direction
		bool sameTileAbove = tiles[tx, ty-1] == tile || tiles[tx, ty+1] == tile;
		bool sameTileAside = tiles[tx+1, ty] == tile || tiles[tx-1, ty] == tile;
		if (sameTileAbove && sameTileAside)
			return false;
		
		return true;*/
	}
	
	public void PlaceBuilding(Tile tile, int tx, int ty, int width, int height)
	{
		/*for (int x = 0; x<level.Width; x++)
			for (int y = 0; y<level.Height; y++)
				tiles[tx + x, ty + y] = tile;
		
		MapVersion++;*/
	}
	
	public bool HasNeighbour(int tx, int ty, Tile tile)
	{
		if (tx < 1 || ty < 1 || tx >= level.Width - 1 || ty >= level.Height - 1)
			return false;
        return false;
		//return tiles[tx-1,ty] == tile || tiles[tx+1,ty] == tile || tiles[tx,ty-1] == tile || tiles[tx,ty+1] == tile;
	}
	
	public Resource GetNearbyResource(int tx, int ty, Tile tile)
	{
		if (tx < 1 || ty < 1 || tx >= level.Width - 1 || ty >= level.Height - 1)
			return null;
		
		/*if (tiles[tx,ty] == tile)
			return resources[tx, ty];
		if (tiles[tx-1,ty] == tile)
			return resources[tx-1, ty];
		if (tiles[tx+1,ty] == tile)
			return resources[tx+1, ty];
		if (tiles[tx,ty-1] == tile)
			return resources[tx, ty-1];
		if (tiles[tx,ty+1] == tile)
			return resources[tx, ty+1];*/
		
		return null;
	}
	
	public bool IsEmpty(int tx, int ty)
	{
        return false;
		//return tiles[tx,ty] == Tile.Ground;
	}
	
	public void ClearResource(Resource resource)
	{
		//int tx = Mathf.RoundToInt(resource.transform.position.x), ty = Mathf.RoundToInt(resource.transform.position.z);
		//tiles[tx,ty] = Tile.Ground;
		//resources[tx, ty] = null;
		
		Destroy(resource.gameObject);
	}
}
