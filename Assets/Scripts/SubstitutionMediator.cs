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
    [SerializeField] private List<Placeholder> selectedPlaceholders = new();
    
    private List<Placeholder> _placeholdersProvidedFromScene = new();
    private Substitutor substitutor = new Substitutor();
    private PlaceholderProvider _placeholderProvider = new();
    public Replacement[] Replacements { get; set; }

    public void ReplaceSelected()
    {
        selectedPlaceholders.ForEach(p=>p.canBeCollectedRandomly = false);
        Replace(selectedPlaceholders);
    }
    public void ReplaceAllFromScene()
    {
        _placeholdersProvidedFromScene = _placeholderProvider.GetPlaceholdersFromScene(replacementType);
        Replace(_placeholdersProvidedFromScene);
    }

    private void Replace(List<Placeholder> placeholders)
    {
        if (pool.transform.childCount == 0)
            pool.InitializePool(poolData);

        if (poolData.PoolSize < _placeholdersProvidedFromScene.Count)
        {
            Debug.LogWarning("Pool size is too small for " + replacementType);
            return;
        }

        Replacements = substitutor.Substitute(
            placeholders,
            parent,
            pool);
    }
    
}