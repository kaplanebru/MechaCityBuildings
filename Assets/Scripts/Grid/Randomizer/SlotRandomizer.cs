using System.Linq;
using UnityEngine;

public class SlotRandomizer : MonoBehaviour
{
    [SerializeField] private CityData cityData;
    [SerializeField] private StructureTypeDatabase structureTypeDatabase;

    public void MixAndApplyPlacements(FloorResidentsData floorResidentsData) 
    {
        ConvertFrequenciesToAmounts(floorResidentsData);
    }
    
    private void ConvertFrequenciesToAmounts(FloorResidentsData floorResidentsData)
    {
        FrequencyData[] frequencyDatas = cityData.RandomizerDataSet.Select(r => r.FrequencyData).ToArray();
        FrequencyToAmountConverter.SetAmountsByRatio
            (frequencyDatas, floorResidentsData.Slots.Count);
    }
}