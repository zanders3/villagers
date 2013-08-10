using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents an AI character and it's current knowledge of the world state.
/// 
/// Implements the steering AI layer.
/// </summary>
public class AICharacter : Character
{
    private AINode behaviourTree;
    private Vector2 steerForce;

    public GameObject CurrentSelection = null;
    public GameObject CurrentAssignment = null;
    public string HeldItem = "None";

    /// <summary>
    /// Seek toward the specified position in the next Update tick.
    /// </summary>
    /// <param name="position">Position to seek towards</param>
    /// <returns>Distance from target position</returns>
    public float Seek(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        direction.y = 0.0f;

        Vector2 desiredVelocity = (new Vector2(direction.x, direction.z) * MaxSpeed) - (velocity * 0.4f);
        steerForce += desiredVelocity * MaxForce;

        return direction.magnitude;
    }

    /// <summary>
    /// Sets the behaviour tree for this character.
    /// </summary>
    public void SetTree(AINode behaviourTree)
    {
        this.behaviourTree = behaviourTree;
    }
    
    protected override Vector2 Steer()
    {
        if (behaviourTree != null)
            behaviourTree.Run(this);

        Vector2 steering = steerForce;
        steerForce = Vector2.zero;
        return steering;
    }

    void OnGUI()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 800.0f, 20.0f), ToString());
    }

    public override string ToString()
    {
        return behaviourTree != null ? behaviourTree.ToString() : "";
    }
}
