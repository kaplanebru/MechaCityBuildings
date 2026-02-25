using System.Collections.Generic;
using UnityEngine;

public class BoundaryFinder
{

    //TODO: Unitlerim 1 olmayabilir

    private static HashSet<Vector2Int> GetBoundary4(HashSet<Vector2Int> filled, int cellUnit)
    {
        var boundary = new HashSet<Vector2Int>();

        foreach (var cell in filled)
        {
            if (!filled.Contains(cell + Vector2Int.up * cellUnit) ||
                !filled.Contains(cell + Vector2Int.down * cellUnit) ||
                !filled.Contains(cell + Vector2Int.left * cellUnit) ||
                !filled.Contains(cell + Vector2Int.right * cellUnit))
            {
                boundary.Add(cell);
            }
        }

        return boundary;
    }

    public static HashSet<Vector2Int> GetBoundsWithInner(int layer, IReadOnlyCollection<Vector2Int> filled, int unit = 1)
    {
        if (layer <= 0)
            layer = 1;
        if (filled == null || filled.Count == 0)
            return new HashSet<Vector2Int>();

        var remaining = new HashSet<Vector2Int>(filled);
        var allBoundary = new HashSet<Vector2Int>();

        for (int i = 0; i < layer; i++)
        {
            if (remaining.Count == 0)
                break;

            var currentBoundary = GetBoundary4(remaining, unit);
            if (currentBoundary.Count == 0)
                break;

            foreach (var b in currentBoundary)
            {
                allBoundary.Add(b);
                remaining.Remove(b);
            }
        }

        return new HashSet<Vector2Int>(allBoundary);
    }

    public static List<Vector2Int> GetBoundary8(IReadOnlyCollection<Vector2Int> filled)
    {
        var set = filled as HashSet<Vector2Int> ?? new HashSet<Vector2Int>(filled);

        var boundary = new List<Vector2Int>();

        foreach (var c in set)
        {
            for (int dy = -1; dy <= 1; dy++)
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;

                if (!set.Contains(new Vector2Int(c.x + dx, c.y + dy)))
                {
                    boundary.Add(c);
                    dx = dy = 2; // 
                }
            }
        }

        return boundary;
    }
}