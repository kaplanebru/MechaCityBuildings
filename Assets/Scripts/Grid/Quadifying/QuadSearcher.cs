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
    public bool TrySearchQuads(
        QuadSample quadSample,
        HashSet<Vector2Int> map,
        int quadAmount,
        out List<QuadOnMap> requestedQuads)
    {
        requestedQuads = new();
        if (quadSample.data.WidthHeight.x < 2 && quadSample.data.WidthHeight.y < 2) return false; //todo: ya da base structure ne ebatlardaysa
        
        foreach (var examinedPoint in map)
        {
            var tempQuadPoints = QuadProjector.GetQuadOnGivenPoint(examinedPoint, quadSample).ToHashSet();

            if (QuadIsOnMap(tempQuadPoints, map))
            {
                requestedQuads.Add(CreateQuadOnMap(examinedPoint, quadSample, tempQuadPoints.ToArray()));
                if (requestedQuads.Count == quadAmount) break;
            }
        }
        return true;
    }

    private QuadOnMap CreateQuadOnMap(Vector2Int startPoint, QuadSample quadSample, Vector2Int[] points)
    {
        var quad = new QuadOnMap(quadSample.data.WidthHeight, startPoint);
        quad.SetPoints(
            points.ToArray(),
            QuadProjector.GetNeighborsOnGivenPoint(startPoint, quadSample).ToArray()
        );

        return quad;
    }

    private bool QuadIsOnMap(HashSet<Vector2Int> quadPoints, HashSet<Vector2Int> map)
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