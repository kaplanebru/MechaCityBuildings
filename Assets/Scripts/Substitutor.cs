using System.Collections.Generic;
using UnityEngine;

public class Substitutor
{
    public void Substitute(List<Placeholder> placeholders, Transform parent, ReplacementPool pool)
    {
        foreach (var placeholder in placeholders)
        {
            //pools[placeholder.replacementType].GetItem();
            var newPrefab = pool.GetItem();

            newPrefab.transform.position = placeholder.transform.position;
            newPrefab.transform.rotation = placeholder.transform.rotation;
            newPrefab.transform.localScale = placeholder.transform.localScale; //lossyScale before

            newPrefab.transform.SetParent(parent);
        }
    }
    
}