using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

public class QuadSearcher
{
    public QuadSample[] quadSamples;

    public QuadSample SelectSampleBySize(Vector2Int widthHeight)
    {
        var sample = quadSamples.FirstOrDefault(wh=>wh.data.WidthHeight == widthHeight);
        
        if (sample == null)
        {
            Debug.LogError($"{nameof(QuadSearcher)} could not find quad sample {widthHeight}");
            return null;
        }
        return sample;
    }
    
    public static QuadOnMap[] SearchQuads(
        Dictionary<StructureTypeData, int> typeInfosAndAmounts,
        HashSet<Vector2Int> map)
    {
        List<QuadOnMap> requestedQuads = new();
        List<Vector2Int> runningMap = new();
        runningMap.AddRange(map);
        SlotTypePossibilityHandler possibilityHandler = new();

        var typeInfos = typeInfosAndAmounts.Keys.ToArray();
        
        possibilityHandler.Initiate(
            map, 
            typeInfos.Select(k=>k.Type).ToHashSet());
        

        typeInfos = typeInfos.OrderByDescending(
            ti => ti.QuadSample.data.GetPointAmount).ToArray();

        foreach (var typeInfo in typeInfos)
        {
            int index = runningMap.Count - 1;
            while (index >= 0)
            {
                var examinedPoint = runningMap[index];
                int quadCounter = 0;
                
                var tempQuadPoints = 
                    QuadProjector.GetQuadOnGivenPoint(examinedPoint, typeInfo.QuadSample).ToHashSet();
                
                if (QuadIsOnMap(tempQuadPoints, runningMap.ToHashSet())) //T4
                {
                    //TODO: check convenience in type
                    
                    var newQuad = CreateQuadOnMap(examinedPoint, typeInfo.QuadSample, tempQuadPoints.ToArray());
                    requestedQuads.Add(newQuad);

                    runningMap.RemoveAll(coord => newQuad.data.Coords.Contains(coord));
                    index -= newQuad.data.Coords.Length;
                    
                    quadCounter++;
                    if (quadCounter >= typeInfosAndAmounts[typeInfo]) break;
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
        var quad = new QuadOnMap(quadSample.data.WidthHeight, startPoint);
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
