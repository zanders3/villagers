using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Assigns characters to tagged game objects, limited to one character per tagged object.
/// Used to assign characters to buildings, gremlins to villagers, etc.
/// </summary>
public class IsAssigned : AINode
{
    private static Dictionary<string, Stack<GameObject>> pendingAssignments = new Dictionary<string, Stack<GameObject>>();
    private string desiredAssignment;

    public IsAssigned(string assignment)
    {
        this.desiredAssignment = assignment;
    }

    public override AINodeState Run(AICharacter character)
    {
        if (character.CurrentAssignment == null && pendingAssignments.ContainsKey(desiredAssignment) && pendingAssignments[desiredAssignment].Count > 0)
            character.CurrentAssignment = pendingAssignments[desiredAssignment].Pop();

        if (character.CurrentAssignment != null && character.CurrentAssignment.tag == desiredAssignment)
            return AINodeState.Succeeded;
        else
            return AINodeState.Failed;
    }

    public static void RegisterAssignment(GameObject obj)
    {
        Debug.Log("RegisterAssignment: " + obj.tag);

        if (pendingAssignments.ContainsKey(obj.tag))
            pendingAssignments[obj.tag].Push(obj);
        else
            pendingAssignments.Add(obj.tag, new Stack<GameObject>(new GameObject[] { obj }));
    }
}
