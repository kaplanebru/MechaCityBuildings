using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Android;


public class QuadSearcher
{
    private static HashSet<QuadOnMap> SearchQuadsInGivenType(StructureTypeSearchData searchData, List<Vector2Int> runningMap, SlotTypePossibilityHandler possibilityHandler)
    {
        List<Vector2Int> tempMap = new();
        tempMap.AddRange(runningMap);
        
        HashSet<QuadOnMap> selectedQuads = new();
        int index = 0;
            
        while (index < tempMap.Count)
        {
            var examinedPoint = tempMap[index];

            var tempQuadPoints =
                QuadProjector.GetQuadOnGivenPoint(examinedPoint, searchData.QuadSample).ToHashSet();

            if (QuadIsOnMap(tempQuadPoints, tempMap))
            {
                //başka quadlar aradığı için quad sayısından fazla oluyor
                if (!possibilityHandler.IsTypeConvenient(searchData.Type, tempQuadPoints.ToArray()))
                {
                    index++;
                    tempMap.RemoveAll(tempQuadPoints.Contains);
                }
                else
                {
                    var newQuad = CreateQuadOnMap(
                        examinedPoint,
                        searchData.QuadSample,
                        tempQuadPoints.ToArray(),
                        searchData.Type);
                    
                    possibilityHandler.UpdateNeighbourPossibilities(newQuad, searchData.Type);
                    selectedQuads.Add(newQuad);
                    tempMap.RemoveAll(tempQuadPoints.Contains);
                
                    index++;
                    searchData.Amount--;
                    if (searchData.Amount <= 0) break;
                }
            }
            else
            {
                index++;
            }
        }
        return selectedQuads;
    }
    public static HashSet<QuadOnMap> SearchQuads(
            List<StructureTypeSearchData> searchDatas,
            HashSet<Vector2Int> map)
    {
        HashSet<QuadOnMap> quads = new();

        List<Vector2Int> runningMap = new();
        runningMap.AddRange(map);

        searchDatas = searchDatas.OrderByDescending(sd => sd.QuadSample.data.GetPointAmount).ToList();
        foreach (var searchData in searchDatas)
        {
            Debug.Log("search data: " + searchData.Type + " " + searchData.Amount);
        }

        SlotTypePossibilityHandler possibilityHandler = new(map, searchDatas.ToHashSet());

        foreach (var searchData in searchDatas)
        {
            foreach (var cell in quads.SelectMany(quad => quad.data.Coords))
            {
                runningMap.Remove(cell);
            }
            
            quads.UnionWith(SearchQuadsInGivenType(searchData, runningMap, possibilityHandler));
        }

        if(HasEmptyPoints(runningMap, searchDatas, out var remainingQuads))
         quads.UnionWith(remainingQuads);
       
        return quads;
    }

    private static QuadOnMap CreateQuadOnMap(Vector2Int startPoint, QuadSample quadSample, Vector2Int[] points,
        StructureType slotType)
    {
        var quad = new QuadOnMap(quadSample.data.WidthHeight);
        quad.Setup(
            points.ToArray(),
            QuadProjector.GetNeighborsOnGivenPoint(startPoint, quadSample).ToArray()
        );

        return quad;
    }

    private static bool QuadIsOnMap(HashSet<Vector2Int> quadPoints, List<Vector2Int> map)
    {
        foreach (var quadPoint in quadPoints)
        {
            if (!map.Contains(quadPoint))
                return false;
        }

        return true;
    }
    
    private static bool HasEmptyPoints(List<Vector2Int> runningMap, List<StructureTypeSearchData> searchDatas,
        out HashSet<QuadOnMap> quads)
    {
        quads = new();
        if (runningMap.Count == 0) return false;

        //searchDatas = searchDatas.Reverse();
        List<StructureType> singularQuadTypes = new();
        QuadSample singularQuadSample = null;

        foreach (var searchData in searchDatas)
        {
            if (searchData.QuadSample.data.GetPointAmount == 1)
            {
                singularQuadTypes.Add(searchData.Type);
                singularQuadSample = searchData.QuadSample;
            }
        }

        if (singularQuadSample == null) return false;

        foreach (var cell in runningMap)
        {
            var newQuad = CreateQuadOnMap(
                cell,
                singularQuadSample,
                new[] { cell },
                singularQuadTypes[Random.Range(0, singularQuadTypes.Count)]);

            quads.Add(newQuad);
        }

        return true;
    }

}