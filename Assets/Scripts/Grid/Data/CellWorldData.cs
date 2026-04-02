using UnityEngine;

public struct CellWorldData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    
    public CellWorldData(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}