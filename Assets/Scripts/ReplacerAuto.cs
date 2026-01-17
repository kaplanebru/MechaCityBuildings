using System.Collections.Generic;
using UnityEngine;

public class ReplacerAuto : ReplacementMediator
{
    private PlaceholderProvider _placeholderProvider = new();
    private List<Placeholder> _placeholders = new();
    
    private void ReplaceAllFromScene()
    {
        _placeholders = _placeholderProvider.GetPlaceholdersFromScene(replacementType);
        SetPlaceholderDatas(_placeholders);
        Replace(ResolvePlaceholderDataSet(_placeholders).ToArray());
    }

   

    public override void ExecuteReplacements()
    {
        ReplaceAllFromScene();
    }
}
