using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadSearcher
{
    public bool TrySearchQuads(
        Vector2Int widthHeight,
        HashSet<Vector2Int> map,
        int squareAmount,
        List<QuadData> requestedQuads)
    {
        if (widthHeight.x < 2 && widthHeight.y < 2) return false; //todo: ya da base structure ne ebatlardaysa

        QuadSample quadSample = new(widthHeight);
        foreach (var point in map)
        {
            var tempQuadPoints = Quadifyer.GetQuadOnGivenPoint(point, quadSample);

            if (QuadIsOnMap(tempQuadPoints, map))
            {
                requestedQuads.Add(CreateQuadData(point, quadSample, tempQuadPoints.ToArray()));
                if (requestedQuads.Count == squareAmount) break;
            }
        }

        return true;
    }

    private QuadData CreateQuadData(Vector2Int startPoint, QuadSample quadSample, Vector2Int[] points)
    {
        var quadData = new QuadData(quadSample.WidthHeight);
        quadData.Setup(startPoint,
            points.ToArray(),
            Quadifyer.GetNeighborsOnGivenPoint(startPoint, quadSample).ToArray()
        );

        return quadData;
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