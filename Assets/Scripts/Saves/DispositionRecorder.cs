using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class DispositionData
{
    public string Name;
    public List<List<SlotData>> SlotsByFloor = new();

    //public Dictionary<StructureType, List<SlotData>> DispositionSlots { get; private set; } = new();
    public DispositionData(string name, List<List<SlotData>> slotsByFloor)
    {
        Name = name;
        SlotsByFloor = slotsByFloor;
    }
    
}
public class DispositionRecorder
{
    public Dictionary<string, DispositionData> dispositionsByName { get; private set; } = new ();
    public string[] RefreshNames() => dispositionsByName.Keys.OrderBy(k => k).ToArray();
    
    public DispositionData GetDispositionSlots(string name) => dispositionsByName[name];
    
    public void ResurrectArrangement(string dispositionName)
    {
        var slotsByFloor = GetDispositionSlots(dispositionName).SlotsByFloor;
        Eventbus.OnDispositionActivationRequest?.Invoke(slotsByFloor);
        //todo: city builder will listen it, not from eventbys btw. and resurrects from slots.
    }

    private bool IsNameTaken(string name)
    {
        return dispositionsByName.ContainsKey(name);
    }

    public void Add(DispositionData dispositionData)
    {
        if (IsNameTaken(dispositionData.Name))
        {
            Debug.LogWarning($"Name {dispositionData.Name} is already taken");
            return;
        }

        var disposition = dispositionData;
        dispositionsByName.Add(disposition.Name, disposition);
    }

    public void Remove(string name)
    {
        if(IsNameTaken(name))
            dispositionsByName.Remove(name);
        else
            Debug.LogWarning($"Disposition {name} doesn't exist");
    }
}


