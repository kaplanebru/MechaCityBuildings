using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]

public class CellRegistry
{ 
    public static HashSet<CellData> GetBoundaries(HashSet<CellData> cellDataSet)
    {
        return cellDataSet.Where(cellData => cellData.Type == CellType.Boundary).ToHashSet();
    }
   
}