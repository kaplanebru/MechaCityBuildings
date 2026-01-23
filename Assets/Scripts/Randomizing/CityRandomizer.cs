using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityRandomizer : MonoBehaviour
{
    [SerializeField] private RandomizerData[] randomizerDataSet;
    [SerializeField] private CityData cityData;
    [SerializeField] private Transform placeholderRoot;

    public ArrangementCache arrangementCache = new();
    private FrequencyToAmountConverter _frequencyToAmountConverter = new();
    private CityOrderRegulator _orderRegulator;

    private List<PlaceholderData> _placeholderDataSet = new();

    private void GetAllPlaceholders()
    {
        _orderRegulator = new CityOrderRegulator(cityData.HeightGap);
        _placeholderDataSet = _orderRegulator.GetRegulatedPlaceholdersData().ToList();

        FrequencyData[] frequencyDatas = randomizerDataSet.Select(r => r.FrequencyData).ToArray();
        _frequencyToAmountConverter.Setup(frequencyDatas, _placeholderDataSet.Count);
    }

    public void MixAndApply()
    {
        GetAllPlaceholders();
        _placeholderDataSet = _placeholderDataSet.OrderBy(_ => UnityEngine.Random.value).ToList();

        _frequencyToAmountConverter.SetAmountsByRatio();
        ApplyTypesToPlaceholders();
    }


    private void ApplyTypesToPlaceholders()
    {
        int leftAmount = 0;
        int startAmount = 0;
        foreach (var randomizerData in randomizerDataSet)
        {
            startAmount = leftAmount;
            leftAmount = startAmount + randomizerData.FrequencyData.Amount;

            for (int i = startAmount; i < leftAmount; i++)
            {
                SetType(i, randomizerData.Type);
            }
        }
        
        _frequencyToAmountConverter.CheckForRest(leftAmount, j => SetType(j));
    }

    private void SetType(int i, ReplacementType type = ReplacementType.LeftBatiment)
    {
        _placeholderDataSet[i].SetType(type);
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