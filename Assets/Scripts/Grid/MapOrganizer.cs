using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapOrganizer
{
    public static HashSet<CellData> ToCellData(HashSet<Vector2Int> map, QuadSample quadSample, int quadAmount)
    {
        List<Vector2Int> singlePoints = new();
        singlePoints.AddRange(map);
        Shuffle(singlePoints);
        
        var randomQuads = QuadSearcher.SearchQuads(quadSample, singlePoints.ToHashSet(), quadAmount);
        var quadCellDatas = CellDataCreator.CreateCellDataFromQuads(randomQuads, map);
        
        EliminateQuadCoordsFromSinglePoints(randomQuads, singlePoints);
        var singleCellDatas = CellDataCreator.CreateCellDataFromSinglePoints(singlePoints.ToHashSet(), map.ToHashSet(), 1);
       
        quadCellDatas.UnionWith(singleCellDatas);
        return quadCellDatas;
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