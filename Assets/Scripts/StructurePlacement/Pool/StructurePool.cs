using System;
using UnityEngine;

public class StructurePool : Pool<Structure>
{
    public PoolData poolData;
    public void InitializePool()
    {
        ClearPool();
        if(poolData.CheckPoolStructureTypeConformation()) 
            CreatePool(poolData.PoolSize, transform, poolData.Prefab);
    }
}
