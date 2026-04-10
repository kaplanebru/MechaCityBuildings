using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]

public class CellRegistry
{ 
    public static HashSet<SlotData> GetBoundaries(HashSet<SlotData> cellDataSet)
    {
        return cellDataSet.Where(cellData => cellData.Type == SlotType.Boundary).ToHashSet();
    }
   
}