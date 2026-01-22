using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceholderProvider 
{
    List<Placeholder> placeholders = new();
    
    public void Initialize(Transform parent)
    {
        placeholders = parent.GetComponentsInChildren<Placeholder>()
            .Where(p => p.canBeCollectedRandomly)
            .ToList();
        
    }

    public List<Placeholder> GetPlaceholdersByType(ReplacementType replacementType)
    {
        return placeholders.Where(p => p.data.Type == replacementType).ToList();
    }
}
