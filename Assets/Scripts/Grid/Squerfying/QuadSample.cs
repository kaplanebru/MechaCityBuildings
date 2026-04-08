using System.Collections.Generic;
using UnityEngine;

public class QuadSample
{
    public Vector2Int WidthHeight { get; private set; }
    public Vector2Int[,] Grid { get; private set; }
    
    public HashSet<Vector2Int> Neighbors { get; private set; }

    public QuadSample(Vector2Int widthHeight)
    {
        WidthHeight = widthHeight;
        
        SetQuadGrid(WidthHeight);
        Neighbors = GetSampleNeighbors(Grid);
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
    
    private static HashSet<Vector2Int> GetSampleNeighbors(Vector2Int[,] quadSample)
    {
        int row = quadSample.GetLength(0);
        int column = quadSample.GetLength(1);

        int lastRow = row - 1;
        int lastColumn = column - 1;
        
        HashSet<Vector2Int> edgeNeighbors = new();

        for (int c = 0; c < column; c++)
        {
            var upper = quadSample[0, c] + Vector2Int.up;
            edgeNeighbors.Add(upper);
            
            var lower = quadSample[lastRow, c] + Vector2Int.down;
            edgeNeighbors.Add(lower);
        }

        for (int r = 0; r < row; r++)
        {
            var left = quadSample[r, 0] + Vector2Int.left;
            edgeNeighbors.Add(left);
            
            var right = quadSample[r, lastColumn] + Vector2Int.right;
            edgeNeighbors.Add(right);
        }
        return edgeNeighbors;
    }
}