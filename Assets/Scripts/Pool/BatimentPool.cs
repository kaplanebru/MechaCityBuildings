using System;
using UnityEngine;

public class BatimentPool : Pool<Transform>
{
    public int poolSize = 200;
    [SerializeField] private Transform prefab;
    

    public void InitializePool()
    {
        CreatePool(poolSize, transform, prefab);
    }
}
