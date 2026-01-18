using System.Collections.Generic;
using UnityEngine;

public class ReplacerSelected : ReplacerBase
{
    [SerializeField] private List<Placeholder> selectedPlaceholders = new();
    
    public void ReplaceSelected()
    {
        selectedPlaceholders.ForEach(p=>p.canBeCollectedRandomly = false);
        SetPlaceholderDatas(selectedPlaceholders);
        ReplaceGiven(ResolvePlaceholderDataSet(selectedPlaceholders).ToArray());
    }

    public override void ExecuteReplacements()
    {
        ReplaceSelected();
    }

    public override void ReleaseItemsToPool()
    {
        base.ReleaseItemsToPool();
        selectedPlaceholders.ForEach(p=>p.canBeCollectedRandomly = true);
    }
}
