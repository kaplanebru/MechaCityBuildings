using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Squarefyer
{

    public static bool TryGetSquarePoints(int pow, Vector2Int point, out HashSet<Vector2Int> squarePoints)
    {
        squarePoints = null;
        if(pow < 2) return false;
        
        squarePoints = ApplySquareToGivenPoint(point, GetSquareGrid(pow));
        return true;
    }
    
    
    private static Vector2Int[,] GetSquareGrid(int pow)
    {
        var squareGrid = new Vector2Int[pow, pow];

        for (int row = 0; row < pow; row++)
        {
            for (int col = 0; col < pow; col++)
            {
                squareGrid[row, col] = new Vector2Int(col, row);
            }
        }
        return squareGrid;
    }

    private static HashSet<Vector2Int> ApplySquareToGivenPoint(Vector2Int point, Vector2Int[,] squareGrid)
    {
        int width = squareGrid.GetLength(0);  
        int height = squareGrid.GetLength(1);
        
       // var squarePoints = new Vector2Int[width * height];
       List<Vector2Int> squarePoints = new();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var a = squareGrid[x, y] + point;
                squarePoints.Add(a);
            }
        }
        return squarePoints.ToHashSet();
    }
}
