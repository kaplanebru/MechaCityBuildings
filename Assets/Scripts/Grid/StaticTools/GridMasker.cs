using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public static class GridMasker
{
    private static bool[,] selectedCells;
    private static List<Vector2Int> cellRecorderCache = new();

    public static void SetGridWithinCells(GridData gridData, List<Vector2Int> cellRecorder) // todo: On all grid update
    {
        var gridWidthInCells = gridData.AdaptiveGridSize.x;
        var gridHeightInCells = gridData.AdaptiveGridSize.y;

        if (gridWidthInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridWidthInCells));

        if (gridHeightInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridHeightInCells));
        
        selectedCells = new bool[gridWidthInCells, gridHeightInCells];
        RestoreSelectedCells(cellRecorder);
    }
    
    public static bool TrySetSelected(int xIndex, int yIndex, bool selected)
    {
        if (selectedCells == null)
        {
            Eventbus.OnReloadCall?.Invoke();
            return false;
        }
        
        selectedCells[xIndex, yIndex] = selected;
        UpdateTracker(new Vector2Int(xIndex, yIndex), selected);
        return true;
    }

    private static void UpdateTracker(Vector2Int cell, bool selected)
    {
        if (selected)
        {
            if (!cellRecorderCache.Contains(cell))
            {
                cellRecorderCache.Add(cell);
                //todo: set dirty if needed
            }
        }
        else
        {
            if (cellRecorderCache.Contains(cell))
            {
                cellRecorderCache.Remove(cell);
                //todo: set dirty if needed
            }
        }
    }

    public static void ResetSelectedCells(OverlayPainter overlayPainter, GridData gridData, bool value = false)
    {
        HashSet<Vector2Int> recorderOutcome = new();
        recorderOutcome.AddRange(cellRecorderCache);

        foreach (var cell in recorderOutcome)
        {
            if(TrySetSelected(cell.x, cell.y, value))
                overlayPainter.SetCellPainted(cell.x, cell.y, value, gridData);
        }
    }

    public static void RestoreSelectedCells(List<Vector2Int> cellRecorder)
    {
        cellRecorderCache = cellRecorder;
        foreach (var cell in cellRecorderCache)
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