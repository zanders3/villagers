using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Loads the AI tree definitions from the characters.txt file.
/// Instantiates AI characters.
/// </summary>
public static class AILoader
{
    private static string[] charactersFile;

    private static AINode[] ParseChildren(ref int filePos)
    {
        int currentLineNumTabs = charactersFile[filePos].Count(c => c == '\t');

        filePos++;
        int initialNumTabs = charactersFile[filePos].Count(c => c == '\t');

        Debug.Log("ParseChildren: " + charactersFile[filePos] + " " + initialNumTabs);

        if (initialNumTabs != currentLineNumTabs + 1)
            throw new Exception("Expected at least one child selector node (incorrect number of tabs) on line " + filePos+1);

        List<AINode> children = new List<AINode>();

        for (; filePos < charactersFile.Length; filePos++)
        {
            int numTabs = charactersFile[filePos].Count(c => c == '\t');
            if (numTabs == initialNumTabs)
                children.Add(Parse(ref filePos));
            else if (numTabs < initialNumTabs)
                break;
            else
                throw new Exception("WAT" + charactersFile[filePos] + " " + filePos);
        }

        filePos--;
        Debug.Log("NumChildren: " + children.Count);

        return children.ToArray();
    }

    private static AINode Parse(ref int filePos)
    {
        string line = charactersFile[filePos];
        string[] parts = line.Split(':');
        string[] parameters = parts.Length >= 2 ? parts[1].Split(',') : new string[0];

        string name = parts[0].Trim();
        Type type = Type.GetType(name, false);
        if (type == null)
            throw new Exception("Unknown AI node type: " + name);

        if (name.Contains("Selector"))
        {
            if (parameters.Length != 1)
                throw new Exception("Expected " + name + ":name on line " + filePos);

            return (AINode)type.GetConstructors()[0].Invoke(new object[]
            {
                parameters[0],
                ParseChildren(ref filePos)
            });
        } 
        else
        {
            ConstructorInfo info = type.GetConstructors()[0];
            if (info.GetParameters().Length != parameters.Length)
                throw new Exception("Expected " + parts[0] + ":" + string.Join(",", info.GetParameters().Select(p => p.Name).ToArray()));

            return (AINode)type.GetConstructors()[0].Invoke(parameters);
        }
    }

    public static AINode ParseCharacterTree(string name)
    {
        if (charactersFile == null)
            charactersFile = ((TextAsset)Resources.Load("characters")).text.Split('\n');
        
        AINode behaviourTree = null;
        for (int i = 0; i<charactersFile.Length; i++)
        {
            if (charactersFile[i].Length > 0 && charactersFile[i][0] == '#')
            {
                i++;
                behaviourTree = Parse(ref i);
                break;
            }
        }
        
        if (behaviourTree == null)
            throw new Exception("Failed to find " + name + " character definition.");

        return behaviourTree;
    }

    public static void CreateCharacter(string prefabName, string name, Vector3 position)
    {
        GameObject obj = (GameObject)GameObject.Instantiate((GameObject)Resources.Load(prefabName), position, Quaternion.identity);

        AICharacter character = obj.GetComponent<AICharacter>() ?? obj.AddComponent<AICharacter>();
        character.SetTree(ParseCharacterTree(name));
    }
}

