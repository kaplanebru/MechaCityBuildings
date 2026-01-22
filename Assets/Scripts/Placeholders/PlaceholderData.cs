using UnityEngine;

public class PlaceholderData
{
    public ReplacementType Type { get; private set; }
    public int OrderIndex { get; private set; }
    
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    
    //For Randomizer
    public void SetType(ReplacementType type) => Type = type;
    public void SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;
    
    //For Substitutor
    public void SetTransformValues(Vector3 position, Quaternion rotation, Vector3 scale) 
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}