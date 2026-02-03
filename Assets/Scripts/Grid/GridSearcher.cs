using UnityEngine;

public class GridSearcher: IGridTool
{
    public GridData Data { get; private set; }

    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData)
    {
        Data = gridRelatedData[0] as GridData;
    }

    public void SetSecondaryTools(params IGridTool[] secondaryTools) {}

    private int GridWidthInCells => Data.GridSize.x;
    private int GridHeightInCells => Data.GridSize.y;
    
    public bool TryWorldPositionToCellIndex(Vector3 worldPosition, out Vector2Int cellIndex)
    {
        cellIndex = WorldPositionToCellIndex(worldPosition);
        return IsInsideGrid(cellIndex.x, cellIndex.y);
    }
    
    public bool IsInsideGrid(int xIndex, int yIndex)
    {
        return xIndex >= 0 
               && xIndex < GridWidthInCells 
               && yIndex >= 0
               && yIndex < GridHeightInCells;
    }
    
    private Vector2Int WorldPositionToCellIndex(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - Data.OriginWorld;

        int xIndex = Mathf.FloorToInt(localPosition.x / Data.CellSize);
        int yIndex = Mathf.FloorToInt(localPosition.z / Data.CellSize);

        return new Vector2Int(xIndex, yIndex);
    }
}
