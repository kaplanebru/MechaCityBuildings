using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quadifyer
{
    public static HashSet<Vector2Int> GetQuadSlotsByPoint(Vector2Int widthHeight, Vector2Int point)
    {
        QuadSample quadSample = new(widthHeight);
        return ApplyQuadOnGivenPoint(point, quadSample.Grid);
    }

    private static HashSet<Vector2Int> ApplyQuadOnGivenPoint(Vector2Int point, Vector2Int[,] quadGrid)
    {
        int width = quadGrid.GetLength(0);
        int height = quadGrid.GetLength(1);

        // var quadPoints = new Vector2Int[width * height];
        List<Vector2Int> quadPoints = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var a = quadGrid[x, y] + point;
                quadPoints.Add(a);
            }
        }

        return quadPoints.ToHashSet();
    }
}