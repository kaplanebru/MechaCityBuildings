using System.Collections.Generic;
using UnityEngine;

public class ReplacerAuto : ReplacerBase
{
    [SerializeField] private Transform placeholderParent;
    private PlaceholderProvider _placeholderProvider = new();
    private List<Placeholder> _placeholders = new();


    private void SetPlaceholders()
    {
        _placeholderProvider.Initialize(placeholderParent);
        _placeholders = _placeholderProvider.GetPlaceholdersByType(replacementType);
        SetPlaceholderDatas(_placeholders);
    }

    public override void ExecuteReplacements()
    {
        SetPlaceholders();
        ReplaceGiven(CreatePlaceholderDataSet(_placeholders).ToArray());
    }
    
    /*public void SetPlaceholders(List<Placeholder> placeholders)
{
    _placeholders = placeholders;
    SetPlaceholderDatas(_placeholders);
}*/
    
    /*private void ReplaceAllFromScene()
   {
      // _placeholders = _placeholderProvider.GetPlaceholdersFromScene(replacementType);
      // SetPlaceholderDatas(_placeholders);
       ReplaceGiven(CreatePlaceholderDataSet(_placeholders).ToArray());
   }*/
}
