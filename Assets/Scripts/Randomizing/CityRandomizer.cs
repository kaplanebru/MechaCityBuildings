using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityRandomizer : MonoBehaviour
{
    [SerializeField] private RandomizerData[] randomizerDataSet;
    [SerializeField] private CityData cityData;
    [SerializeField] private Transform placeholderRoot;
    [SerializeField] private ReplacementDataBase replacementDatabase;

    public ArrangementCache arrangementCache = new();
    private DiceRoller _diceRoller = new();
    private FrequencyToAmountConverter _frequencyToAmountConverter = new();
    private CityOrderRegulator _orderRegulator;

    private List<PlaceholderData> _placeholderDataSet = new();
    private Dictionary<ReplacementType, int> _amountsByType = new Dictionary<ReplacementType, int>();

    private void GetAllPlaceholders()
    {
        _orderRegulator = new CityOrderRegulator(cityData.HeightGap);
        _placeholderDataSet = _orderRegulator.GetRegulatedPlaceholdersData().ToList();
    }

    private void ConvertFrequenciesToAmounts()
    {
        FrequencyData[] frequencyDatas = randomizerDataSet.Select(r => r.FrequencyData).ToArray();
        _frequencyToAmountConverter.Setup(frequencyDatas, _placeholderDataSet.Count);
        _frequencyToAmountConverter.SetAmountsByRatio();
    }

    public void MixAndApply()
    {
        GetAllPlaceholders();
        ConvertFrequenciesToAmounts();
        InitiateAmountsByType();
        ApplyTypesToPlaceholders();
    }

    private void InitiateAmountsByType()
    {
        _amountsByType.Clear();
        foreach (var data in randomizerDataSet)
        {
            _amountsByType.Add(data.Type, data.FrequencyData.Amount);
            //Debug.Log($"{data.Type}: {data.FrequencyData.Amount}");
        }
    }
    private void HandleSingleTypeCase(PlaceholderData placeholder)
    {
        var lastType = _amountsByType.Keys.First();
        _amountsByType[lastType]--;
        MarkPlaceholder(placeholder, lastType);


        if (_amountsByType[lastType] <= 0)
            _amountsByType.Remove(lastType);
    }

    private void ApplyTypesToPlaceholders()
    {
        //Debug.Log("place holder count: " + _placeholderDataSet.Count);
        //Debug.Log("amounts by types: " + _amountsByType.Values.Sum());
        foreach (var placeholder in _placeholderDataSet)
        {
            while (true)
            {
                if (_amountsByType.Count == 0)
                    return;

                if (_amountsByType.Count == 1)
                {
                    HandleSingleTypeCase(placeholder);
                    break;
                }

                var type = _diceRoller.RollDices(_amountsByType);

                if (_amountsByType.TryGetValue(type, out var remaining) && remaining > 0)
                {
                    remaining--;
                    if (remaining <= 0) 
                        _amountsByType.Remove(type);
                    else 
                        _amountsByType[type] = remaining;

                    MarkPlaceholder(placeholder, type);
                    
                    break;
                }

                _amountsByType.Remove(type);
            }
        }
    }

    private void MarkPlaceholder(PlaceholderData placeholder, ReplacementType type)
    {
        replacementDatabase.TryGet(type, out var replacementData);
        placeholder.ApplyReplacementData(replacementData);
        //Debug.Log("placeholder type: " + placeholder.GetReplacementType());
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