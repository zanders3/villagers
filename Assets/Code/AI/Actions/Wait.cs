
/// <summary>
/// Waits for the specified number of seconds.
/// </summary>
using System;


public class Wait : AINode
{
    private float time, currentTimeWaiting = 0.0f;

    public Wait(string time)
    {
        this.time = Convert.ToSingle(time);
    }

    public override AINodeState Run(AICharacter character)
    {
        currentTimeWaiting += UnityEngine.Time.deltaTime;

        if (currentTimeWaiting >= time)
        {
            Reset();
            return AINodeState.Succeeded;
        } 
        else
            return AINodeState.Running;
    }

    public override void Reset()
    {
        currentTimeWaiting = 0.0f;
    }
}

