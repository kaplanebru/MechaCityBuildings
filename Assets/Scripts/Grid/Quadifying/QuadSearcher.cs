using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class QuadSearcher
{
    public static QuadOnMap[] SearchQuads(
        Dictionary<StructureTypeData, int> typeInfosAndAmounts,
        HashSet<Vector2Int> map,
        Dictionary<StructureType, StructureType[]> adjacencyImpossibilities)
    {
        List<QuadOnMap> requestedQuads = new();
        List<Vector2Int> runningMap = new();
        runningMap.AddRange(map);
        var typeInfos = typeInfosAndAmounts.Keys.ToArray();

        SlotTypePossibilityHandler possibilityHandler = new( map, 
            typeInfos.Select(k=>k.Type).ToHashSet(),
            adjacencyImpossibilities);
        
        typeInfos = typeInfos.OrderByDescending(ti => ti.QuadSample.data.GetPointAmount).ToArray();

        foreach (var typeData in typeInfos)
        {
            int index = runningMap.Count - 1;
            while (index >= 0)
            {
                var examinedPoint = runningMap[index];
               
                var tempQuadPoints = 
                    QuadProjector.GetQuadOnGivenPoint(examinedPoint, typeData.QuadSample).ToHashSet();
                
                if (QuadIsOnMap(tempQuadPoints, runningMap.ToHashSet()))
                {
                    if (!possibilityHandler.IsTypeConvenient(typeData.Type, tempQuadPoints.ToArray()))
                    {
                        index--;
                        continue;
                    }
                    
                    var newQuad = CreateQuadOnMap(
                        examinedPoint,
                        typeData.QuadSample, 
                        tempQuadPoints.ToArray()); 
                    
                    if(typeData.QuadSample.data.GetPointAmount == 4)
                        Debug.Log("big quad");
                    
                    possibilityHandler.UpdateNeighbourPossibilities(newQuad, typeData.Type);
                    requestedQuads.Add(newQuad);
                    runningMap.RemoveAll(coord => newQuad.data.Coords.Contains(coord));
                    index -= newQuad.data.Coords.Length;
                    
                     typeInfosAndAmounts[typeData]--;
                     if (typeInfosAndAmounts[typeData] <= 0) break;
                }
                else
                {
                    index--;
                }
            }
        }
        
        return requestedQuads.ToArray();
    }

    private static QuadOnMap CreateQuadOnMap(Vector2Int startPoint, QuadSample quadSample, Vector2Int[] points)
    {
        var quad = new QuadOnMap(quadSample.data.WidthHeight);
        quad.Setup(
            points.ToArray(),
            QuadProjector.GetNeighborsOnGivenPoint(startPoint, quadSample).ToArray()
        );

        return quad;
    }

    private static bool QuadIsOnMap(HashSet<Vector2Int> quadPoints, HashSet<Vector2Int> map)
    {
        foreach (var quadPoint in quadPoints)
        {
            if (!map.Contains(quadPoint))
                return false;
        }
        return true;
    }
}
