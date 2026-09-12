using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Android;


public class QuadSearcher
{
    //todo: listeden çıkarmak yerine quad cell'leri sealed yapabılabilir. if selaed continue.
    //todo: MAP ORGANİZER 79. SATIR COMMENTLİ: Acaba randomize edilmese nasıl çalışır?
    private static HashSet<QuadOnMap> DiscoveredQuadsInGivenType(
        StructureTypeSearchData structureTypeData,
        Dictionary<Vector2Int, bool> examiningMap,
        SlotNeighborConvenienceHandler possibilityHandler)
    {
        HashSet<QuadOnMap> selectedQuads = new();
        var map = examiningMap.Keys.ToList();

        foreach (var point in map)
        {
            if (examiningMap[point]) continue;
            var tempQuadPoints =
                QuadProjector.GetQuadOnGivenPoint(point, structureTypeData.QuadSample).ToHashSet();

            if (QuadIsOnMap(tempQuadPoints, map))
            {
                var neighbors = QuadProjector.GetNeighborsOnGivenPoint(point, structureTypeData.QuadSample).ToArray();
                if (!possibilityHandler.IsTypeConvenient2(structureTypeData.Type, neighbors))
                    continue;
                
                var newQuad = CreateQuadOnMap(
                    structureTypeData.QuadSample,
                    tempQuadPoints.ToArray(),
                    neighbors,
                    structureTypeData.Type);
                

                possibilityHandler.UpdateFilledCells(newQuad, structureTypeData.Type);
                selectedQuads.Add(newQuad);

                foreach (var quadPoint in tempQuadPoints)
                {
                    examiningMap[quadPoint] = true;
                }

                structureTypeData.Amount--;
                if (structureTypeData.Amount <= 0) break;
            }
        }
        return selectedQuads;
    }

    public static HashSet<QuadOnMap> SearchQuads(List<StructureTypeSearchData> structureTypeDatas,
        HashSet<Vector2Int> map)
    {
        HashSet<QuadOnMap> discoveredQuads = new();
        Dictionary<Vector2Int, bool> examiningMap = map.ToDictionary(point => point, point => false);


        structureTypeDatas = structureTypeDatas.OrderByDescending(sd => sd.QuadSample.data.GetPointAmount).ToList();
        SlotNeighborConvenienceHandler possibilityHandler = new(structureTypeDatas.ToHashSet());

        foreach (var structureTypeData in structureTypeDatas)
        {
            var found = DiscoveredQuadsInGivenType(structureTypeData, examiningMap, possibilityHandler);
            int before = discoveredQuads.Count;
            discoveredQuads.UnionWith(found);
            Debug.Log($"{structureTypeData.Type}: found={found.Count} before={before} after={discoveredQuads.Count}");

            examiningMap = examiningMap
                .Where(kvp => !kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        if (HasEmptyPoints(examiningMap.Keys.ToList(), structureTypeDatas, out var remainingQuads))
            discoveredQuads.UnionWith(remainingQuads);
        
        return discoveredQuads;
    }

    private static QuadOnMap CreateQuadOnMap(QuadSample quadSample, Vector2Int[] points, Vector2Int[] neighbors,
        StructureType structureType)
    {
        var quad = new QuadOnMap(quadSample.data.WidthHeight);
        quad.Setup(
            points.ToArray(),
            neighbors,
            structureType
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
            var neighbors = QuadProjector.GetNeighborsOnGivenPoint(cell, singularQuadSample).ToArray();
            
            var newQuad = CreateQuadOnMap(
                singularQuadSample,
                new[] { cell },
                neighbors,
                singularQuadTypes[Random.Range(0, singularQuadTypes.Count)]);

            quads.Add(newQuad);
        }

        return true;
    }
}