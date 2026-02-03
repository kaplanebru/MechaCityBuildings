using UnityEngine;

public class PlaceholderData
{
    public ReplacementType ReplacementType;
    public int OrderIndex { get; private set; }
    
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public ReplacementType GetReplacementType() => ReplacementType;
    
    //For Randomizer
    //TODO: fix it for single applications as well, except from randomizer
    public void ApplyReplacementType(ReplacementType type) => ReplacementType = type;
    public void SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;
    
    //For Substitutor
    public void SetTransformValues(Vector3 position, Quaternion rotation, Vector3 scale) 
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}