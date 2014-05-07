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
        if (character.CurrentAssignment == null && CanRegister(desiredAssignment))
            character.CurrentAssignment = pendingAssignments[desiredAssignment].Pop();

        if (character.CurrentAssignment != null && character.CurrentAssignment.tag == desiredAssignment)
            return AINodeState.Succeeded;
        else
            return AINodeState.Failed;
    }

    public static void RegisterAssignment(GameObject obj, string tag, int numToAssign)
    {
        Debug.Log("RegisterAssignment: " + tag);

        Stack<GameObject> assignments;
        if (!pendingAssignments.TryGetValue(tag, out assignments))
        {
            pendingAssignments.Add(tag, assignments = new Stack<GameObject>());
        }

        for (int i = 0; i<numToAssign; i++)
            assignments.Push(obj);
    }

    public static bool CanRegister(string tag)
    {
        return pendingAssignments.ContainsKey(tag) && pendingAssignments[tag].Count > 0;
    }
}
