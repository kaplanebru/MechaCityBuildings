using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ReplacementType
{
    RightBatiment,
    LeftBatiment,
    Stairs
}

[Serializable]
public class PoolData
{
    public int PoolSize = 200;
    public Replacement Prefab;
}

public class SubstitutionMediator : MonoBehaviour
{
    [SerializeField] private ReplacementType replacementType;
    [SerializeField] private PoolData poolData;
    
    public ReplacementPool pool;
    public Transform parent;

    private Substitutor substitutor = new Substitutor();
    private Placeholder[] collectables;
    private List<Placeholder> _placeholders = new();

    public void Substitute()
    {
        CollectPlaceholders();
        
        pool.InitializePool(poolData);

        if (poolData.PoolSize < _placeholders.Count)
        {
            Debug.LogWarning("Pool size is too small for " + replacementType);
            return;
        }

        substitutor.Substitute(
            _placeholders,
            parent,
            pool);
    }

    private void CollectPlaceholders()
    {
        _placeholders.Clear();

        _placeholders = FindObjectsByType<Placeholder>(FindObjectsSortMode.None).
            Where(p=> p.canBeCollectedRandomly && p.replacementType == replacementType).ToList();
        
        _placeholders = FindObjectsByType<Placeholder>(FindObjectsSortMode.None).Where(p=> p.replacementType == ReplacementType.Stairs).ToList();
        print(_placeholders.Count);
    }
}