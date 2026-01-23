using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CityRandomizer : MonoBehaviour
{
    [SerializeField] private RandomizerData[] randomizerDataSet;
    [SerializeField] private CityData cityData;
    [SerializeField] private Transform placeholderRoot;

    public ArrangementCache arrangementCache = new();
    private DiceRoller _diceRoller = new();
    private CityOrderRegulator _orderRegulator;

    private List<PlaceholderData> _placeholderDataSet = new();
    private Dictionary<ReplacementType, int> _amountsByType = new Dictionary<ReplacementType, int>();

    private void GetAllPlaceholders()
    {
        _orderRegulator = new CityOrderRegulator(cityData.HeightGap);
        _placeholderDataSet = _orderRegulator.GetRegulatedPlaceholdersData().ToList();
    }

    public void MixAndApply()
    {
        GetAllPlaceholders();
        InitiateAmountsByType();
        ApplyTypesToPlaceholders();
    }

    private void InitiateAmountsByType()
    {
        _amountsByType.Clear();
        foreach (var data in randomizerDataSet)
        {
            _amountsByType.Add(data.Type, data.FrequencyData.Amount);
        }
    }
    private void HandleSingleTypeCase(PlaceholderData placeholder)
    {
        var lastType = _amountsByType.Keys.First();
        _amountsByType[lastType]--;
        placeholder.ApplyType(lastType);

        if (_amountsByType[lastType] <= 0)
            _amountsByType.Remove(lastType);
    }

    private void ApplyTypesToPlaceholders()
    {
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

                    placeholder.ApplyType(type);
                    break;
                }

                _amountsByType.Remove(type);
            }
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