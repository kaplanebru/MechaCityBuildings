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
        _frequencyToAmountConverter.SetAmountsByRatio();
    }

    public void MixAndApply()
    {
        GetAllPlaceholders();
        ApplyTypesToPlaceholders();
    }

    private Dictionary<ReplacementType, int> _amountsByType = new Dictionary<ReplacementType, int>();

    DiceRoller _diceRoller = new();

    private void HandleSingleTypeCase(PlaceholderData placeholder)
    {
        var lastType = _amountsByType.Keys.First();
        _amountsByType[lastType]--;
        placeholder.SetType(lastType);

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
                    if (remaining <= 0) _amountsByType.Remove(type);
                    else _amountsByType[type] = remaining;

                    placeholder.SetType(type);
                    break;
                }

                _amountsByType.Remove(type);
            }
        }
    }

    /*private void ApplyTypesToPlaceholders()
    {
        foreach (var placeholder in _placeholderDataSet)
        {
            Roll:
            if(_amountsByType.Count == 0) return;

            if (_amountsByType.Count == 1)
            {
                var lastType = _amountsByType.ElementAt(0).Key;
                _amountsByType[lastType]--;
                placeholder.SetType(lastType);
                goto Roll;
            }

            var type = _diceRoller.RollDices(_amountsByType);
            if (_amountsByType[type] > 0)
            {
                _amountsByType[type]--;
                placeholder.SetType(type);
            }
            else
            {
                _amountsByType.Remove(type);
                goto Roll;
            }
        }
    }*/

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