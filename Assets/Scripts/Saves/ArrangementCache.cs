using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ArrangementData
{
    public string Name;
    public Dictionary<ReplacementType, List<PlaceholderData>> CategorizedBuildings { get; private set; } = new();
    public ArrangementData(string name, PlaceholderData[] savedBuildings)
    {
        Name = name;
        CategorizeBuildings(savedBuildings);
    }

    private void CategorizeBuildings(PlaceholderData[] savedBuildings)
    {
        foreach (var savedBuilding in savedBuildings)
        {
            if(!CategorizedBuildings.ContainsKey(savedBuilding.GetReplacementType()))
                CategorizedBuildings.Add(savedBuilding.GetReplacementType(), new List<PlaceholderData>());
            
            var buildingGroup = CategorizedBuildings[savedBuilding.GetReplacementType()];
            buildingGroup.Add(savedBuilding);
        }
    }
}
public class ArrangementCache
{
    public Dictionary<string, ArrangementData> arrangements { get; private set; } = new ();
    public string[] RefreshNames() => arrangements.Keys.OrderBy(k => k).ToArray();
    
    public ArrangementData GetArrangement(string name) => arrangements[name];

    private bool IsNameTaken(string name)
    {
        return arrangements.ContainsKey(name);
    }

    public void Add(string name, PlaceholderData[] savedBuildings)
    {
        if (IsNameTaken(name))
        {
            Debug.LogWarning($"Name {name} is already taken");
            return;
        }
        var arrangement = new ArrangementData(name, savedBuildings);
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
    
    public void ResurrectArrangement(string arrangementName)
    {
        var categorizedBuildings = GetArrangement(arrangementName).CategorizedBuildings;
        
        Eventbus.OnReplacementWithSavedRequest?.Invoke(categorizedBuildings);
    }
}


