using System.Collections.Generic;
using UnityEngine;

public class ReplacerAuto : ReplacerBase
{
    [SerializeField] private Transform placeholderParent;
    private List<Placeholder> _placeholders = new();
    private PlaceholderProvider _placeholderProvider;
    private List<PlaceholderData> _placeholderDataSet = new();


    private void SetPlaceholders()
    {
        _placeholders = _placeholderProvider.GetPlaceholdersByType(replacementType);
        SetPlaceholderDatas(_placeholders);
    }

    public override void ExecuteReplacements()
    {
        SetPlaceholders();
        ReplaceGiven(
            PlaceholderProvider.CreatePlaceholderDataSet(_placeholders).ToArray());
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
