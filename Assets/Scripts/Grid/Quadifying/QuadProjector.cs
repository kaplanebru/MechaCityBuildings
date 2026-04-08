using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadProjector
{
    
    public static IEnumerable<Vector2Int> GetNeighborsOnGivenPoint(Vector2Int point, QuadSample quadSample)
    {
        var sampleNeighbors = quadSample.data.Neighbors;
        return sampleNeighbors.Select(sn => sn + point);
    }

    public static IEnumerable<Vector2Int> GetQuadOnGivenPoint(Vector2Int point, QuadSample quadSample)
    {
        var sampleCoors = quadSample.data.Coords;
        return sampleCoors.Select(sc => sc + point);
    }
}