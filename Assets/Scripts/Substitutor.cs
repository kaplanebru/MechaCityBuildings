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
        if (_pool.poolSize < transforms.Length)
        {
            Debug.LogWarning("Pool size is too small");
            return;
        }
        
        foreach (var transform in transforms)
        {
            var newPrefab = _pool.GetItem();
            
            newPrefab.transform.position = transform.position;
            newPrefab.transform.rotation = transform.rotation;
            newPrefab.localScale = transform.lossyScale;
            
            newPrefab.transform.SetParent(parent);
        }
    }
    

    /*public void Substitute(GameObject[] collectables, Transform parent)
    {
        SetPositions(collectables);

        foreach (var collectableTransform in _collectableTransforms)
        {
            var newPrefab = Object.Instantiate(_placeHolderPb, collectableTransform.position, collectableTransform.rotation);
            newPrefab.transform.localScale  = collectableTransform.transform.localScale;
            newPrefab.transform.SetParent(parent);
        }
    }*/
    
    
    
}
