using System;
using UnityEngine;

public class GridPainter : IGridTool
{
    private PaintData _paintData;
    private GridSearcher _searcher;
    private GridMasker _masker;

    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData)
    {
        _paintData = gridRelatedData[1] as PaintData;
    }

    public void SetSecondaryTools(params IGridTool[] secondaryTools)
    {
        foreach (var tool in secondaryTools)
            if (tool is GridSearcher)
                _searcher = tool as GridSearcher;
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

        // Convert brush radius from world units (meters) to cell units.
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


                if (!_searcher.IsInsideGrid(xIndex, yIndex))
                    continue;

                //_masker.selectedCells[xIndex, yIndex] = paintValue;
                _masker.SetSelected(xIndex, yIndex, paintValue);
            }
        }
    }
}