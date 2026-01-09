using System.Collections.Generic;
using UnityEngine;

public class Substitutor
{
    private BatimentPool _pool;

    public Substitutor(BatimentPool pool)
    {
        _pool = pool;
    }

    public void Substitute(Transform[] transforms, Transform parent)
    {
        foreach (var transform in transforms)
        {
            var newPrefab = _pool.GetItem();

            newPrefab.transform.position = transform.position;
            newPrefab.transform.rotation = transform.rotation;
            newPrefab.localScale = transform.lossyScale;

            newPrefab.transform.SetParent(parent);
        }
    }
}