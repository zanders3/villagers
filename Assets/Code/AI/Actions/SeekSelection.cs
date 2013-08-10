using System;

public class SeekSelection : AINode
{
    public override AINodeState Run(AICharacter character)
    {
        if (character.CurrentSelection == null)
            return AINodeState.Failed;

        float dist = character.Seek(character.CurrentSelection.transform.position);

        return dist < 1.0f ? AINodeState.Succeeded : AINodeState.Running;
    }

    public override string ToString()
    {
        return "SeekSelection";
    }
}

