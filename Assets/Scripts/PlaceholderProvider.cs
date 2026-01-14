using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceholderProvider 
{
    public List<Placeholder> GetPlaceholdersFromScene(ReplacementType replacementType)
    {
        List<Placeholder> placeholders = new();

        placeholders = Object.FindObjectsByType<Placeholder>(FindObjectsSortMode.None).
            Where(p=> p.canBeCollectedRandomly && p.replacementType == replacementType).ToList();
        
        Debug.Log(placeholders.Count);

        return placeholders;
    }
}
