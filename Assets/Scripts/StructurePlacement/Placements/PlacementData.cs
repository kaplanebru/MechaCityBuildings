using System;
using UnityEngine;

[Serializable]
public class PlacementData
{
    public StructureType StructureType; // {get; private set;}
    public int OrderIndex; // { get; private set; }
    
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    
    public StructureType GetStructureType() => StructureType;
    
    //For Randomizer
    //TODO: fix it for single applications as well, except from randomizer
    public void ApplyStructureType(StructureType type) => StructureType = type;
    public void SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;
    
    //For Substitutor
    public void SetTransformValues(Vector3 position, Quaternion rotation, Vector3 scale) 
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}