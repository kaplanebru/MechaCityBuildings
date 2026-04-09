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
        List<Vector2Int> tempMap = new();
        tempMap.AddRange(map);

        var quadSamples = quadSamplesAndAmounts.Keys.ToArray();
        quadSamples = quadSamples.OrderByDescending(qs => qs.data.GetPointAmount).ToArray();

        foreach (var quadSample in quadSamples)
        {
            for (int i = tempMap.Count - 1; i >= 0; i--)
            {
                var examinedPoint = tempMap[i];
                int counter = 0;
                var tempQuadPoints = 
                    QuadProjector.GetQuadOnGivenPoint(examinedPoint, quadSample).ToHashSet();
                
                if (QuadIsOnMap(tempQuadPoints, tempMap.ToHashSet()))
                {
                    requestedQuads.Add(CreateQuadOnMap(examinedPoint, quadSample, tempQuadPoints.ToArray()));
                    tempQuadPoints.Remove(examinedPoint);
                    
                    if (counter >= quadSamplesAndAmounts[quadSample]) break;
                }
            }
        }
        
        return requestedQuads.ToArray();
       
         
        //todo: quad bulunca map'i güncelle
       
        /*foreach (var examinedPoint in map)
        {
            var tempQuadPoints = QuadProjector.GetQuadOnGivenPoint(examinedPoint, quadSample).ToHashSet();

            if (QuadIsOnMap(tempQuadPoints, map))
            {
                requestedQuads.Add(CreateQuadOnMap(examinedPoint, quadSample, tempQuadPoints.ToArray()));
                if (requestedQuads.Count == quadAmount) break;
            }
        }
        return requestedQuads.ToArray();*/
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