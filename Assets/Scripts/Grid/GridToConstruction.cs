using UnityEngine;

public class GridToConstruction: IGridTool
{
    public GridData Data { get; private set; }

    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData)
    {
        Data = gridRelatedData[0] as GridData;
    }

    public void SetSecondaryTools(params IGridTool[] secondaryTools) {}

    public Vector3 GetCellIndexToWorldPositionCenter(int xIndex, int yIndex)
    {
        float worldX = Data.OriginWorldTransform.position.x + (xIndex + 0.5f) * Data.CellSize;
        float worldZ = Data.OriginWorldTransform.position.z + (yIndex + 0.5f) * Data.CellSize;


        float worldY = Data.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }

   
    public Vector3 CellIndexToWorldPositionCorner(int xIndex, int yIndex)
    {
        float worldX = Data.OriginWorldTransform.position.x + xIndex * Data.CellSize;
        float worldZ = Data.OriginWorldTransform.position.z + yIndex * Data.CellSize;
        float worldY = Data.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }
}
