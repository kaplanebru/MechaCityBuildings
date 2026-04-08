using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Quadifyer
{
    public static HashSet<Vector2Int> GetQuadSlotsByPoint(Vector2Int widthHeight, Vector2Int point)
    {
        QuadSample quadSample = new(widthHeight);
        return ApplyQuadToGivenPoint(point, quadSample.Grid);
    }

    private static HashSet<Vector2Int> ApplyQuadToGivenPoint(Vector2Int point, Vector2Int[,] quadGrid)
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

public class QuadSample
{
    public Vector2Int WidthHeight { get; private set; }
    public Vector2Int[,] Grid { get; private set; }

    public QuadSample(Vector2Int widthHeight)
    {
        WidthHeight = widthHeight;
        SetQuadGrid(WidthHeight);
    }

    private void SetQuadGrid(Vector2Int widthHeight)
    {
        var quadGrid = new Vector2Int[widthHeight.x, widthHeight.y];

        for (int row = 0; row < widthHeight.x; row++)
        {
            for (int col = 0; col < widthHeight.y; col++)
            {
                quadGrid[row, col] = new Vector2Int(col, row);
            }
        }

        Grid = quadGrid;
    }
}