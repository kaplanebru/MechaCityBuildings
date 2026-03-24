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
        if (StructureType != Prefab.type)
        {
            Debug.LogError("The type of this pool doesn't match");
            return false;
        }
        return true;
    }
}