using System;
using UnityEngine;

public class PlaceholderData
{
    public ReplacementType Type { get; private set; }
    public int District { get; private set; }
    
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    
    //For Randomizer
    public void SetType(ReplacementType type) => Type = type;
    public void SetDistrict(int district) => District = district;
    
    //For Substitutor
    public void SetTransformValues(Vector3 position, Quaternion rotation, Vector3 scale) 
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}
public class Placeholder : MonoBehaviour
{
    public PlaceholderData data = new();
    public int district;
    public bool canBeCollectedRandomly = true;
    public bool canBeOrderedRandomly = true;

    public void SetDataTransformValues()
    {
        data.SetTransformValues(transform.position, transform.rotation, transform.localScale);
        data.SetDistrict(district);
    }
}

