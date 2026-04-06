using System.Collections.Generic;
using UnityEngine;

public class SquareSearcher
{
    public bool TrySearchSquares(int pow, HashSet<Vector2Int> map, int squareAmount, List<SquareData> requestedSquares)
    {
        if (pow < 2) return false;
        
        foreach (var point in map)
        {
            var tempSquarePoints = Squarefyer.GetSquareSlotsByPoint(pow, point);

            if (SquareIsOnMap(tempSquarePoints, map))
            {
                requestedSquares.Add(new SquareData(pow, point));
                if (requestedSquares.Count == squareAmount) break;
            }
        }
        return true;
    }

    private bool SquareIsOnMap(HashSet<Vector2Int> squarePoints, HashSet<Vector2Int> map)
    {
        foreach (var squarePoint in squarePoints)
        {
            if (!map.Contains(squarePoint))
                return false;
        }

        return true;
    }
}

//todo: cellData'ya center ekle, points List ekle. Cell yerine slot da diyebiliriz belki