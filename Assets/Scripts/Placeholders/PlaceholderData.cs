using UnityEngine;

public class PlaceholderData
{
    public ReplacementData ReplacementData;
    public int OrderIndex { get; private set; }
    
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;

    public ReplacementType GetReplacementType() => ReplacementData.Type;
    
    //For Randomizer
    //TODO: fix it for single applications as well, except from randomizer
    public void ApplyReplacementData(ReplacementData data) => ReplacementData = data;
    public void SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;
    
    //For Substitutor
    public void SetTransformValues(Vector3 position, Quaternion rotation, Vector3 scale) 
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}