using System.Collections.Generic;
using UnityEngine;

public class ReplacerAuto : ReplacerBase
{
    private PlaceholderProvider _placeholderProvider = new();
    private List<Placeholder> _placeholders = new();
    
    private void ReplaceAllFromScene()
    {
        _placeholders = _placeholderProvider.GetPlaceholdersFromScene(replacementType);
        SetPlaceholderDatas(_placeholders);
        ReplaceGiven(ResolvePlaceholderDataSet(_placeholders).ToArray());
    }

   

    public override void ExecuteReplacements()
    {
        ReplaceAllFromScene();
    }
}
