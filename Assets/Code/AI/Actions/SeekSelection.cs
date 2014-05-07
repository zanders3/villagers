using System;

public class SeekSelection : AINode
{
    public override AINodeState Run(AICharacter character)
    {
        if (character.CurrentSelection == null)
            return AINodeState.Failed;

        character.Seek(character.CurrentSelection.transform.position);

        return AINodeState.Succeeded;
    }

    public override string ToString()
    {
        return "SeekSelection";
    }
}

