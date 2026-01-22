using System.Collections.Generic;
using UnityEngine;

public class ReplacerAuto : ReplacerBase
{
    [SerializeField] private Transform placeholderParent;
    private List<PlaceholderData> _placeholderDataSet = new();
    
    public override void ExecuteReplacements()
    {
        _placeholderDataSet = PlaceholderProvider.GetPlaceholderDataSetByType(replacementType);
        ReplaceGiven(_placeholderDataSet.ToArray());
    }
}
