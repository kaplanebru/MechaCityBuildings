using System;
using UnityEngine;

public class BatimentPool : Pool<Transform>
{

    private int _poolSize;
    private Transform _prefab;
    public void Setup(int poolSize, Transform prefab)
    {
        _poolSize = poolSize;
        _prefab = prefab;
    }

    public void InitializePool()
    {
        CreatePool(_poolSize, transform, _prefab);
    }
}
