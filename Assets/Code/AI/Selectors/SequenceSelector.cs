using System;

/// <summary>
/// Runs first child node onwards until it returns Running or completes
/// Stores which node last returned Running
/// </summary>
public class SequenceSelector : AINode
{
    private AINode[] children;
    private string name;
    private int lastRunningNode = 0, lastRunNode = 0;
    
    public SequenceSelector(string name, params AINode[] children)
    {
        this.name = name;
        this.children = children;
    }
    
    public override AINodeState Run(AICharacter character)
    {
        for (int i = lastRunningNode; i<children.Length; i++)
        {
            lastRunNode = i;
            switch (children[i].Run(character))
            {
                case AINodeState.Running:
                    lastRunningNode = i;
                    return AINodeState.Running;
                    
                case AINodeState.Failed:
                    Reset();
                    return AINodeState.Failed;
            }
        }
        
        Reset();
        return AINodeState.Succeeded;
    }
    
    public override void Reset()
    {
        lastRunningNode = 0;
        for (int i = 0; i<children.Length; i++)
            children[i].Reset();
    }
    
    public override string ToString()
    {
        return name + "." + children[lastRunNode].ToString();
    }
}
