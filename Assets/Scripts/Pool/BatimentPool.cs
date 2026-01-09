using System;
using UnityEngine;

public class BatimentPool : Pool<Transform>
{
    public void InitializePool(int poolSize, Transform prefab)
    {
        ClearPool();
        CreatePool(poolSize, transform, prefab);
    }
}
