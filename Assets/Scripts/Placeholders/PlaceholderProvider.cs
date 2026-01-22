using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlaceholderProvider 
{
    private List<Placeholder> placeholders = new();
    private Transform _placeholderParent;


    public PlaceholderProvider(Transform placeholderParent)
    {
        _placeholderParent = placeholderParent;
    }

    public void GetAllPlaceholders()
    {
        placeholders = _placeholderParent.GetComponentsInChildren<Placeholder>()
            .Where(p => p.canBeCollectedRandomly)
            .ToList();
    }

    public List<PlaceholderData> GetPlaceholderDataSet()
    {
        GetAllPlaceholders();
        return placeholders.Select(placeholder => placeholder.data).ToList();
    }
    public List<Placeholder> GetPlaceholdersByType(ReplacementType replacementType)
    {
        GetAllPlaceholders();
        return placeholders.Where(p => p.data.Type == replacementType).ToList();
    }
    
    public static List<PlaceholderData> CreatePlaceholderDataSet(List<Placeholder> placeholders)
    {
        return placeholders.Select(placeholder => placeholder.data).ToList();
    }
    

 
}
