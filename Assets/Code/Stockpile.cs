using System;
using System.Linq;
using System.Collections.Generic;

public static class Stockpile
{
    private static Dictionary<string, int> resources = new Dictionary<string, int>();

    public static void AddResource(string resource, int count)
    {
        if (resources.ContainsKey(resource))
            resources[resource] = resources[resource] + count;
        else
            resources.Add(resource, count);
    }

    public static void RemoveResource(string resource, int count)
    {
        if (resources.ContainsKey(resource))
        {
            resources[resource] = resources[resource] - count;
            if (resources[resource] <= 0)
                resources.Remove(resource);
        }
    }

    public static string GetResourceInfo()
    {
        return string.Join("\n", resources.Select(pair => pair.Key + ":\t" + pair.Value).ToArray());
    }
}

