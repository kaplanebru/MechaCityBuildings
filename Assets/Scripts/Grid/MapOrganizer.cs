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

    public StructureTypeSearchData(StructureType type, QuadSample quadSample, int amount, HashSet<StructureType> impossibleTypes)
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
    [HideInInspector] public bool[] adjacencyMatrixData;
    public CityData cityData;
    [SerializeField] private StructureDatabase structureDatabase;
    

    public int GetSelectedStructureTypeAmount() => cityData.RandomizerDataSet.Length;

    private List<StructureTypeSearchData> GetStructureTypeDatas()
    {
        var structureTypeData = new List<StructureTypeSearchData>();
        var selectedTypes = cityData.GetSelectedStructureTypes();

        foreach (var type in selectedTypes)
        {
            if (cityData.TryGetAmountByType(type, out var amount))
            {
                structureTypeData.Add(new StructureTypeSearchData(
                    type, 
                    structureDatabase.GetData(type).QuadSample, 
                    amount,
                    AdjacencyHelper.GetImpossibleAdjacencyForGivenType(type, selectedTypes, adjacencyMatrixData).ToHashSet()));
            }
        }
        return structureTypeData;
    }
    
    public HashSet<SlotData> ToSlotData(HashSet<Vector2Int> map)
    {
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
        FrequencyData[] frequencyDatas = cityData.RandomizerDataSet.Select(r => r.FrequencyData).ToArray();
        FrequencyToAmountConverter.SetAmountsByRatio
            (frequencyDatas, cellAmount);
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