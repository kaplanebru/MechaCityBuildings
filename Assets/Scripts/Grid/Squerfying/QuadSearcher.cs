using System.Collections.Generic;
using UnityEngine;

public class QuadSearcher
{
    public bool TrySearchQuads(Vector2Int widthHeight, HashSet<Vector2Int> map, int squareAmount, List<QuadData> requestedQuads)
    {
        if (widthHeight.x < 2 && widthHeight.y < 2) return false; //todo: ya da base structure ne ebatlardaysa
        
        
        foreach (var point in map)
        {
            var tempQuadPoints = Quadifyer.GetQuadSlotsByPoint(widthHeight, point);

            if (QuadIsOnMap(tempQuadPoints, map))
            {
                requestedQuads.Add(new QuadData(widthHeight, point));
                if (requestedQuads.Count == squareAmount) break;
            }
        }
        return true;
    }

    private bool QuadIsOnMap(HashSet<Vector2Int> quadPoints, HashSet<Vector2Int> map)
    {
        foreach (var quadPoint in quadPoints)
        {
            if (!map.Contains(quadPoint))
                return false;
        }
        return true;
    }
}

//todo: cellData'ya center ekle, points List ekle. Cell yerine slot da diyebiliriz belki