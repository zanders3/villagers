using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Tile
{
	Ground,
	Cliff,
	Water,
	Tree,
	TownHall,
	Bridge,
	Rock,
	House,
	Stockpile,
	Campfire,
	Poleturner
}

public class Map : MonoBehaviour 
{
	public static Map Instance = null;
	
	public int Width, Height;
	public TextAsset Level;
	
	private Tile[,] tiles;
	private Resource[,] resources;
	private Dictionary<Tile, GameObject> tileCache = new Dictionary<Tile, GameObject>();
	
	public Pathfinder Pathfinder = null;
	
	public int MapVersion
	{
		get;
		private set;
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		for (int x = 0; x<=Width; x++)
			Gizmos.DrawLine(new Vector3(x - 0.5f, 0.0f, -0.5f), new Vector3(x - 0.5f, 0.0f, Height - 0.5f));
		
		for (int y = 0; y<=Height; y++)
			Gizmos.DrawLine(new Vector3(-0.5f, 0.0f, y - 0.5f), new Vector3(Width - 0.5f, 0.0f, y - 0.5f));
		
		Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
		if (tiles != null)
			for (int x = 0; x<Width; x++)
				for (int y = 0; y<Height; y++)
					if (!Pathfinder.IsWalkable(tiles[x,y]))
						Gizmos.DrawCube(new Vector3(x, 0.0f, y), new Vector3(1.0f, 0.01f, 1.0f));
	}
	
	void Start()
	{
		Instance = this;
		LoadLevel(Level.text);
	}
	
	private void LoadLevel(string levelFile)
	{
		string[] lines = levelFile.Split('\n');
		Height = lines.Length;
		Width = lines[0].Length;
		
		tiles = new Tile[Width,Height];
		resources = new Resource[Width,Height];
		for (int y = 0; y<Height; y++)
		{
			char[] columns = lines[y].ToCharArray();
			for (int x = 0; x<Width; x++)
				tiles[x,y] = (Tile)((int)columns[Width-x-1] - 48);
		}
		
		InstantiateTiles();
		Pathfinder = new Pathfinder(tiles, Width, Height);
	}
	
	private GameObject GetTile(Tile tile)
	{
		GameObject obj;
		if (!tileCache.TryGetValue(tile, out obj))
		{
			obj = (GameObject)Resources.Load("Tiles/" + tile);
			tileCache.Add(tile, obj);
		}
		return obj;
	}
	
	private void MakeTile(Tile tile, int x, int y)
	{	
		Transform t = ((GameObject)GameObject.Instantiate(GetTile(tile), new Vector3(x, 0.0f, y), Quaternion.identity)).transform;
		t.parent = this.transform;
		
		if (tile == Tile.Rock || tile == Tile.Tree || tile == Tile.Cliff)
		{
			t.rotation = Quaternion.Euler(new Vector3(0.0f, Random.value * 360.0f, 0.0f));
		}
		
		if (t.GetComponent<Resource>() != null)
			resources[x,y] = t.GetComponent<Resource>();
	}
	
	private void InstantiateTiles()
	{
		for (int x = 0; x<Width; x++)
		{
			for (int y = 0; y<Height; y++)
			{
				Tile tile = tiles[x,y];
				if (tile != Tile.TownHall || x > 0 && y > 0 && tiles[x-1,y] != Tile.TownHall && tiles[x,y-1] != Tile.TownHall)
					MakeTile(tile, x, y);
				
				if (tile != Tile.Water && tile != Tile.Ground && tile != Tile.Bridge)
					MakeTile(Tile.Ground, x, y);
			}
		}
	}
	
	//All buildings must have at least 1 square of ground around them
	public bool CanPlaceBuilding(Tile tile, int tx, int ty, int width, int height)
	{
		if (tx < 1 || ty < 1 || tx >= Width - 1 || ty >= Height - 1)
			return false;
		
		//You can only build on tiles adjacent/diagonal to ground/tile being placed
		for (int x = -1; x <= 1; x++)
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
		
		return true;
	}
	
	public void PlaceBuilding(Tile tile, int tx, int ty, int width, int height)
	{
		for (int x = 0; x<width; x++)
			for (int y = 0; y<height; y++)
				tiles[tx + x, ty + y] = tile;
		
		MapVersion++;
	}
	
	public bool HasNeighbour(int tx, int ty, Tile tile)
	{
		if (tx < 1 || ty < 1 || tx >= Width - 1 || ty >= Height - 1)
			return false;
		
		return tiles[tx-1,ty] == tile || tiles[tx+1,ty] == tile || tiles[tx,ty-1] == tile || tiles[tx,ty+1] == tile;
	}
	
	public Resource GetNearbyResource(int tx, int ty, Tile tile)
	{
		if (tx < 1 || ty < 1 || tx >= Width - 1 || ty >= Height - 1)
			return null;
		
		if (tiles[tx,ty] == tile)
			return resources[tx, ty];
		if (tiles[tx-1,ty] == tile)
			return resources[tx-1, ty];
		if (tiles[tx+1,ty] == tile)
			return resources[tx+1, ty];
		if (tiles[tx,ty-1] == tile)
			return resources[tx, ty-1];
		if (tiles[tx,ty+1] == tile)
			return resources[tx, ty+1];
		
		return null;
	}
	
	public bool IsEmpty(int tx, int ty)
	{
		return tiles[tx,ty] == Tile.Ground;
	}
	
	public void ClearResource(Resource resource)
	{
		int tx = Mathf.RoundToInt(resource.transform.position.x), ty = Mathf.RoundToInt(resource.transform.position.z);
		tiles[tx,ty] = Tile.Ground;
		resources[tx, ty] = null;
		
		Destroy(resource.gameObject);
	}
}
