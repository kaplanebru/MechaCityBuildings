using System;
using UnityEngine;

public class ReplacementPool : Pool<Replacement>
{
   
    public void InitializePool(PoolData data)
    {
        ClearPool();
        CreatePool(data.PoolSize, transform, data.Prefab);
    }
}
