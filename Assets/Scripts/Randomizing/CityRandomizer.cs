using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RandomizerData
{
    public ReplacementType Type;
    [Range(0, 100)] public int Ratio = 50;
    public int Amount { get; set; }
}

public class CityRandomizer : MonoBehaviour
{
    [SerializeField] private RandomizerData[] randomizerDataSet;
    [SerializeField] private Transform placeholderParent;
    public ArrangementCache arrangementCache = new();
    private Randomizer _randomizer = new();
    private PlaceholderProvider _placeholderProvider;

    private List<PlaceholderData> _placeholderDataSet = new();

    private void GetAllPlaceholders()
    {
        _placeholderProvider = new(placeholderParent);
        _placeholderDataSet = _placeholderProvider.GetPlaceholderDataSet();
        _randomizer.Setup(randomizerDataSet, _placeholderDataSet);
    }
    
    public void MixAndApply()
    { 
        GetAllPlaceholders();
        _placeholderDataSet = _placeholderDataSet.OrderBy(_ => UnityEngine.Random.value).ToList();

        _randomizer.SetAmountsByRatio();
        ApplyTypesToPlaceholders();
    }

    
    private void ApplyTypesToPlaceholders()
    {
        int leftAmount = 0;
        int startAmount = 0;
        foreach (var randomizerData in randomizerDataSet)
        {
            startAmount = leftAmount;
            leftAmount = startAmount + randomizerData.Amount;

            for (int i = startAmount; i < leftAmount; i++)
            {
                _placeholderDataSet[i].SetType(randomizerData.Type);
            }
        }

        _randomizer.CheckForRest(leftAmount);
    }

   
    
    public void SaveCurrentArrangement(string arrangementName)
    {
        arrangementCache.Add(arrangementName, _placeholderDataSet.ToArray());
    }


    public void ResurrectArrangement(string arrangementName)
    {
        arrangementCache.ResurrectArrangement(arrangementName);
    }
}