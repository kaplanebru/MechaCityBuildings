using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class Randomizer : MonoBehaviour
{
    public PlacementDatabase placementDatabase;
    [SerializeField] private CityData cityData;
    [SerializeField] private StructureTypeDatabase structureTypeDatabase;
    
    private HeightTierHelper _heightTierHelper;
    private PlacementOrderRegulator _orderRegulator;
    private Dictionary<StructureType, PendingPlacements> _pendingPlacements = new();
    
    public void SetPlacementsOnFloor(HashSet<CellWorldData> cellWorldDatas, int floorIndex)
    {
        if (placementDatabase.placementFloors[floorIndex] == null)
        {
            Debug.LogError("No placement floor was found with index " + floorIndex);
            return;
        }
        
        _orderRegulator = new PlacementOrderRegulator(cityData.HeightGap);
        placementDatabase.placementFloors[floorIndex].PlacementDataset = 
            _orderRegulator.GetRegulatedPlacements(cellWorldDatas).ToList();
    }
    

    private void InitiateQuotas()
    {
        _heightTierHelper = new HeightTierHelper();
        _heightTierHelper.SetHeightTierDatas(cityData.GetQuotaByHeight());
    }

    private void InitiatePendingStructures()
    {
        _pendingPlacements.Clear();
        foreach (var data in cityData.RandomizerDataSet)
        {
            var pendingReplacement = new PendingPlacements(
                data.Type,
                data.FrequencyData.Amount);
            _pendingPlacements.Add(data.Type, pendingReplacement);
            pendingReplacement.SetRemainingQuota
                (_heightTierHelper.GetRemainingQuota(structureTypeDatabase.GetHeightTierByType(data.Type)));

        }
    }

    private void ConvertFrequenciesToAmounts(int floorIndex)
    {
        FrequencyData[] frequencyDatas = cityData.RandomizerDataSet.Select(r => r.FrequencyData).ToArray();
        FrequencyToAmountConverter.SetAmountsByRatio(frequencyDatas, placementDatabase.placementFloors[floorIndex].PlacementDataset.Count);
    }

    public void MixAndApplyPlacements(int floorIndex) 
    {
        ConvertFrequenciesToAmounts(floorIndex);
        InitiateQuotas();
        InitiatePendingStructures();
        ApplyTypesToPlacements(floorIndex);
    }


    private void ApplyTypesToPlacements(int floorIndex)
    {
        //eliminate zeros at start:
        _pendingPlacements = _pendingPlacements.Where(kvp => kvp.Value.Amount != 0)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var placementDataset = placementDatabase.placementFloors[floorIndex].PlacementDataset;
        foreach (var placementData in placementDataset)
        {
            if (_pendingPlacements.Count == 0)
                return;

            var replacementData = GetReplacementDataByCheckingQuotas();

            _lastHeightTier = replacementData.HeightTier;
            var selectedType = replacementData.Type;
            placementData.ApplyStructureType(selectedType);

            _pendingPlacements[selectedType].Amount--;
            if (_pendingPlacements[selectedType].Amount == 0)
                _pendingPlacements.Remove(selectedType);
        }

        if(counter>0)
            Debug.LogWarning($"Safety replacement count/" + counter);
        counter = 0;
    }


    private int _lastHeightTier;
    private int counter = 0;

    private void UpdateQuota(int tier,StructureType type)
    {
        _heightTierHelper.UpdateUsedQuota(tier);
        _pendingPlacements[type].SetRemainingQuota(_heightTierHelper.GetRemainingQuota(tier));
    }
    private StructureTypeData GetReplacementDataByCheckingQuotas()
    {
        var candidateType = DiceRoller.RollDices(_pendingPlacements.Values.ToArray());
        var candidateTier = structureTypeDatabase.GetHeightTierByType(candidateType);

        if (candidateTier != _lastHeightTier)
        {
            _heightTierHelper.ResetQuotas();
            UpdateQuota(candidateTier, candidateType);
            return structureTypeDatabase.GetData(candidateType);
        }

        Dictionary<StructureType, PendingPlacements> remainingReplacements = new();
        remainingReplacements.AddRange(_pendingPlacements);

        while (true)
        {
            if (_heightTierHelper.HasQuota(candidateTier))
            {
                _heightTierHelper.ResetQuotasExcept(candidateTier);
                UpdateQuota(candidateTier, candidateType);
                return structureTypeDatabase.GetData(candidateType);
            }

            remainingReplacements.Remove(candidateType);
            if (remainingReplacements.Count == 0)
                break;

            candidateType = DiceRoller.RollDices(remainingReplacements.Values.ToArray());
            candidateTier = structureTypeDatabase.GetHeightTierByType(candidateType);
        }

        counter++;
        return structureTypeDatabase.GetData(candidateType);
    }

   
}