using System;

public class FleeSelection : AINode
{
    public override AINodeState Run(AICharacter character)
    {
        if (character.CurrentSelection == null)
            return AINodeState.Failed;

        character.Flee(character.CurrentSelection.transform.position);
        return AINodeState.Succeeded;
    }
}
