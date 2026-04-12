using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class MapOrganizer : MonoBehaviour
{
    [HideInInspector] public bool[] adjacencyMatrixData;
    public CityData cityData;
    [SerializeField] private StructureTypeDatabase structureTypeDatabase;
    

    public int GetSelectedStructureTypeAmount() => cityData.RandomizerDataSet.Length;

    private Dictionary<StructureTypeData, int> GetSelectedStructureTypeDatasAndAmounts()
    {
        var dict = new Dictionary<StructureTypeData, int>();
        var selectedTypes = cityData.GetSelectedStructureTypes();

        foreach (var type in selectedTypes)
        {
            if (cityData.TryGetAmountByType(type, out var amount))
            {
                dict[structureTypeDatabase.GetData(type)] = amount;
            }
        }
        return dict;
    }
    
    public HashSet<SlotData> ToSlotData(HashSet<Vector2Int> map)
    {
        ConvertFrequenciesToAmounts(map.Count);
        var typeDatasAndAmounts = GetSelectedStructureTypeDatasAndAmounts();
        
        var adjacencyImpossibilities =
            AdjacencyHelper.GetImpossibleAdjacencyDB(
                typeDatasAndAmounts.
                    Select(s => s.Key.Type).
                    ToArray(), adjacencyMatrixData);

        //averageFrequency
        //int.MaxValue://TODO: her zaman max olamaz, 2 tane quad1x1 varsa mesela. En son max yap. sayılar bitince. random. max.

        return DisposeMap(map, typeDatasAndAmounts, adjacencyImpossibilities);
    }

    private HashSet<SlotData> DisposeMap(
        HashSet<Vector2Int> map,
        Dictionary<StructureTypeData, int> typeDatasAndAmounts,
        Dictionary<StructureType, StructureType[]> adjacencyImpossibilities
        )
    {
        List<Vector2Int> mapToAlter = new();
        mapToAlter.AddRange(map);
        Shuffle(mapToAlter);

        var randomQuads = QuadSearcher.SearchQuads(
            typeDatasAndAmounts,
            mapToAlter.ToHashSet(),
            adjacencyImpossibilities);

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