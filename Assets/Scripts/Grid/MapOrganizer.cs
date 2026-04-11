using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapOrganizer
{
    public static HashSet<SlotData> ToSlotData(
        HashSet<Vector2Int> map,
        Dictionary<StructureTypeData, int> structureTypeDatasAndAmounts,
        Dictionary<StructureType, StructureType[]> adjacencyPossibilities)
    {
        List<Vector2Int> mapToAlter = new();
        mapToAlter.AddRange(map);
        Shuffle(mapToAlter);

        var randomQuads = QuadSearcher.SearchQuads(
            structureTypeDatasAndAmounts,
            mapToAlter.ToHashSet(),
            adjacencyPossibilities);
        var slotDatas = SlotCreator.CreateCellDataFromQuads(randomQuads, map);

        //EliminateQuadCoordsFromSinglePoints(randomQuads, mapToAlter);
        //var singleCellDatas = SlotDataCreator.CreateCellDataFromSinglePoints(mapToAlter.ToHashSet(), map.ToHashSet(), 1);
        //quadCellDatas.UnionWith(singleCellDatas);
        return slotDatas;
    }


    private static void EliminateQuadCoordsFromSinglePoints(QuadOnMap[] randomQuads, List<Vector2Int> singlePoints)
    {
        foreach (var quad in randomQuads)
        {
            foreach (var quadPoint in quad.data.Coords)
            {
                singlePoints.Remove(quadPoint);
            }
        }
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