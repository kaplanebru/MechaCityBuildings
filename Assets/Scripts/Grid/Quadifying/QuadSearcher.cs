using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadSearcher
{
    //todo: listeden çıkarmak yerine quad cell'leri sealed yapabılabilir. if selaed continue.
    //todo: MAP ORGANİZER 79. SATIR COMMENTLİ: Acaba randomize edilmese nasıl çalışır?

    private static HashSet<QuadOnMap> DiscoveredQuadsInGivenGroup(
        List<StructureTypeSearchData> group,
        Dictionary<Vector2Int, bool> examiningMap,
        SlotNeighborConvenienceHandler possibilityHandler)
    {
        HashSet<QuadOnMap> selectedQuads = new();
        var map = examiningMap.Keys.ToList();

        foreach (var point in map)
        {
            if (examiningMap[point]) continue;

            // Bu noktaya sığan ve komşuluğu uygun tipler
            var candidates = group
                .Where(sd => QuadIsOnMap(
                    QuadProjector.GetQuadOnGivenPoint(point, sd.QuadSample), examiningMap))
                .Where(sd => possibilityHandler.IsTypeConvenient2(sd.Type,
                    QuadProjector.GetNeighborsOnGivenPoint(point, sd.QuadSample).ToArray()))
                .ToList();

            if (candidates.Count == 0) continue;

            var structureTypeData = PickWeighted(candidates);
            var tempQuadPoints =
                QuadProjector.GetQuadOnGivenPoint(point, structureTypeData.QuadSample).ToArray();
            var neighbors =
                QuadProjector.GetNeighborsOnGivenPoint(point, structureTypeData.QuadSample).ToArray();

            var newQuad = CreateQuadOnMap(
                structureTypeData.QuadSample,
                tempQuadPoints,
                neighbors,
                structureTypeData.Type);

            possibilityHandler.UpdateFilledCells(newQuad, structureTypeData.Type);
            selectedQuads.Add(newQuad);

            foreach (var quadPoint in tempQuadPoints)
            {
                examiningMap[quadPoint] = true;
            }

            structureTypeData.Amount--;
            if (structureTypeData.Amount <= 0)
            {
                group.Remove(structureTypeData);
                if (group.Count == 0) break;
            }
        }

        return selectedQuads;
    }

    public static HashSet<QuadOnMap> SearchQuads(List<StructureTypeSearchData> structureTypeDatas,
        HashSet<Vector2Int> map)
    {
        HashSet<QuadOnMap> discoveredQuads = new();
        Dictionary<Vector2Int, bool> examiningMap = map.ToDictionary(point => point, point => false);

        SlotNeighborConvenienceHandler possibilityHandler = new(structureTypeDatas.ToHashSet());

        Dictionary<int, List<StructureTypeSearchData>> volumeGroups = structureTypeDatas
            .GroupBy(sd => sd.QuadSample.data.GetPointAmount)
            .ToDictionary(g => g.Key, g => g.ToList());

        var singularTypes = volumeGroups.TryGetValue(1, out var singles) ? singles.ToList() : null;

        foreach (var volume in volumeGroups.Keys.OrderByDescending(v => v))
        {
            var group = volumeGroups[volume];
            var found = DiscoveredQuadsInGivenGroup(group, examiningMap, possibilityHandler);
            int before = discoveredQuads.Count;
            discoveredQuads.UnionWith(found);
            Debug.Log($"volume {volume}: found={found.Count} before={before} after={discoveredQuads.Count}");

            examiningMap = examiningMap
                .Where(kvp => !kvp.Value)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        if (HasEmptyPoints(examiningMap.Keys.ToList(), singularTypes, out var remainingQuads))
            discoveredQuads.UnionWith(remainingQuads);

        return discoveredQuads;
    }

    private static StructureTypeSearchData PickWeighted(List<StructureTypeSearchData> candidates)
    {
        int total = 0;
        foreach (var c in candidates) total += c.Amount;

        int roll = Random.Range(0, total);
        foreach (var c in candidates)
        {
            if (roll < c.Amount) return c;
            roll -= c.Amount;
        }
        return candidates[^1];
    }

    private static QuadOnMap CreateQuadOnMap(QuadSample quadSample, Vector2Int[] points, Vector2Int[] neighbors,
        StructureType structureType)
    {
        var quad = new QuadOnMap(quadSample.data.WidthHeight);
        quad.Setup(points, neighbors, structureType);
        return quad;
    }

    // Quad'ın tüm hücreleri harita içinde ve boş mu? O(1) lookup.
    private static bool QuadIsOnMap(IEnumerable<Vector2Int> quadPoints, Dictionary<Vector2Int, bool> examiningMap)
    {
        foreach (var quadPoint in quadPoints)
        {
            if (!examiningMap.TryGetValue(quadPoint, out var filled) || filled)
                return false;
        }
        return true;
    }

    private static bool HasEmptyPoints(List<Vector2Int> runningMap, List<StructureTypeSearchData> singularTypes,
        out HashSet<QuadOnMap> quads)
    {
        quads = new();
        if (runningMap.Count == 0 || singularTypes == null || singularTypes.Count == 0) return false;

        foreach (var cell in runningMap)
        {
            var searchData = singularTypes[Random.Range(0, singularTypes.Count)];
            var neighbors = QuadProjector.GetNeighborsOnGivenPoint(cell, searchData.QuadSample).ToArray();

            var newQuad = CreateQuadOnMap(
                searchData.QuadSample,
                new[] { cell },
                neighbors,
                searchData.Type);

            quads.Add(newQuad);
        }

        return true;
    }
}