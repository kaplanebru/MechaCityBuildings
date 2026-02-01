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
    private Dictionary<ReplacementType, int> _pendingReplacements = new Dictionary<ReplacementType, int>();

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

    private void InitiateUnassignedBuildings()
    {
        _pendingReplacements.Clear();
        foreach (var data in randomizerDataSet)
        {
            _pendingReplacements.Add(data.Type, data.FrequencyData.Amount);
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
        InitiateUnassignedBuildings();
        ApplyTypesToPlaceholders();
    }


    private void ApplyTypesToPlaceholders()
    {
        //eliminate zeros at start:
        _pendingReplacements = _pendingReplacements.Where(kvp => kvp.Value != 0)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var placeholder in _placeholderDataSet)
        {
            if (_pendingReplacements.Count == 0)
                return;

            var replacementData = GetReplacementDataByCheckingQuotas();

            _lastHeightTier = replacementData.HeightTier;
            var selectedType = replacementData.Type;
            placeholder.ApplyReplacementType(selectedType);

            _pendingReplacements[selectedType]--;
            if (_pendingReplacements[selectedType] == 0)
                _pendingReplacements.Remove(selectedType);
        }
        Debug.LogWarning($"Safety replacement count/" + counter);
        counter = 0;

    }

    private int _lastHeightTier;

    private bool HasQuota(int heightTier)
    {
       // print("current quota: " + _currentQuotasByHeight[heightTier] + " limit: " + _quotaLimitsByHeight[heightTier]);
        return _currentQuotasByHeight[heightTier] < _quotaLimitsByHeight[heightTier];
    }

    private int counter = 0;
    private ReplacementData GetReplacementDataByCheckingQuotas()
    {
        var candidateType = _diceRoller.RollDices(_pendingReplacements);
        //return replacementDatabase.GetData(examinedType); //to debug
        var candidateTier = replacementDatabase.GetHeightTierByType(candidateType);
        
        if (candidateTier != _lastHeightTier)
        {
            ResetQuotas(); //bug: sonuncuyu sıfırlamadan artırmak mı lazım
            _currentQuotasByHeight[candidateTier]++;
            return replacementDatabase.GetData(candidateType);
        }

        Dictionary<ReplacementType, int> remainingReplacements = new();
        remainingReplacements.AddRange(_pendingReplacements);

        while (true)
        {
            if (HasQuota(candidateTier))
            {
                ResetQuotasExcept(candidateTier);
                _currentQuotasByHeight[candidateTier]++;
                return replacementDatabase.GetData(candidateType);
            }

            remainingReplacements.Remove(candidateType);
            if (remainingReplacements.Count == 0)
                break;

            candidateType = _diceRoller.RollDices(remainingReplacements);
            candidateTier = replacementDatabase.GetHeightTierByType(candidateType);
        }
        
        counter++;
        return replacementDatabase.GetData(candidateType);

        /*int attempts = _unassignedReplacements.Count; //3 type, 2 tier olsun
        while (attempts > 0)
        {
            var examinedType = _diceRoller.RollDices(_unassignedReplacements); //aynı dice'ı verebilir temp grup lazım
            var examinedTier = replacementDatabase.GetHeightTierByType(examinedType);

            if (examinedTier == _lastHeightTier)
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
                ResetQuotas();
                _currentQuotasByHeight[examinedTier]++;
                return replacementDatabase.GetData(examinedType);
            }
            attempts--;
        }*/
    }

    private void ResetQuotasExcept(int selectedHeightTier)
    {
        foreach (var heightKey in heightKeys)
        {
            if (heightKey == selectedHeightTier) continue;
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