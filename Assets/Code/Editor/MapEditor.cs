using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CustomEditor(typeof(Map))]
public class MapEditor : Editor
{
	Tile selectedTile = Tile.Ground;
	int width = 0, height = 0;

	void OnEnable()
	{
		Map map = target as Map;
		Level level = map.Level;

		if (level == null)
		{
			level = new Level();
			level.Resize(20, 20);
			map.Level = level;
		}

		width = level.Width;
		height = level.Height;
	}

    public override void OnInspectorGUI()
    {
		Map map = target as Map;
		Level level = map.Level;

		GUILayout.BeginHorizontal();
		{
			width = EditorGUILayout.IntField(width);
			height = EditorGUILayout.IntField(height);
			if (GUILayout.Button("Resize"))
			{
				level.Resize(width, height);
				EditorUtility.SetDirty(map);
			}
		}
		GUILayout.EndHorizontal();

		selectedTile = (Tile)GUILayout.SelectionGrid((int)selectedTile, System.Enum.GetNames(typeof(Tile)), 3);
    }

	Vector3 lastPosition = Vector3.zero;

	void OnSceneGUI()
	{
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		Tools.current = Tool.None;

		Plane ground = new Plane(Vector3.up, 0.0f);
		float enter;
		Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Camera.current.pixelHeight - Event.current.mousePosition.y));
		ground.Raycast(mouseRay, out enter);

		Vector3 position = mouseRay.origin + mouseRay.direction * enter;
		position.x = Mathf.Floor(position.x);
		position.z = Mathf.Floor(position.z);
		if (lastPosition != position)
		{
			lastPosition = position;
			SceneView.RepaintAll();
		}

		Handles.DrawSolidRectangleWithOutline(new Vector3[]
		{
			position,
			position + new Vector3(1, 0, 0),
			position + new Vector3(1, 0, 1),
			position + new Vector3(0, 0, 1)
		}, new Color(0.2f, 0.2f, 1.0f, 0.2f), new Color(0.2f, 0.2f, 1.0f, 0.2f));

		Map map = target as Map;
		Level level = map.Level;

		if ((Event.current.type == EventType.mouseDown || Event.current.type == EventType.mouseDrag) && 
		    Event.current.button == 0 && !Event.current.alt && !Event.current.shift)
		{
			if (position.x >= 0.0f && position.y >= 0.0f && 
			    position.x < level.Width && position.z < level.Height)
			{
				Event.current.Use();

				level[(int)position.x, (int)position.z] = selectedTile;
				MeshGenerator.GenerateMesh(level, map.GetComponent<MeshFilter>());
			}
			else if (Event.current.type == EventType.mouseDown)
				Selection.activeGameObject = null;
		}

		if (Event.current.type == EventType.Layout) 
		{
			HandleUtility.AddDefaultControl(controlID);
		}
	}
}
