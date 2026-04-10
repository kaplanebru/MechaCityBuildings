using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ArrangementData
{
    public string Name;
    public Dictionary<StructureType, List<SlotData>> CategorizedBuildings { get; private set; } = new();
    public ArrangementData(string name, SlotData[] savedBuildings)
    {
        Name = name;
        CategorizeBuildings(savedBuildings);
    }

    private void CategorizeBuildings(SlotData[] savedBuildings)
    {
        foreach (var savedBuilding in savedBuildings)
        {
            if(!CategorizedBuildings.ContainsKey(savedBuilding.GetStructureType()))
                CategorizedBuildings.Add(savedBuilding.GetStructureType(), new List<SlotData>());
            
            var buildingGroup = CategorizedBuildings[savedBuilding.GetStructureType()];
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

    public void Add(string name, SlotData[] savedBuildings)
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
        var categorizedBuildings = 
            GetArrangement(arrangementName).CategorizedBuildings;
        
        Eventbus.OnReplacementWithSavedRequest?.Invoke(categorizedBuildings);
    }
}


