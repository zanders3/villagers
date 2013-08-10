using System;

/// <summary>
/// Removes a resource counter from the currently targeted resource object.
/// This action fails if all of the resources have already been gathered from this object.
/// </summary>
public class GatherSelection : AINode
{
    public override AINodeState Run(AICharacter character)
    {
        if (character.CurrentSelection == null || character.CurrentSelection.GetComponent<Resource>() == null)
            return AINodeState.Failed;

        return character.CurrentSelection.GetComponent<Resource>().Gather() ? AINodeState.Succeeded : AINodeState.Failed;
    }
}
