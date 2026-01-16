using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ReplacementType
{
    RightBatiment,
    LeftBatiment,
    Stairs,
    VariedBatiment
}

[Serializable]
public class PoolData
{
    public int PoolSize = 200;
    public Replacement Prefab;
}

public abstract class ReplacementMediator : MonoBehaviour
{
    [SerializeField] protected ReplacementType replacementType;
    [SerializeField] private PoolData poolData;
    public bool randomizable = true;


    public ReplacementPool pool;
    public Transform parent;
    
    private Substitutor substitutor = new Substitutor();
    public Replacement[] Replacements { get; set; }

    public abstract void ExecuteReplacements();
    protected void Substitute(List<Placeholder> placeholders)
    {
        if (pool.transform.childCount == 0)
            pool.InitializePool(poolData);

        if (poolData.PoolSize < placeholders.Count)
        {
            Debug.LogWarning("Pool size is too small for " + replacementType);
            return;
        }

        Replacements = substitutor.Substitute(
            placeholders,
            parent,
            pool);
    }

    public virtual void ReleaseItemsToPool()
    {
        pool.ReleaseItemsToPool(Replacements);
    }
    
}