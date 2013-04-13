using System;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
	public PathNode(int x, int y, int g, int h, PathNode parent)
	{
		Position = new Vector3(x, 0.0f, y);
		X = x;
		Y = y;
		G = g;
		H = h;
		F = g + h;
		Parent = parent;
	}
	
	public Vector3 Position;
	public int X, Y, G, H, F;
	public PathNode Parent;
	
	public override bool Equals(object o)
	{
		return GetHashCode() == o.GetHashCode();
	}
	
	public override int GetHashCode()
	{
		//We can hash perfectly! WIN
		return X*4098+Y;
	}
}

public class Pathfinder
{
	private Tile[,] tiles;
	private int width, height;
	
	public Pathfinder(Tile[,] tiles, int width, int height)
	{
		this.tiles = tiles;
		this.width = width;
		this.height = height;
	}
	
	public static bool IsWalkable(Tile tile)
	{
		return tile == Tile.Ground || tile == Tile.Bridge;
	}
	
	List<PathNode> openList = new List<PathNode>();
	HashSet<PathNode> closedList = new HashSet<PathNode>();
	
	public List<PathNode> PathTo(int x, int y, Func<int,int,bool> foundTile, Func<int,int,int> calculateCost)
	{
		openList.Clear();
		closedList.Clear();
		
		//Add starting position to open list
		openList.Add(new PathNode(x, y, 0, 0, null));
		
		PathNode current = null;
		while (true)
		{
			//Find smallest cost node
			current = null;
			int smallestCost = int.MaxValue;
			foreach (PathNode node in openList)
			{
				if (node.F < smallestCost)
				{
					current = node;
					smallestCost = node.F;
				}
			}
			
			//No path
			if (current == null)
			{
				Debug.Log("No path");
				return new List<PathNode>();
			}
			
			//Have we found our target?
			if (foundTile(current.X, current.Y))
			{
				//Build the path back to the start
				List<PathNode> path = new List<PathNode>();
				while (current != null)
				{
					path.Add(current);
					current = current.Parent;
				}
				
				path.Reverse(0, path.Count);
				return path;
			}
			
			//Remove from the open list and add to the closed list
			openList.Remove(current);
			closedList.Add(current);
			
			//Add adjacent squares to open list
			AddNode(current, 1, 0, calculateCost);
			AddNode(current, -1, 0, calculateCost);
			AddNode(current, 0, 1, calculateCost);
			AddNode(current, 0, -1, calculateCost);
		}
	}
	
	void AddNode(PathNode parent, int dx, int dy, Func<int,int,int> calculateCost)
	{
		//Are we inside the map?
		int x = parent.X + dx, y = parent.Y + dy;
		if (x < 0 || y < 0 || x >= width || y >= height)
			return;
		
		//Is the square walkable?
		Tile currentTile = tiles[x,y];
		if (!IsWalkable(currentTile))
			return;
		
		//Already in the closed list?
		if (closedList.Contains(new PathNode(x, y, 0, 0, null)))
			return;
		
		//Calculate the cost
		int moveCost = 10;
			
		int g = parent.G + moveCost;
		int h = calculateCost(x, y);
		
		//Have we already added this to the open list?
		foreach (PathNode node in openList)
			if (node.X == x && node.Y == y)
			{
				//We have!
				//Does the current parent make a better path than the old one?
				if (node.G > g)
				{
					node.Parent = parent;
					node.F = g + h;
					node.G = g;
					node.H = h;
				}
			
				return;
			}
		
		openList.Add(new PathNode(x, y, g, h, parent));
	}
}

