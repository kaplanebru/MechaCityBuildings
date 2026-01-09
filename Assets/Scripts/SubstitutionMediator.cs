using System.Collections.Generic;
using UnityEngine;

public class SubstitutionMediator : MonoBehaviour
{
    [SerializeField] private BatimentPool batimentPool;
    
    private GameObject[] placeHolders;
    private List<Transform> transforms = new();
    private Substitutor substitutor;

    void Start()
    {
        batimentPool.InitializePool();
        GetCollectables();
        substitutor = new Substitutor(batimentPool);
        substitutor.Substitute(transforms.ToArray(), transform);

        //substitutor = new Substitutor(placeHolderPb);
        //GetCollectables();
        // substitutor.Substitute(collectables, transform);
    }

    void GetCollectables()
    {
        placeHolders = GameObject.FindGameObjectsWithTag("Collectable");
        foreach (var collectable in placeHolders)
        {
            transforms.Add(collectable.transform);
        }
    }
}