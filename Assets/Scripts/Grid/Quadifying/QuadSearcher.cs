using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
    //todo: tek tip quad için geçerli
    public static QuadOnMap[] SearchQuads(
        Dictionary<QuadSample, int> quadSamplesAndAmounts,
        HashSet<Vector2Int> map)
    {
        List<QuadOnMap> requestedQuads = new();
        List<Vector2Int> runningMap = new();
        runningMap.AddRange(map);

        var quadSamples = quadSamplesAndAmounts.Keys.ToArray();
        quadSamples = quadSamples.OrderByDescending(qs => qs.data.GetPointAmount).ToArray();

        foreach (var quadSample in quadSamples)
        {
            int index = runningMap.Count - 1;
            while (index >= 0)
            {
                Debug.Log(index);

                var examinedPoint = runningMap[index];
                int quadCounter = 0;
                
                var tempQuadPoints = 
                    QuadProjector.GetQuadOnGivenPoint(examinedPoint, quadSample).ToHashSet();
                
                if (QuadIsOnMap(tempQuadPoints, runningMap.ToHashSet()))
                {
                    var newQuad = CreateQuadOnMap(examinedPoint, quadSample, tempQuadPoints.ToArray());
                    requestedQuads.Add(newQuad);

                    runningMap.RemoveAll(coord => newQuad.data.Coords.Contains(coord));
                    index -= newQuad.data.Coords.Length;
                    
                    quadCounter++;
                    if (quadCounter >= quadSamplesAndAmounts[quadSample]) break;
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

//todo: cellData'ya center ekle, points List ekle. Cell yerine slot da diyebiliriz belki