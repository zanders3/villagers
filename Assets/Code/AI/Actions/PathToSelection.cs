using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

struct Point2
{
    public Point2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;

    public override int GetHashCode()
    {
        return x * 2048 + y;
    }

    public override bool Equals(object obj)
    {
        return obj is Point2 && ((Point2)obj) == this;
    }

    public static bool operator == (Point2 a, Point2 b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator != (Point2 a, Point2 b)
    {
        return !(a == b);
    }

    public override string ToString()
    {
        return x + ", " + y;
    }
}

/// <summary>
/// Attempts to path to the currently targeted game object.
/// Each path node is seeked towards before finally seeking to the final target position.
/// </summary>
public class PathToSelection : AINode
{
    private List<PathNode> path = null;
    private int mapVersion = -1;
    private Point2 targetPos = new Point2(-1, -1);

    public override AINodeState Run(AICharacter character)
    {
        if (character.CurrentSelection == null)
            return AINodeState.Failed;

        Point2 currentTargetPos = new Point2(
            Mathf.FloorToInt(character.CurrentSelection.transform.position.x),
            Mathf.FloorToInt(character.CurrentSelection.transform.position.z)
        );
        if (path == null || currentTargetPos != targetPos || Map.Instance.MapVersion != mapVersion)
        {
            targetPos = currentTargetPos;
            mapVersion = Map.Instance.MapVersion;

            Point2 currentPos = new Point2(
                Mathf.FloorToInt(character.transform.position.x),
                Mathf.FloorToInt(character.transform.position.z)
            );
            Debug.Log(currentPos + " -> " + targetPos);

            path = Map.Instance.Pathfinder.PathTo(
                currentPos.x,
                currentPos.y,
                (cx, cy) => Math.Abs(cx - targetPos.x) == 1 && Math.Abs(cy - targetPos.y) == 1,
                (cx, cy) => (Math.Abs(targetPos.x - cx) + Math.Abs(targetPos.x - cy)) * 10
            );

            if (path.Count == 0)
            {
                Reset();
                return AINodeState.Failed;
            }
        }

        float dist = character.Seek(path.Count > 0 ? path[0].Position : character.CurrentSelection.transform.position);
        if (path.Count > 0)
        {
            if (dist < 1.0f)
                path.RemoveAt(0);

            return AINodeState.Running;
        } 
        else if (dist < 1.0f)
        {
            Reset();
            return AINodeState.Succeeded;
        }
        else
            return AINodeState.Running;
    }

    public override void Reset()
    {
        path = null;
        targetPos = new Point2(-1, -1);
    }

    public override string ToString()
    {
        return string.Format("PathToSelection({0},{1},{2})", 
                             targetPos.x, 
                             targetPos.y, 
                             path != null ? path.Count.ToString() : "no path"
        );
    }
}
