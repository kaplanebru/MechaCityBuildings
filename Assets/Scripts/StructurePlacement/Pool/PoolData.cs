using System;
using UnityEngine;

[Serializable]
public class PoolData
{
    public int PoolSize = 200;
    public StructureType StructureType;
    public Structure Prefab;
    
    public bool CheckPoolStructureTypeConformation()
    {
        if (Prefab == null)
        {
            Debug.LogError("Pool has no prefab assigned.");
            return false;
        }
        if (StructureType != Prefab.type)
        {
            Debug.LogError($"Pool type {StructureType} doesn't match prefab type {Prefab.type}.");
            return false;
        }
        return true;
    }
}