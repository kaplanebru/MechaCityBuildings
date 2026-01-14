using System.Collections.Generic;
using UnityEngine;

public class ReplacerSelected : ReplacementMediator
{
    [SerializeField] private List<Placeholder> selectedPlaceholders = new();
    
    public void ReplaceSelected()
    {
        selectedPlaceholders.ForEach(p=>p.canBeCollectedRandomly = false);
        Substitute(selectedPlaceholders);
    }

    public override void ExecuteReplacements()
    {
        ReplaceSelected();
    }
}
