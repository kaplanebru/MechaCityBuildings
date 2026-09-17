using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class StructureTypeSearchData
{
    public StructureType Type;
    public QuadSample QuadSample;
    public int Amount;
    private HashSet<StructureType> _impossibleStructureTypes = new();
    public IReadOnlyCollection<StructureType> ImpossibleStructureTypes => _impossibleStructureTypes;

    public StructureTypeSearchData(StructureType type, QuadSample quadSample, int amount,
        HashSet<StructureType> impossibleTypes)
    {
        Type = type;
        QuadSample = quadSample;
        Amount = amount;
        _impossibleStructureTypes.UnionWith(impossibleTypes); // ← private field'a yaz
        //Debug.Log($"{type} impossible types: {string.Join(",", ImpossibleStructureTypes)}");
    }
}

[ExecuteInEditMode]
public class MapOrganizer : MonoBehaviour
{
    private CityData _currentCityData;
    [SerializeField] private StructureDatabase structureDatabase;
    
    
    private List<StructureTypeSearchData> GetStructureTypeDatas()
    {
        var structureTypeData = new List<StructureTypeSearchData>();
        var selectedTypes = _currentCityData.GetStructureTypes();

        foreach (var type in selectedTypes)
        {
            if (_currentCityData.TryGetAmountByType(type, out var amount))
            {
                structureTypeData.Add(new StructureTypeSearchData(
                    type,
                    structureDatabase.GetData(type).QuadSample,
                    amount,
                    AdjacencyHelper.GetImpossibleAdjacencyForGivenType(type, selectedTypes.ToArray(), _currentCityData.matrix)
                        .ToHashSet()));
            }
        }

        return structureTypeData;
    }

    public HashSet<SlotData> ToSlotData(HashSet<Vector2Int> map, CityData cityData)
    {
        _currentCityData = cityData;
        
        ConvertFrequenciesToAmounts(map.Count);
        var structureTypeDatas = GetStructureTypeDatas();
        
        return DisposeMapByShuffle(map, structureTypeDatas);
    }

    private HashSet<SlotData> DisposeMapByShuffle(
        HashSet<Vector2Int> map,
        List<StructureTypeSearchData> structureTypeDatas)
    {
        List<Vector2Int> mapToAlter = new();
        mapToAlter.AddRange(map);
        Shuffle(mapToAlter);

        var randomQuads = QuadSearcher.SearchQuads(
            structureTypeDatas,
            mapToAlter.ToHashSet());

        return SlotCreator.CreateSlotDataFromQuads(randomQuads.ToArray(), map);
    }

    private void ConvertFrequenciesToAmounts(int cellAmount)
    {
        FrequencyData[] frequencyDatas = _currentCityData.RandomizerDataSet.
            Select(r => r.FrequencyData).
            ToArray();
        
        FrequencyToAmountConverter.SetAmountsByRatio(frequencyDatas, cellAmount);
    }

    public void MatchRandomizerWithPool(ref CityData cityData, HashSet<StructureType> activeTypes)
    {
        cityData.RandomizerDataSet = cityData.RandomizerDataSet.Where(r => activeTypes.Contains(r.Type)).ToList();
    }

    public static void Shuffle<T>(IList<T> collection) //T[] //IList
    {
        int n = collection.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (collection[n], collection[k]) = (collection[k], collection[n]);
        }
    }
}