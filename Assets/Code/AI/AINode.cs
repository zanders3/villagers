using System;
using System.Collections.Generic;

public enum AINodeState
{
    Running,
    Failed,
    Succeeded
}

public abstract class AINode
{
    public abstract AINodeState Run(AICharacter character);

    public virtual void Reset()
    {
    }
}
