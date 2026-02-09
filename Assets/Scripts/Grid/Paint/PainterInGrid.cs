using System;
using UnityEngine;

public class PainterInGrid : IGridTool
{
    private PaintData _paintData;
    private GridProjector _projector;
    private GridMasker _masker;

    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData)
    {
        _paintData = gridRelatedData[1] as PaintData;
    }

    public void SetSecondaryTools(params IGridTool[] secondaryTools)
    {
        foreach (var tool in secondaryTools)
            if (tool is GridProjector)
                _projector = tool as GridProjector;
            else if (tool is GridMasker)
                _masker = tool as GridMasker;
    }

    public void PaintSelectedCellsInBrush(bool paintValue, Vector2Int centerCellIndex)
    {
        if (_paintData.CellSizeInWorldUnits <= 0f)
            throw new ArgumentOutOfRangeException(nameof(_paintData.CellSizeInWorldUnits),
                "cellSizeInWorldUnits must be > 0");

        if (_paintData.BrushRadiusInWorldUnits < 0f)
            return;

        int brushRadiusInCells = Mathf.CeilToInt(_paintData.BrushRadiusInWorldUnits / _paintData.CellSizeInWorldUnits);

        int brushRadiusSquared = brushRadiusInCells * brushRadiusInCells;


        for (int yOffset = -brushRadiusInCells; yOffset <= brushRadiusInCells; yOffset++)
        {
            for (int xOffset = -brushRadiusInCells; xOffset <= brushRadiusInCells; xOffset++)
            {
                int distanceSquared = xOffset * xOffset + yOffset * yOffset;
                if (distanceSquared > brushRadiusSquared)
                    continue;

                int xIndex = centerCellIndex.x + xOffset;
                int yIndex = centerCellIndex.y + yOffset;


                if (!_projector.IsInsideGrid(xIndex, yIndex))
                    continue;

                _masker.SetSelected(xIndex, yIndex, paintValue);
            }
        }
    }
}