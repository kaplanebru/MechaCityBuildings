using System.Collections.Generic;
using UnityEngine;

public class Substitutor
{
    public Replacement[] Substitute(PlaceholderData[] placeholderDataSet, Transform parent, ReplacementPool pool)
    {

        List<Replacement> replacements = new List<Replacement>();
        foreach (var placeholderData in placeholderDataSet)
        {
            var replacement = pool.GetItem();
            replacements.Add(replacement);
            replacement.transform.position = placeholderData.Position;
            replacement.transform.rotation = placeholderData.Rotation;
            replacement.transform.localScale = placeholderData.Scale;

            replacement.transform.SetParent(parent);
           

        }

        Debug.Log("...");
        return replacements.ToArray();
    }
    
}