using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class MapOrganizer : MonoBehaviour
{
    [HideInInspector] public bool[] adjacency;
    public StructureTypeData[] structureTypeDatas; //TEMP: test

    public HashSet<SlotData> ToSlotData(HashSet<Vector2Int> map)
    {
        List<Vector2Int> mapToAlter = new();
        var adjacencyImpossibilities =
            AdjacencyHelper.GetImpossibleAdjacencyDB(structureTypeDatas.Select(s => s.Type).ToArray(), adjacency);

        //TEMPORARY: TEST
        Dictionary<StructureTypeData, int> structureTypeDatasAndAmounts = new();
        structureTypeDatasAndAmounts.Add(structureTypeDatas[0], int.MaxValue);
        structureTypeDatasAndAmounts.Add(structureTypeDatas[1], 2); //averageFrequency


        mapToAlter.AddRange(map);
        Shuffle(mapToAlter);

        var randomQuads = QuadSearcher.SearchQuads(
            structureTypeDatasAndAmounts,
            mapToAlter.ToHashSet(),
            adjacencyImpossibilities);

        return SlotCreator.CreateCellDataFromQuads(randomQuads, map);
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