using System;
using UnityEngine;

[Serializable]
public class NeighborCellPoint
{
    public Vector2Int Coords;
    public StructureType StructureType;
    
    public NeighborCellPoint(Vector2Int coords)
    {
        Coords = coords;
    }
    
    public void SetStructureType(StructureType structureType)
    {
        StructureType = structureType;
    }
}