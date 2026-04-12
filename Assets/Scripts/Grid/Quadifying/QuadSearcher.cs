using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class QuadSearcher
{

    private static bool HasEmptyPoints(List<Vector2Int> runningMap, StructureTypeData[] typeDatas, out HashSet<QuadOnMap> quads)
    {
        quads = new ();
        if (runningMap.Count == 0)
        {
            Debug.Log("Map exhausted");
            return false;
        }

        typeDatas = typeDatas.Reverse().ToArray();
        List<StructureType> singularQuadTypes = new();
        QuadSample singularQuadSample = null;

        foreach (var typeData in typeDatas)
        {
            if (typeData.QuadSample.data.GetPointAmount == 1)
            {
                singularQuadTypes.Add(typeData.Type);
                singularQuadSample = typeData.QuadSample;
            }
        }
        
        if(singularQuadSample == null) return false;
        
        foreach (var cell in runningMap)
        {
            var newQuad = CreateQuadOnMap(
                cell,
                singularQuadSample, 
                new []{cell},
                singularQuadTypes[Random.Range(0, singularQuadTypes.Count)]); 
            
            quads.Add(newQuad);
        }
        
        return true;
    }
    
    public static HashSet<QuadOnMap> SearchQuads(
        Dictionary<StructureTypeData, int> typeDatasAndAmounts,
        HashSet<Vector2Int> map,
        Dictionary<StructureType, StructureType[]> adjacencyImpossibilities)
    {
        HashSet<QuadOnMap> quads = new();
        
        List<Vector2Int> runningMap = new();
        runningMap.AddRange(map);
        
        var typeDatas = typeDatasAndAmounts.Keys.ToArray();
        typeDatas = typeDatas.OrderByDescending(ti => ti.QuadSample.data.GetPointAmount).ToArray();
        
        SlotTypePossibilityHandler possibilityHandler = new(map, adjacencyImpossibilities);
        
        foreach (var typeData in typeDatas)
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
                        tempQuadPoints.ToArray(),
                        typeData.Type); 
                    
                    
                    possibilityHandler.UpdateNeighbourPossibilities(newQuad, typeData.Type);
                    quads.Add(newQuad);
                    runningMap.RemoveAll(tempQuadPoints.Contains);//coord => newQuad.data.Coords.Contains(coord));
                    index -= newQuad.data.Coords.Length;
                    
                     typeDatasAndAmounts[typeData]--;
                     if (typeDatasAndAmounts[typeData] <= 0) break;
                }
                else
                {
                    index--;
                }
            }
        }
        
        if(HasEmptyPoints(runningMap, typeDatas, out var remainingQuads))
            quads.UnionWith(remainingQuads);
        
        return quads;
    }

    private static QuadOnMap CreateQuadOnMap(Vector2Int startPoint, QuadSample quadSample, Vector2Int[] points, StructureType slotType)
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
