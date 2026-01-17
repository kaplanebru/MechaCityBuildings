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

    public void Subscribe()
    {
        Eventbus.OnReplacementRequest += ReplaceSaved;
    }

    public void Unsubscribe()
    {
        Eventbus.OnReplacementRequest -= ReplaceSaved;
    }

    private void ReplaceSaved(ReplacementType type, PlaceholderData[] placeholderDataSet)
    {
        if(type != replacementType) return;
        Replace(placeholderDataSet);
    }
    protected void Replace(PlaceholderData[] placeholderDataSet)
    {
        if (pool.transform.childCount == 0)
            pool.InitializePool(poolData);
        else
            ReleaseItemsToPool(); //todo: test

        if (poolData.PoolSize < placeholderDataSet.Length)
        {
            Debug.LogWarning("Pool size is too small for " + replacementType);
            return;
        }

        Replacements = substitutor.Substitute(
            placeholderDataSet,
            parent,
            pool);
    }

    protected void SetPlaceholderDatas(List<Placeholder> placeholders)
    {
        placeholders.ForEach(p=>p.SetDataTransformValues());
    }
    protected List<PlaceholderData> ResolvePlaceholderDataSet(List<Placeholder> placeholders)
    {
        return placeholders.Select(placeholder => placeholder.data).ToList();
    }

    public virtual void ReleaseItemsToPool()
    {
        if (pool.pool.Count == 0)
            return;
        pool.ReleaseItemsToPool(Replacements);
    }
    
}