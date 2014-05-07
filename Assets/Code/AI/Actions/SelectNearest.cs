using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Selects the nearest N game objects with a specific tag near the character.
/// You can select multiple tags by separating them with a space.
/// </summary>
public class SelectNearest : AINode
{
    private string tag;
    private string[] tags;
    private int nearestToRandomlySelect;
    
    public SelectNearest(string tag, string nearestToRandomlySelect)
    {
        this.tag = tag;
        this.tags = tag.Split(' ');
        this.nearestToRandomlySelect = System.Convert.ToInt32(nearestToRandomlySelect);
    }
    
    protected virtual AINodeState SelectNearestTarget(AICharacter character, List<GameObject> targets)
    {
        int index = UnityEngine.Random.Range(0, System.Math.Min(nearestToRandomlySelect, targets.Count));
        character.CurrentSelection = targets[index];

        if (character.CurrentSelection == null)
            Debug.Log("Selection failed");
        
        return character.CurrentSelection != null ? AINodeState.Succeeded : AINodeState.Failed;
    }
    
    public override AINodeState Run(AICharacter character)
    {
        List<GameObject> targets = new List<GameObject>();
        for (int i = 0; i < tags.Length; i++)
            targets.AddRange(GameObject.FindGameObjectsWithTag(tags[i]));

        targets.Sort((a, b) =>
        {
            float aDist2 = (a.transform.position - character.transform.position).sqrMagnitude;
            float bDist2 = (b.transform.position - character.transform.position).sqrMagnitude;
            return aDist2.CompareTo(bDist2);
        });

        if (targets.Count == 0)
            return AINodeState.Failed;

        return SelectNearestTarget(character, targets);
    }
    
    public override string ToString()
    {
        return string.Format("SelectNearest({0},{1})", tag, nearestToRandomlySelect);
    }
}

