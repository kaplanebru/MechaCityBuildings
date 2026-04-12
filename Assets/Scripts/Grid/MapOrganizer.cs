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
    public HashSet<StructureType> AdjacencyImpossibilities = new();

    public StructureTypeSearchData(StructureType type, QuadSample quadSample, int amount, HashSet<StructureType> impossibleTypes)
    {
        Type = type;
        QuadSample = quadSample;
        Amount = amount;
        AdjacencyImpossibilities.UnionWith(impossibleTypes);
    }

    public void RemoveAdjacency(StructureType adjacencyType)
    {
        AdjacencyImpossibilities.Remove(adjacencyType);
    }
}


[ExecuteInEditMode]
public class MapOrganizer : MonoBehaviour
{
    [HideInInspector] public bool[] adjacencyMatrixData;
    public CityData cityData;
    [SerializeField] private StructureTypeDatabase structureTypeDatabase;
    

    public int GetSelectedStructureTypeAmount() => cityData.RandomizerDataSet.Length;

    private List<StructureTypeSearchData> GetSearchData()
    {
        var searchData = new List<StructureTypeSearchData>();
        var selectedTypes = cityData.GetSelectedStructureTypes();

        foreach (var type in selectedTypes)
        {
            Debug.Log(type);
            if (cityData.TryGetAmountByType(type, out var amount))
            {
                searchData.Add(new StructureTypeSearchData(
                    type, 
                    structureTypeDatabase.GetData(type).QuadSample, 
                    amount,
                    AdjacencyHelper.GetImpossibleAdjacencyForGivenType(type, selectedTypes, adjacencyMatrixData).ToHashSet()));
            }
        }
        return searchData;
    }
    
    public HashSet<SlotData> ToSlotData(HashSet<Vector2Int> map)
    {
        ConvertFrequenciesToAmounts(map.Count);
        var searchDatas = GetSearchData();
        
        /*foreach (var searchData in searchDatas)
        {
            foreach (var type in searchData.AdjacencyImpossibilities)
            {
                Debug.Log(searchData.Type + " " + type);
            }
        }*/

        return DisposeMap(map, searchDatas);
    }

    private HashSet<SlotData> DisposeMap(
        HashSet<Vector2Int> map,
        List<StructureTypeSearchData> searchDatas)
    {
        List<Vector2Int> mapToAlter = new();
        mapToAlter.AddRange(map);
        Shuffle(mapToAlter);

        var randomQuads = QuadSearcher.SearchQuads(
            searchDatas,
            mapToAlter.ToHashSet());

        return SlotCreator.CreateCellDataFromQuads(randomQuads.ToArray(), map);
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