using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Android;


public class QuadSearcher
{
    //todo: listeden çıkarmak yerine quad cell'leri sealed yapabılabilir. if selaed continue.
    //Acaba randomize edilmese nasıl çalışır?
    //MAP ORGANİZER 79. SATIR COMMENTLİ
    private static HashSet<QuadOnMap> SearchQuadsInGivenType(StructureTypeSearchData structureTypeData, List<Vector2Int> runningMap, SlotTypePossibilityHandler possibilityHandler)
    {
        List<Vector2Int> tempMap = new();
        tempMap.AddRange(runningMap);
        
        HashSet<QuadOnMap> selectedQuads = new();
        int index = 0;
            
        while (index < tempMap.Count)
        {
            var examinedPoint = tempMap[index];

            var tempQuadPoints =
                QuadProjector.GetQuadOnGivenPoint(examinedPoint, structureTypeData.QuadSample).ToHashSet();

            if (QuadIsOnMap(tempQuadPoints, tempMap))
            {
                //başka quadlar aradığı için quad sayısından fazla oluyor
                var tempNeighbors = QuadProjector.GetNeighborsOnGivenPoint(examinedPoint, structureTypeData.QuadSample).ToArray();
                if (!possibilityHandler.IsTypeConvenient2(structureTypeData.ImpossibleStructureTypes, tempNeighbors)) //tempnEİGHBORS tempQuadPoints.ToArray()
                {
                    index++;
                    //tempMap.RemoveAll(tempQuadPoints.Contains);
                    // belki bu eksik quaddaki pointler başka quad içinde işlevseldir diye temp'leri remove etmedim
                    //ama remove edilecekleri durumda da index++ olmaması gerekir, point 5. indexteyse remove ettikten
                    //sonra listedeki 5. index'in elemanı başka bir point olur
                }
                else
                {
                    var newQuad = CreateQuadOnMap(
                        examinedPoint,
                        structureTypeData.QuadSample,
                        tempQuadPoints.ToArray(),
                        structureTypeData.Type);
                    
                    possibilityHandler.UpdateNeighbourPossibilities(newQuad, structureTypeData.Type);
                    selectedQuads.Add(newQuad);
                    tempMap.RemoveAll(tempQuadPoints.Contains);
                    //index++;
                    
                    structureTypeData.Amount--;
                    if (structureTypeData.Amount <= 0) break;
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
            List<StructureTypeSearchData> structureTypeDatas,
            HashSet<Vector2Int> map)
    {
        HashSet<QuadOnMap> quads = new();

        List<Vector2Int> runningMap = new();
        runningMap.AddRange(map);

        structureTypeDatas = structureTypeDatas.OrderByDescending(sd => sd.QuadSample.data.GetPointAmount).ToList();

        SlotTypePossibilityHandler possibilityHandler = new(map, structureTypeDatas.ToHashSet());

        foreach (var structureTypeData in structureTypeDatas)
        {
            foreach (var cell in quads.SelectMany(quad => quad.data.Coords))
            {
                runningMap.Remove(cell);
            }
            
            quads.UnionWith(SearchQuadsInGivenType(structureTypeData, runningMap, possibilityHandler));
        }

        if(HasEmptyPoints(runningMap, structureTypeDatas, out var remainingQuads))
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