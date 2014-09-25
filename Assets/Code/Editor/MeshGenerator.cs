using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;

public static class MeshGenerator
{
	struct PlaceInfo
	{
		public PlaceInfo(MeshFilter[] meshFilters, params int[] angles)
		{
			Meshes = meshFilters != null ? meshFilters.Select(filter => filter.sharedMesh).ToArray() : new Mesh[0];
			Angles = angles;
		}

		public int[] Angles;
		public Mesh[] Meshes;
	};

	public static void GenerateMesh(MeshTransition meshInfo, Level level, MeshFilter meshFilter)
	{
		if (meshInfo == null)
			return;

		PlaceInfo[] infos = new PlaceInfo[]
		{
			new PlaceInfo(null, 0),  //Case 0
			new PlaceInfo(meshInfo.Corner, 2),//Case 1
			new PlaceInfo(meshInfo.Corner, 3),//Case 2
			new PlaceInfo(meshInfo.Side, 2),  //Case 3
			new PlaceInfo(meshInfo.Corner, 0),//Case 4
			new PlaceInfo(meshInfo.Corner, 0, 2),//Case 5
			new PlaceInfo(meshInfo.Side, 3),  //Case 6
			new PlaceInfo(meshInfo.InsetCorner, 3),//Case 7
			new PlaceInfo(meshInfo.Corner, 1),//Case 8
			new PlaceInfo(meshInfo.Side, 1),//Case 9
			new PlaceInfo(meshInfo.Corner, 1, 3),//Case 10
			new PlaceInfo(meshInfo.InsetCorner, 2),//Case 11
			new PlaceInfo(meshInfo.Side, 0),//Case 12
			new PlaceInfo(meshInfo.InsetCorner, 1),//Case 13
			new PlaceInfo(meshInfo.InsetCorner, 0),//Case 14
			new PlaceInfo(meshInfo.Fill, 0)//Case 15
		};

		Tile tile = Tile.Ground;
		List<CombineInstance> combineInst = new List<CombineInstance>();
		for (int x = -1; x<level.Width; x++)
		{
			for (int y = -1; y<level.Height; y++)
			{
				int ind = 
					(level[x,y] == tile ? 8 : 0) + 
					(level[x+1,y] == tile ? 4 : 0) +
					(level[x+1,y+1] == tile ? 2 : 0) +
					(level[x,y+1] == tile ? 1 : 0);

				if (infos[ind].Meshes.Length == 0)
					continue;

				PlaceInfo info = infos[ind];
				for (int i = 0; i<info.Angles.Length; i++)
				{
					combineInst.Add(new CombineInstance
					{
						mesh = info.Meshes[Random.Range(0, info.Meshes.Length)],
						transform = Matrix4x4.TRS(
							new Vector3(x + 1.0f, 0, y + 1.0f),
							Quaternion.Euler(-90.0f, info.Angles[i] * 90.0f, 0.0f),
			              	Vector3.one
						)
					});
				}
			}
		}

		if (meshFilter.sharedMesh == null)
			meshFilter.mesh = new Mesh();
		
		meshFilter.sharedMesh.Clear();
		Mesh mesh = meshFilter.sharedMesh;
		mesh.CombineMeshes(combineInst.ToArray());

		{
			Vector3[] verts = mesh.vertices;
			Vector2[] uvs = new Vector2[verts.Length];
			for (int i = 0; i<verts.Length; i++)
				uvs[i] = new Vector2(verts[i].x, verts[i].z);

			mesh.uv = uvs;
		}
	}
}
