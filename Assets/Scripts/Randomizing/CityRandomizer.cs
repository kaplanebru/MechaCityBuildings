using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PendingReplacement
{
    public ReplacementType Type;
    public int Amount;
    public int RemainingQuota;

    public PendingReplacement(ReplacementType type, int amount)
    {
        Type = type;
        Amount = amount;
    }

    public void SetRemainingQuota(int remainingQuota)
    {
        RemainingQuota = remainingQuota;
    }
}

public class CityRandomizer : MonoBehaviour
{
    [SerializeField] private RandomizerData[] randomizerDataSet;
    [SerializeField] private CityData cityData;
    [SerializeField] private Transform placeholderRoot;
    [SerializeField] private ReplacementDataBase replacementDatabase;

    public ArrangementCache arrangementCache = new();
    private DiceRoller _diceRoller = new();
    private FrequencyToAmountConverter _frequencyToAmountConverter = new();

    private HeightTierHelper _heightTierHelper;
    private CityOrderRegulator _orderRegulator;

    private List<PlaceholderData> _placeholderDataSet = new();
    private Dictionary<ReplacementType, PendingReplacement> _pendingReplacements = new();

    private void GetAllPlaceholders()
    {
        _orderRegulator = new CityOrderRegulator(cityData.HeightGap);
        _placeholderDataSet = _orderRegulator.GetRegulatedPlaceholdersData().ToList();
    }

    private void InitiateQuotas()
    {
        _heightTierHelper = new HeightTierHelper();
        _heightTierHelper.SetHeightTierDatas(cityData.GetQuotaByHeight());
    }

    private void InitiatePendingReplacements()
    {
        _pendingReplacements.Clear();
        foreach (var data in randomizerDataSet)
        {
            var x = new PendingReplacement(
                data.Type,
                data.FrequencyData.Amount);
            _pendingReplacements.Add(data.Type, x);
            x.SetRemainingQuota(_heightTierHelper.GetRemainingQuota(replacementDatabase.GetHeightTierByType(data.Type)));

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
        InitiatePendingReplacements();
        ApplyTypesToPlaceholders();
    }


    private void ApplyTypesToPlaceholders()
    {
        //eliminate zeros at start:
        _pendingReplacements = _pendingReplacements.Where(kvp => kvp.Value.Amount != 0)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        foreach (var placeholder in _placeholderDataSet)
        {
            if (_pendingReplacements.Count == 0)
                return;

            var replacementData = GetReplacementDataByCheckingQuotas();

            _lastHeightTier = replacementData.HeightTier;
            var selectedType = replacementData.Type;
            placeholder.ApplyReplacementType(selectedType);

            _pendingReplacements[selectedType].Amount--;
            if (_pendingReplacements[selectedType].Amount == 0)
                _pendingReplacements.Remove(selectedType);
        }

        if(counter>0)
            Debug.LogWarning($"Safety replacement count/" + counter);
        counter = 0;
    }


    private int _lastHeightTier;
    private int counter = 0;

    private ReplacementData GetReplacementDataByCheckingQuotas()
    {
        var candidateType = _diceRoller.RollDices(_pendingReplacements.Values.ToArray());
        var candidateTier = replacementDatabase.GetHeightTierByType(candidateType);

        if (candidateTier != _lastHeightTier)
        {
            _heightTierHelper.ResetQuotas();
            _heightTierHelper.UpdateUsedQuota(candidateTier);
            _pendingReplacements[candidateType].SetRemainingQuota(_heightTierHelper.GetRemainingQuota(candidateTier));
            return replacementDatabase.GetData(candidateType);
        }

        Dictionary<ReplacementType, PendingReplacement> remainingReplacements = new();
        remainingReplacements.AddRange(_pendingReplacements);

        while (true)
        {
            if (_heightTierHelper.HasQuota(candidateTier))
            {
                _heightTierHelper.ResetQuotasExcept(candidateTier);
                _heightTierHelper.UpdateUsedQuota(candidateTier);
                _pendingReplacements[candidateType].SetRemainingQuota(_heightTierHelper.GetRemainingQuota(candidateTier));
                return replacementDatabase.GetData(candidateType);
            }

            remainingReplacements.Remove(candidateType);
            if (remainingReplacements.Count == 0)
                break;

            candidateType = _diceRoller.RollDices(remainingReplacements.Values.ToArray());
            candidateTier = replacementDatabase.GetHeightTierByType(candidateType);
        }

        counter++;
        return replacementDatabase.GetData(candidateType);
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