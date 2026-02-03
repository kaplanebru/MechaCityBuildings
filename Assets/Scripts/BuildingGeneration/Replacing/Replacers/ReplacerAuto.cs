using System.Collections.Generic;
using UnityEngine;

public class ReplacerAuto : ReplacerBase
{
    private List<PlaceholderData> _placeholderDataSet = new();
    
    public override void ExecuteReplacements()
    {
        _placeholderDataSet = PlaceholderProvider.GetPlaceholderDataSetByType(replacementType);
        ReplaceGiven(_placeholderDataSet.ToArray());
    }
}
