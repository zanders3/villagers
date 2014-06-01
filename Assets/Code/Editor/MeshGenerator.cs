using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public static class MeshGenerator
{
	class Vertex
	{
		public Vector3 Pos;
		public Vector3 Normal;
		public Vector2 UV;

		public Vertex(float x, float y, float ux, float uy)
		{
			Pos = new Vector3(x + 0.5f, 0.0f, y + 0.5f);
			Normal = Vector3.up;
			UV = new Vector2(x, y);
		}
		
		public override bool Equals (object obj)
		{
			Vertex o = obj as Vertex;
			if (o != null)
				return Pos == o.Pos && UV == o.UV;
			else
				return false;
		}
		
		public override int GetHashCode()
		{
			return Pos.GetHashCode() | UV.GetHashCode();
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
				7, 1, 2,
				2, 6, 7,
				2, 4, 6
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
				2, 3, 5,
				5, 6, 2,
				2, 6, 0
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

		Tile tile = Tile.Ground;
		for (int x = -1; x<level.Width; x++)
		{
			for (int y = -1; y<level.Height; y++)
			{
				int ind = 
					(level[x,y] == tile ? 8 : 0) + 
					(level[x+1,y] == tile ? 4 : 0) +
					(level[x+1,y+1] == tile ? 2 : 0) +
					(level[x,y+1] == tile ? 1 : 0);

				int offset = verts.Count;
				foreach (int i in fillShapes[ind])
					inds.Add(i + offset);

				float min = 0.2f, max = 0.8f;
				int total = 0;
				verts.Add(new Vertex(x, y, total == 2 ? 0.0f : 1.0f, 0.0f));
				verts.Add(new Vertex(x + (level[x,y] == tile ? min : max), y, total == 2 ? 0.0f : 1.0f, 1.0f));
				verts.Add(new Vertex(x + 1.0f, y, total == 2 ? 1.0f : 0.0f, 0.0f));
				verts.Add(new Vertex(x + 1.0f, y + (level[x+1,y] == tile ? min : max), total == 2 ? 1.0f : 0.0f, 1.0f));
				verts.Add(new Vertex(x + 1.0f, y + 1.0f, total == 2 ? 1.0f : 0.0f, 0.0f));
				verts.Add(new Vertex(x + (level[x,y+1] == tile ? min : max), y + 1.0f, total == 2 ? 1.0f : 0.0f, 1.0f));
				verts.Add(new Vertex(x, y + 1.0f, total == 2 ? 0.0f : 1.0f, 0.0f));
				verts.Add(new Vertex(x, y + (level[x,y] == tile ? min : max), total == 2 ? 0.0f : 1.0f, 1.0f));
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
