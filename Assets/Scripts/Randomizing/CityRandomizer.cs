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
    Dictionary<int, int> _currentQuotasByHeight = new();
    private List<int> heightKeys = new();

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
            _currentQuotasByHeight[heightTier] = 0;
            heightKeys.Add(heightTier);
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
    private void ApplyTypesToPlaceholders()
    {
        //eliminate zeros at start:
        _amountsByType = _amountsByType.Where(kvp => kvp.Value != 0).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var placeholder in _placeholderDataSet)
        {
            if (_amountsByType.Count == 0)
                return;

            var replacementData = CheckQuotasAndGetReplacementData();

            _lastHeightTier = replacementData.HeightTier;
            var selectedType = replacementData.Type;
            _amountsByType[selectedType]--;

            if (_amountsByType[selectedType] == 0)
                _amountsByType.Remove(selectedType);

            placeholder.ApplyReplacementType(selectedType);
        }
    }

    private int _lastHeightTier;

    private bool HasQuota(int heightTier)
    {
        return _currentQuotasByHeight[heightTier] < _quotaLimitsByHeight[heightTier];
    }
    bool _firstAttempt = true;
    private ReplacementData CheckQuotasAndGetReplacementData()
    {
        int attempts = _amountsByType.Count;
        while (attempts > 0)
        {
            var examinedType = _diceRoller.RollDices(_amountsByType);
            var examinedTier = replacementDatabase.GetHeightTierByType(examinedType);

            if (examinedTier == _lastHeightTier && !_firstAttempt)
            {
                if (HasQuota(examinedTier))
                {
                    ResetQuotasExcept(examinedTier);
                    _currentQuotasByHeight[examinedTier]++;
                    return replacementDatabase.GetData(examinedType);
                }
            }
            else
            {
                if(_firstAttempt) _firstAttempt = false;
                
                ResetQuotas();
                _currentQuotasByHeight[examinedTier]++;
                return replacementDatabase.GetData(examinedType);
            }
            attempts--;
        }

        Debug.LogError($"Safety replacement with No more quotas left" + " amountsByType: " + _amountsByType.Count);
        return replacementDatabase.GetData(_amountsByType.First().Key);
    }

    private void ResetQuotasExcept(int selectedHeightTier)
    {
        foreach (var heightKey in heightKeys)
        {
            if(heightKey == selectedHeightTier) continue;
            _currentQuotasByHeight[heightKey] = 0;
        }
    }

    private void ResetQuotas()
    {
        _currentQuotasByHeight.Keys.ToList().ForEach(key => _currentQuotasByHeight[key] = 0);
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