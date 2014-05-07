using System;

/// <summary>
/// Sets the object that this character has been assigned to as the current selection.
/// </summary>
public class SelectAssigned : AINode
{
    public override AINodeState Run(AICharacter character)
    {
        if (character.CurrentAssignment == null)
            return AINodeState.Failed;

        character.CurrentSelection = character.CurrentAssignment;
        return AINodeState.Succeeded;
    }
}

