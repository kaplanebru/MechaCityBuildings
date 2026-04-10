using System;
using UnityEngine;

[Serializable]
public class NeighborCell
{
    public Vector2Int Coords;
    public StructureType StructureType;
    
    public NeighborCell(Vector2Int coords)
    {
        Coords = coords;
    }
    
    public void SetStructureType(StructureType structureType)
    {
        StructureType = structureType;
    }
}