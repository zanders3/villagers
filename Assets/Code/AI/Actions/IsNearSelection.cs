using UnityEngine;
using System.Collections.Generic;

public class IsNearTo : SelectNearest
{
    private float distance;

    public IsNearTo(string tag, string distance) : base(tag, "1")
    {
        this.distance = System.Convert.ToSingle(distance);
    }

    protected override AINodeState SelectNearestTarget(AICharacter character, List<GameObject> targets)
    {
        float dist = Vector3.Distance(character.transform.position, targets[0].transform.position);
        return dist < distance ? AINodeState.Succeeded : AINodeState.Failed;
    }
}

public class IsFarFrom : SelectNearest
{
    private float distance;
    
    public IsFarFrom(string tag, string distance) : base(tag, "1")
    {
        this.distance = System.Convert.ToSingle(distance);
    }
    
    protected override AINodeState SelectNearestTarget(AICharacter character, List<GameObject> targets)
    {
        float dist = Vector3.Distance(character.transform.position, targets[0].transform.position);
        return dist < distance ? AINodeState.Failed : AINodeState.Succeeded;
    }
}