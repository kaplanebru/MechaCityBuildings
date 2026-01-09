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
    public BatimentPool pool;
    private Substitutor substitutor = new Substitutor();
    
    private GameObject[] placeHolders;
    private List<Transform> transforms = new();

    public void Substitute()
    {
        if (!pool || _data.PoolSize <= 0 || !_data.Prefab)
        {
            Debug.LogWarning("Assign values!");
            return;
        }
        
        FindPlaceholdersAndSetTransforms();
        
        pool.InitializePool(_data.PoolSize, _data.Prefab);
        
        
        if (_data.PoolSize < transforms.Count)
        {
            Debug.LogWarning("Pool size is too small");
            return;
        }
        
        substitutor.Substitute(transforms.ToArray(), transform, pool);
    }

    private void FindPlaceholdersAndSetTransforms()
    {
        transforms.Clear();
        placeHolders = GameObject.FindGameObjectsWithTag("Collectable");
        
        foreach (var collectable in placeHolders)
        {
            transforms.Add(collectable.transform);
        }
    }
}