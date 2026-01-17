using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class ArrangementData
{
    public string Name;
    public Dictionary<string, ReplacementType> BuildingsByType { get; private set; } = new();
    public ArrangementData(string name, Dictionary<string, ReplacementType> buildingsByType)
    {
        Name = name;
        BuildingsByType = buildingsByType;
    }
}
public class ArrangementCache
{
    public Dictionary<string, ArrangementData> arrangements { get; private set; } = new ();
    public string[] GetNames() => arrangements.Keys.ToArray();

    private bool IsNameTaken(string name)
    {
        return arrangements.ContainsKey(name);
    }

    public void Add(string name, Dictionary<string, ReplacementType> buildingsByType)
    {
        if (IsNameTaken(name))
        {
            Debug.LogWarning($"Name {name} is already taken");
            return;
        }
        var arrangement = new ArrangementData(name, buildingsByType);
        arrangements.Add(arrangement.Name, arrangement);
    }

    public void Remove(string name)
    {
        if(IsNameTaken(name))
            arrangements.Remove(name);
        else
        {
            Debug.LogWarning($"Name {name} doesn't exist");
        }
    }
}


