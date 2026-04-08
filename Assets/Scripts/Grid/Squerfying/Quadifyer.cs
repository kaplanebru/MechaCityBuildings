using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quadifyer
{
    
    public static IEnumerable<Vector2Int> GetNeighborsOnGivenPoint(Vector2Int point, QuadSample quadSample)
    {
        var sampleNeighbors = quadSample.Neighbors;
        return sampleNeighbors.Select(sampleNeighbor => sampleNeighbor + point);
    }

    public static HashSet<Vector2Int> GetQuadOnGivenPoint(Vector2Int point, QuadSample quadSample)
    {
        int width = quadSample.WidthHeight[0];
        int height = quadSample.WidthHeight[1];

        // var quadPoints = new Vector2Int[width * height];
        List<Vector2Int> quadPoints = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var a = quadSample.Grid[x, y] + point;
                quadPoints.Add(a);
            }
        }

        return quadPoints.ToHashSet();
    }

   
}