using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    Dictionary<int, int> _quotaLimitsByHeight = new();
    Dictionary<int, int> _currentQuotas = new();

    private void GetAllPlaceholders()
    {
        _orderRegulator = new CityOrderRegulator(cityData.HeightGap);
        _placeholderDataSet = _orderRegulator.GetRegulatedPlaceholdersData().ToList();
    }

    private void InitiateQuotas()
    {
        _quotaLimitsByHeight = cityData.GetQuotaByHeight();
        foreach (var heightTier in _quotaLimitsByHeight.Keys)
        {
            _currentQuotas[heightTier] = 0;
        }
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
        InitiateQuotas();
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


    private bool HasQuota(int heightTier)
    {
        if (_currentQuotas[heightTier] >= _quotaLimitsByHeight[heightTier])
        {
            _currentQuotas[heightTier] = 0;
            return false;
        }

        _currentQuotas[heightTier]++;
        return true;
    }

    private void ApplyTypesToPlaceholders()
    {
        //eliminate zeros at start:
        _amountsByType = _amountsByType.Where(kvp => kvp.Value != 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        foreach (var placeholder in _placeholderDataSet)
        {
            if (_amountsByType.Count == 0)
                return;

            var replacementData = GetReplacementData();
            var selectedType = replacementData.Type;
            _amountsByType[selectedType]--;
            
            if (_amountsByType[selectedType] == 0)
                _amountsByType.Remove(selectedType);
            
            placeholder.ApplyReplacementData(replacementData);
        }
    }


    Dictionary<ReplacementType, int> tempAmountsByType = new ();

    private ReplacementData GetReplacementData()
    {
        tempAmountsByType.Clear();
        tempAmountsByType.AddRange(_amountsByType);

        while (tempAmountsByType.Count > 0)
        {
            var examinedType = _diceRoller.RollDices(_amountsByType);
            var replacementData = replacementDatabase.Get(examinedType);

            if (HasQuota(replacementData.HeightTier))
            {
                return replacementData;
            }

            tempAmountsByType.Remove(examinedType);
        }

        var inevitableType = _diceRoller.RollDices(_amountsByType);
        var inevitableData = replacementDatabase.Get(inevitableType);
        _currentQuotas[inevitableData.HeightTier] = _quotaLimitsByHeight[inevitableData.HeightTier];
        return inevitableData;
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