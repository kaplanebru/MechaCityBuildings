using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceholderProvider 
{
    public List<Placeholder> GetPlaceholdersFromScene(ReplacementType replacementType)
    {
        List<Placeholder> placeholders = new();

        placeholders = Object.FindObjectsByType<Placeholder>(FindObjectsSortMode.None).
            Where(p=> p.canBeCollectedRandomly && p.data.Type == replacementType).ToList();
        
        return placeholders;
    }
}
