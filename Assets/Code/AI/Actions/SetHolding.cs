using System;

/// <summary>
/// Sets what the character is currently holding.
/// </summary>
public class SetHolding : AINode
{
    private string item;

    public SetHolding(string item)
    {
        this.item = item;
    }

    public override AINodeState Run(AICharacter character)
    {
        character.HeldItem = item;
        return AINodeState.Succeeded;
    }
}
