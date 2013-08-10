using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Selects the nearest N game objects with a specific tag near the character.
/// </summary>
public class SelectNearest : AINode
{
    private string tag;
    private int nearestToRandomlySelect;
    
    public SelectNearest(string tag, string nearestToRandomlySelect)
    {
        this.tag = tag;
        this.nearestToRandomlySelect = System.Convert.ToInt32(nearestToRandomlySelect);
    }
    
    protected virtual GameObject SelectNearestTarget(List<GameObject> targets)
    {
        if (targets.Count == 0)
            return null;
        
        int index = UnityEngine.Random.Range(0, System.Math.Min(nearestToRandomlySelect, targets.Count));
        return targets[index];
    }
    
    public override AINodeState Run(AICharacter character)
    {
        List<GameObject> targets = GameObject.FindGameObjectsWithTag(tag).ToList();
        targets.Sort((a, b) =>
        {
            float aDist2 = (a.transform.position - character.transform.position).sqrMagnitude;
            float bDist2 = (b.transform.position - character.transform.position).sqrMagnitude;
            return aDist2.CompareTo(bDist2);
        });

        character.CurrentSelection = null;
        character.CurrentSelection = SelectNearestTarget(targets);

        if (character.CurrentSelection == null)
            Debug.Log("Selection failed");

        return character.CurrentSelection != null ? AINodeState.Succeeded : AINodeState.Failed;
    }
    
    public override string ToString()
    {
        return string.Format("SelectNearest({0},{1})", tag, nearestToRandomlySelect);
    }
}

