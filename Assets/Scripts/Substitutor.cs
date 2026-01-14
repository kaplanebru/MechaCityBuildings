using System.Collections.Generic;
using UnityEngine;

public class Substitutor
{
    public Replacement[] Substitute(List<Placeholder> placeholders, Transform parent, ReplacementPool pool)
    {
        List<Replacement> replacements = new List<Replacement>();
        foreach (var placeholder in placeholders)
        {
            //pools[placeholder.replacementType].GetItem();
            var replacement = pool.GetItem();

            replacement.transform.position = placeholder.transform.position;
            replacement.transform.rotation = placeholder.transform.rotation;
            replacement.transform.localScale = placeholder.transform.localScale; //lossyScale before

            replacement.transform.SetParent(parent);
            replacements.Add(replacement);
        }

        return replacements.ToArray();
    }
    
}