using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public static class MeshGenerator
{
	class Vertex
	{
		public Vector3 Pos;
		public Vector3 Normal;
		public Vector2 UV;

		public Vertex(Tile tile, float x, float y)
		{
			Pos = new Vector3(x + 0.5f, (int)tile - 1.0f, y + 0.5f);
			Normal = Vector3.up;
			UV = new Vector2(x, y);
		}
		
		public override bool Equals (object obj)
		{
			Vertex o = obj as Vertex;
			if (o != null)
				return Pos == o.Pos;
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return Pos.GetHashCode();
		}
	}

	public static void GenerateMesh(Level level, MeshFilter meshFilter)
	{
		List<Vertex> verts = new List<Vertex>();
		List<int> inds = new List<int>();
		
		int[][] fillShapes = new int[][]
		{
			//http://en.wikipedia.org/wiki/Marching_squares
			//Assumes vertices move clockwise every half square
			//Case 0 Empty
			new int[]
			{
			},
			//Case 1 Bottom Left
			new int[]
			{
				5, 6, 7
			},
			//Case 2 Bottom Right
			new int[]
			{
				3, 4, 5
			},
			//Case 3 Bottom
			new int[]
			{
				3, 4, 7,
				4, 6, 7
			},
			//Case 4 Top Right
			new int[]
			{
				1, 2, 3
			},
			//Case 5 BL <-> TR
			new int[]
			{
				1, 2, 3,
				5, 6, 7
			},
			//Case 6 Right
			new int[]
			{
				1, 2, 4,
				4, 5, 1
			},
			//Case 7 Full Bottom Right
			new int[]
			{
				4, 6, 7,
				7, 1, 4,
				1, 2, 4
			},
			//Case 8 Top Left
			new int[]
			{
				7, 0, 1
			},
			//Case 9 Left
			new int[]
			{
				0, 1, 5,
				5, 6, 0
			},
			//Case 10 TL <-> BR
			new int[]
			{
				7, 0, 1,
				3, 4, 5
			},
			//Case 11 Full Bottom Left
			new int[]
			{
				0, 1, 3,
				0, 3, 4,
				0, 4, 6
			},
			//Case 12 Top
			new int[]
			{
				0, 2, 3,
				3, 7, 0
			},
			//Case 13 Full Top Left
			new int[]
			{
				0, 2, 3,
				0, 3, 5,
				0, 5, 6
			},
			//Case 14 Full Top Right
			new int[]
			{
				0, 2, 4,
				0, 4, 5,
				0, 5, 7
			},
			//Case 15 Full
			new int[]
			{
				0, 2, 4,
				0, 4, 6
			}
		};

		for (Tile currentTile = Tile.Ground; currentTile <= Tile.Ground; currentTile++)
		{
			for (int x = -1; x<level.Width; x++)
			{
				for (int y = -1; y<level.Height; y++)
				{
					int ind = 
						(level[x,y] == currentTile ? 8 : 0) + 
						(level[x+1,y] == currentTile ? 4 : 0) +
						(level[x+1,y+1] == currentTile ? 2 : 0) +
						(level[x,y+1] == currentTile ? 1 : 0);

					int offset = verts.Count;
					foreach (int i in fillShapes[ind])
						inds.Add(i + offset);

					float yThresh = level[x,y] == currentTile ? 0.2f : 0.8f;

					verts.Add(new Vertex(currentTile, x, y));
					verts.Add(new Vertex(currentTile, x + (level[x,y] == currentTile ? 0.2f : 0.8f), y));
					verts.Add(new Vertex(currentTile, x + 1.0f, y));
					verts.Add(new Vertex(currentTile, x + 1.0f, y + (level[x+1,y] == currentTile ? 0.2f : 0.8f)));
					verts.Add(new Vertex(currentTile, x + 1.0f, y + 1.0f));
					verts.Add(new Vertex(currentTile, x + (level[x,y+1] == currentTile ? 0.2f : 0.8f), y + 1.0f));
					verts.Add(new Vertex(currentTile, x, y + 1.0f));
					verts.Add(new Vertex(currentTile, x, y + (level[x,y] == currentTile ? 0.2f : 0.8f)));
				}
			}
		}
		
		ToMesh(meshFilter, verts, inds);
	}
	
	static void ToMesh(MeshFilter meshFilter, List<Vertex> verts, List<int> inds)
	{
		if (meshFilter.sharedMesh == null)
			meshFilter.mesh = new Mesh();
		
		meshFilter.sharedMesh.Clear();
		Mesh mesh = meshFilter.sharedMesh;

		Dictionary<Vertex, int> vertToIndex = new Dictionary<Vertex, int>();
		List<Vertex> finalVerts = new List<Vertex>();
		for (int i = 0; i<inds.Count; i++)
		{
			int newInd;
			Vertex vert = verts[inds[i]];
			if (!vertToIndex.TryGetValue(vert, out newInd))
			{
				vertToIndex.Add(vert, finalVerts.Count);
				newInd = finalVerts.Count;
				finalVerts.Add(vert);
			}

			inds[i] = newInd;
		}

		//Swap index winding order since default is CCW
		for (int i = 0; i<inds.Count; i+=3)
		{
			int tmp = inds[i+1];
			inds[i+1] = inds[i+2];
			inds[i+2] = tmp;
		}

		mesh.vertices = finalVerts.Select(vert => vert.Pos).ToArray();
		mesh.uv = finalVerts.Select(vert => vert.UV).ToArray();
		mesh.normals = finalVerts.Select(vert => vert.Normal).ToArray();
		mesh.triangles = inds.ToArray();
	}
}
