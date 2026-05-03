using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadProjector
{
    
    public static IEnumerable<Vector2Int> GetNeighborsOnGivenPoint(Vector2Int point, QuadSample quadSample)
    {
        var sampleNeighbors = quadSample.data.Neighbors;
        return sampleNeighbors.Select(sn => sn + point);
        //neigbor map'te olmayabilir
    }

    public static IEnumerable<Vector2Int> GetQuadOnGivenPoint(Vector2Int point, QuadSample quadSample)
    {
        var sampleCoors = quadSample.data.Coords;
        return sampleCoors.Select(sc => sc + point);
    }

    public static Vector2Int[] GetFilteredNeighborsOnGivenPoint(
        Vector2Int point, QuadSample quadSample, HashSet<Vector2Int> map, out Vector2Int missingNeighborSum)
    {
        missingNeighborSum = Vector2Int.zero;
        List<Vector2Int> neighbors = new List<Vector2Int>();
        var tempNeighbors = GetNeighborsOnGivenPoint(point, quadSample);

        foreach (var tempNeighbor in tempNeighbors)
        {
            if (!map.Contains(tempNeighbor))
            {
               missingNeighborSum += tempNeighbor;
            }
            else
            {
                 neighbors.Add(tempNeighbor);
            }
        }
        return neighbors.ToArray();
    }
}