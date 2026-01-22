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
    public ArrangementCache arrangementCache = new();
    private Randomizer _randomizer = new();

    private List<Placeholder> _placeholders = new();
    private List<PlaceholderData> _placeholderDataSet = new();

    private void GetAllPlaceholders()
    {
        _placeholders.Clear();
        _placeholders = FindObjectsByType<Placeholder>(FindObjectsSortMode.None)
            .Where(p => p.canBeOrderedRandomly).ToList();
        _placeholderDataSet = FillPlaceholderDataSet(_placeholders);
        _randomizer.Setup(randomizerDataSet, _placeholderDataSet);
    }
    
    //TODO: HELPER YAP
    private List<PlaceholderData> FillPlaceholderDataSet(List<Placeholder> placeholders)
    {
        return placeholders.Select(placeholder => placeholder.data).ToList();
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