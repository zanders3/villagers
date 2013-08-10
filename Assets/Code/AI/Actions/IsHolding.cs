using System;

/// <summary>
/// Checks if the character is holding the specified thing.
/// </summary>
public class IsHolding : AINode
{
    private string item;

    public IsHolding(string item)
    {
        this.item = item;
    }

    public override AINodeState Run(AICharacter character)
    {
        return character.HeldItem == item ? AINodeState.Succeeded : AINodeState.Failed;
    }
}
