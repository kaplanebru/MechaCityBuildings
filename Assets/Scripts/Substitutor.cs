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
            if(replacements.Count == 1)
                Debug.Log("repacement pos: " + replacements[0].transform.position);

            replacement.transform.position = placeholderData.Position;
            replacement.transform.rotation = placeholderData.Rotation;
            replacement.transform.localScale = placeholderData.Scale;

            replacement.transform.SetParent(parent);
           

        }

        Debug.Log("placeholder pos: " + placeholderDataSet[0].Position);
        Debug.Log("...");

        return replacements.ToArray();
    }
    
}