using System;

/// <summary>
/// Places the specified resource on the stockpile. Fails if the stockpile is full.
/// </summary>
public class PlaceResource : AINode
{
    private string resource;

    public PlaceResource(string resource)
    {
        this.resource = resource;
    }

    public override AINodeState Run(AICharacter character)
    {
        //TODO: stockpile space and visibility
        Stockpile.AddResource(resource, 1);

        return AINodeState.Succeeded;
    }
}
