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

public class SceneRandomizer : MonoBehaviour
{
    [SerializeField] private RandomizerData[] randomizerDataSet;
    public ArrangementCache arrangementCache = new();

    private List<Placeholder> _placeholders = new();
    private List<PlaceholderData> _placeholderDataSet = new();

    private void GetAllPlaceholders()
    {
        _placeholders.Clear();
        _placeholders = FindObjectsByType<Placeholder>(FindObjectsSortMode.None)
            .Where(p => p.canBeOrderedRandomly).ToList();
        _placeholderDataSet = ResolvePlaceholderDataSet(_placeholders);
    }
    
    //TODO: HELPER YAP
    private List<PlaceholderData> ResolvePlaceholderDataSet(List<Placeholder> placeholders)
    {
        return placeholders.Select(placeholder => placeholder.data).ToList();
    }

    public void MixAndApply()
    {
        //if (_placeholders.Count == 0)
        GetAllPlaceholders();

        _placeholderDataSet = _placeholderDataSet.OrderBy(_ => UnityEngine.Random.value).ToList();

        SetAmountsByRatio();
        ApplyTypes();
    }

    
    private void ApplyTypes()
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

        CheckForRest(leftAmount);
    }

    private int GetRatioSum()
    {
        return randomizerDataSet.Sum(d => d.Ratio);
    }

    private void SetAmountsByRatio()
    {
        int totalAmount = _placeholderDataSet.Count;
        float ratioSum = GetRatioSum();

        foreach (var data in randomizerDataSet)
        {
            data.Amount = Mathf.FloorToInt(totalAmount * data.Ratio / ratioSum);
        }
    }

    private void CheckForRest(int rest)
    {
        if (rest == _placeholderDataSet.Count) return;

        for (int i = _placeholderDataSet.Count - 1; i >= rest; i--)
        {
            _placeholderDataSet[i].SetType(randomizerDataSet.Last().Type);
        }
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