using System;
using System.Collections.Generic;
using UnityEngine;

public static class GridBrusher
{
    public static void BrushSelectedCells(SelectedCellData selectedCellData, OverlayPainter overlayPainter,
        GridData gridData, PaintData paintData)
    {
        if (paintData.CellSizeInWorldUnits <= 0f)
            throw new ArgumentOutOfRangeException(nameof(paintData.CellSizeInWorldUnits),
                "cellSizeInWorldUnits must be > 0");

        if (paintData.BrushRadiusInWorldUnits < 0f)
            return;

        int brushRadiusInCells = Mathf.CeilToInt(paintData.BrushRadiusInWorldUnits / paintData.CellSizeInWorldUnits);

        int brushRadiusSquared = brushRadiusInCells * brushRadiusInCells;


        for (int yOffset = -brushRadiusInCells; yOffset <= brushRadiusInCells; yOffset++)
        {
            for (int xOffset = -brushRadiusInCells; xOffset <= brushRadiusInCells; xOffset++)
            {
                int distanceSquared = xOffset * xOffset + yOffset * yOffset;
                if (distanceSquared > brushRadiusSquared)
                    continue;

                int xIndex = selectedCellData.CellCenterIndex.x + xOffset;
                int yIndex = selectedCellData.CellCenterIndex.y + yOffset;


                if (!GridProjector.IsInsideGrid(xIndex, yIndex, gridData))
                    continue;


                if(GridMasker.TrySetSelected(xIndex, yIndex, selectedCellData.IsPainting))
                    overlayPainter.SetCellPainted(xIndex, yIndex, selectedCellData.IsPainting, gridData);
            }
        }
    }

   
}