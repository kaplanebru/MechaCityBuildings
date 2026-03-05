using System.Collections.Generic;
using UnityEngine;

public static class GridProjector
{
    public static bool TryWorldPositionToCellIndex(Vector3 worldPosition, GridData gridData, out Vector2Int cellIndex)
    {
        cellIndex = WorldPositionToCellIndex(worldPosition, gridData);
        return IsInsideGrid(cellIndex.x, cellIndex.y, gridData);
    }
    
    public static bool IsInsideGrid(int xIndex, int yIndex, GridData gridData)
    {
        return xIndex >= 0 
               && xIndex < gridData.AdaptiveGridSize.x 
               && yIndex >= 0
               && yIndex < gridData.AdaptiveGridSize.y;
    }
    
    private static Vector2Int WorldPositionToCellIndex(Vector3 worldPosition, GridData gridData)
    {
        Vector3 localPosition = worldPosition - gridData.OriginWorldTransform.position;

        int xIndex = Mathf.FloorToInt(localPosition.x / gridData.BuildingCellSize);
        int yIndex = Mathf.FloorToInt(localPosition.z / gridData.BuildingCellSize);

        return new Vector2Int(xIndex, yIndex);
    }
}
