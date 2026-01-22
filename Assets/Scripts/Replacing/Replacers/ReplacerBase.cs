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

public abstract class ReplacerBase : MonoBehaviour
{
    public ReplacementType replacementType;
    [SerializeField] private PoolData poolData;
    public bool randomizable = true;


    public ReplacementPool pool;
    public Transform parent;
    
    private Substitutor substitutor = new Substitutor();
    public Replacement[] Replacements { get; set; }

    public abstract void ExecuteReplacements();
    
    public void ReplaceGiven(PlaceholderData[] placeholderDataSet)
    {
        pool.CheckPoolActivity();
        
        if(!pool.IsInitialized())//if (pool.transform.childCount == 0)
            pool.InitializePool(poolData);
        else
            ReleaseItemsToPool(); 
        
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
        if ( Replacements == null) //Replacements.Length == 0 ||
        {
            print("yes");
            Replacements = FindObjectsByType<Replacement>(FindObjectsSortMode.None).Where(r=>r.type == replacementType).ToArray();
        }
        pool.ReleaseItemsToPool(Replacements);
    }
    
}