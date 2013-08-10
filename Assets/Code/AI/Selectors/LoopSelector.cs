using System;


/// <summary>
/// Runs in an infinite loop until a node returns the Running or Failed state
/// Cannot be Reset
/// </summary>
public class LoopSelector : AINode
{
    private AINode[] children;
    private string name;
    private int lastRunningNode = 0;
    
    public LoopSelector(string name, params AINode[] children)
    {
        this.name = name;
        this.children = children;
    }
    
    public override AINodeState Run(AICharacter character)
    {
        switch (children[lastRunningNode].Run(character))
        {
            case AINodeState.Failed:
                return AINodeState.Failed;
            case AINodeState.Running:
                return AINodeState.Running;
            case AINodeState.Succeeded:
            default:
                lastRunningNode = (lastRunningNode + 1) % children.Length;
                return AINodeState.Running;
        }
    }
    
    public override string ToString()
    {
        return name + "." + children[lastRunningNode].ToString();
    }
}

