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

    private IEnumerable<StructureTypeData> GetSelectedStructureTypeDatas()
    {
        var selectedTypes = cityData.GetSelectedStructureTypes();
        
        foreach (var type in selectedTypes)
        {
            yield return structureTypeDatabase.GetData(type);
        }
    }
    
    public HashSet<SlotData> ToSlotData(HashSet<Vector2Int> map)
    {
        ConvertFrequenciesToAmounts(map.Count);
        var selectedStructureTypeDatas = GetSelectedStructureTypeDatas().ToArray();
        
        var adjacencyImpossibilities =
            AdjacencyHelper.GetImpossibleAdjacencyDB(
                selectedStructureTypeDatas.Select(s => s.Type).ToArray(), adjacencyMatrixData);

        //TEMPORARY: TEST
        Dictionary<StructureTypeData, int> structureTypeDatasAndAmounts = new();
        
        //TODO: her zaman max olamaz, 2 tane quad1x1 varsa mesela. En son max yap. sayılar bitince. random. max.
        structureTypeDatasAndAmounts.Add(selectedStructureTypeDatas[0], int.MaxValue);
        structureTypeDatasAndAmounts.Add(selectedStructureTypeDatas[1], cityData.RandomizerDataSet[1].FrequencyData.Amount); //averageFrequency

        return DisposeMap(map, structureTypeDatasAndAmounts, adjacencyImpossibilities);
    }

    private HashSet<SlotData> DisposeMap(
        HashSet<Vector2Int> map,
        Dictionary<StructureTypeData, int> structureTypeDatasAndAmounts,
        Dictionary<StructureType, StructureType[]> adjacencyImpossibilities
        )
    {
        List<Vector2Int> mapToAlter = new();
        mapToAlter.AddRange(map);
        Shuffle(mapToAlter);

        var randomQuads = QuadSearcher.SearchQuads(
            structureTypeDatasAndAmounts,
            mapToAlter.ToHashSet(),
            adjacencyImpossibilities);

        return SlotCreator.CreateCellDataFromQuads(randomQuads, map);
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