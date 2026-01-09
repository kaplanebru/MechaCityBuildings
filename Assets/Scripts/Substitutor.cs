using System.Collections.Generic;
using UnityEngine;

public class Substitutor
{
    public void Substitute(Transform[] transforms, Transform parent, BatimentPool pool)
    {
        foreach (var transform in transforms)
        {
            var newPrefab = pool.GetItem();

            newPrefab.transform.position = transform.position;
            newPrefab.transform.rotation = transform.rotation;
            newPrefab.localScale = transform.lossyScale;

            newPrefab.transform.SetParent(parent);
        }
    }
    
}