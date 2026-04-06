using System.Collections.Generic;
using UnityEngine;

public class SquareSearcher
{
    public void SearchSquares(int pow, HashSet<Vector2Int> map, int squareAmount)
    {
        
        foreach (var point in map)
        {
           if(Squarefyer.TryGetSquarePoints(pow, point, out var squarePoints)) return;

           if (SquareIsOnMap(squarePoints, map))
           {
               
           }
          
        }
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
