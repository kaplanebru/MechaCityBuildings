using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SubstitutionData
{
    public int PoolSize = 200;
    public Transform Prefab;
}
public class SubstitutionMediator : MonoBehaviour
{
    [SerializeField] private SubstitutionData _data;
    [SerializeField] private BatimentPool pool;
    
    private Substitutor substitutor;
    private GameObject[] placeHolders;
    private List<Transform> transforms = new();

    public void Substitute()
    {
        pool.Setup(_data.PoolSize, _data.Prefab);
        pool.InitializePool();
        substitutor = new Substitutor(pool);
        
        GetPlaceholders();
        
        if (_data.PoolSize < transforms.Count)
        {
            Debug.LogWarning("Pool size is too small");
            return;
        }
        
        substitutor.Substitute(transforms.ToArray(), transform);
    }

    public void GetPlaceholders()
    {
        transforms.Clear();
        placeHolders = GameObject.FindGameObjectsWithTag("Collectable");
        
        foreach (var collectable in placeHolders)
        {
            transforms.Add(collectable.transform);
        }
    }
    
}