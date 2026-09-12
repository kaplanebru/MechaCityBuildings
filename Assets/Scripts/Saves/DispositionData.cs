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


