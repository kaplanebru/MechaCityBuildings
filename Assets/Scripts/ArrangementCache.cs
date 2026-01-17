using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ArrangementData
{
    public string Name;
    public PlaceholderData[] SavedBuildings { get; private set; }
    public ArrangementData(string name, PlaceholderData[] savedBuildings)
    {
        Name = name;
        SavedBuildings = savedBuildings;
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
        var savedBuildings = GetArrangement(arrangementName).SavedBuildings;
        Eventbus.OnReplacementRequest?.Invoke(savedBuildings);
        
        //TODO: SUBSTITUTE
        //typelara ayır: ona göre pool'a event publish et
       
    }
}


