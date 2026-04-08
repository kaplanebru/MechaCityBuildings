using System.Collections.Generic;
using UnityEngine;

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
    
    public static void GetNeighbors(Vector2Int[,] quadGrid)
    {
        int row = quadGrid.GetLength(0);
        int column = quadGrid.GetLength(1);

        if (row < 2 || column < 2)
        {
            //todo: if one dimensional handle wih different algorithm
            return;
        }

        int lastRow = row - 1;
        int lastColumn = column - 1;
        
        Vector2Int[] corners =
        {
            quadGrid[0, 0],
            quadGrid[lastRow, 0],
            quadGrid[0, lastColumn],
            quadGrid[lastRow, lastColumn]
        };

        List<Vector2Int> edges = new();

        for (int i = 0; i < column; i++)
        {
            edges.Add(quadGrid[0, i]);
            edges.Add(quadGrid[lastRow, i]);
        }

        for (int i = 0; i < row; i++)
        {
            edges.Add(quadGrid[i, column]);
            edges.Add(quadGrid[i, lastColumn]);
        }

        foreach (var corner in corners)
        {
            edges.Remove(corner);
        }
    }
}