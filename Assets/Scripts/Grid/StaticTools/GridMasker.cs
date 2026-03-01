using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public static class GridMasker
{
    private static bool[,] selectedCells;

    public static void SetGridWithinCells(GridData gridData) // todo: On all grid update
    {
        var gridWidthInCells = gridData.AdaptiveGridSize.x;
        var gridHeightInCells = gridData.AdaptiveGridSize.y;

        if (gridWidthInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridWidthInCells));

        if (gridHeightInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridHeightInCells));

        selectedCells = new bool[gridWidthInCells, gridHeightInCells];
        RestoreSelectedCells(gridData);
    }

    public static void SetSelected(int xIndex, int yIndex, bool selected, GridData gridData)
    {
        selectedCells[xIndex, yIndex] = selected;
        UpdateTracker(new Vector2Int(xIndex, yIndex), selected, gridData);
    }

    private static void UpdateTracker(Vector2Int cell, bool selected, GridData gridData)
    {
        if (selected)
        {
            gridData.AddToCellRecordCache(cell);
        }
        else
        {
            gridData.RemoveFromCellRecordCache(cell);
        }
    }

    public static void ResetSelectedCells(OverlayPainter overlayPainter, GridData gridData, bool value = false)
    {
        HashSet<Vector2Int> recorderOutcome = new();
        recorderOutcome.AddRange(gridData.cellRecorderCache);

        foreach (var cell in recorderOutcome)
        {
            SetSelected(cell.x, cell.y, value, gridData);
            overlayPainter.SetCellPainted(cell.x, cell.y, value, gridData);
        }
    }

    public static void RestoreSelectedCells(GridData gridData)
    {
        foreach (var cell in gridData.cellRecorderCache)
        {
            selectedCells[cell.x, cell.y] = true;
        }
    }
    

    /*public void ClearSelectedCells(bool value = false)
    {
        for (int yIndex = 0; yIndex < gridHeightInCells; yIndex++)
        {
            for (int xIndex = 0; xIndex < gridWidthInCells; xIndex++)
            {
                selectedCells[xIndex, yIndex] = value;
            }
        }
    }


    public void ClearOccupiedCells(bool value = false)
    {
        for (int yIndex = 0; yIndex < gridHeightInCells; yIndex++)
        {
            for (int xIndex = 0; xIndex < gridWidthInCells; xIndex++)
            {
                occupiedCells[xIndex, yIndex] = value;
            }
        }
    }*/
}