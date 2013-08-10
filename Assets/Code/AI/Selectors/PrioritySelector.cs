using System;

/// <summary>
/// Executes each node in sequence until a node returns Success or Running
/// If the node that runs changes the previous Running or Successful node is Reset
/// </summary>
public class PrioritySelector : AINode
{
    private AINode[] children;
    private string name;
    private int lastRun = -1;
    
    public PrioritySelector(string name, params AINode[] children)
    {
        this.name = name;
        this.children = children;
    }
    
    public override AINodeState Run(AICharacter character)
    {
        for (int i = 0; i<children.Length; i++)
        {
            AINodeState state = children[i].Run(character);
            if (state != AINodeState.Failed)
            {
                if (lastRun != i && lastRun != -1)
                    children[lastRun].Reset();
                lastRun = i;
                return state;
            }
        }

        return AINodeState.Succeeded;
    }
    
    public override void Reset()
    {
        if (lastRun != -1)
            children[lastRun].Reset();
        lastRun = -1;
    }
    
    public override string ToString()
    {
        if (lastRun != -1)
            return name + "." + children[lastRun].ToString();
        else
            return name;
    }
}

