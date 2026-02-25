using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridMasker 
{
    private static bool[,] selectedCells;
    private static bool[,] occupiedCells;

    private static HashSet<Vector2Int> tempCellTracker = new();
    
    public static void SetGridWithinCells(GridData gridData)
    {
        var gridWidthInCells = gridData.AdaptiveGridSize.x;
        var gridHeightInCells = gridData.AdaptiveGridSize.y;

        if (gridWidthInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridWidthInCells));

        if (gridHeightInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridHeightInCells));

        selectedCells = new bool[gridWidthInCells, gridHeightInCells];
        occupiedCells = new bool[gridWidthInCells, gridHeightInCells];
    }

    public static HashSet<Vector2Int> RegisterTrackedCells()
    {
        HashSet<Vector2Int> competedTrack = new HashSet<Vector2Int>();
        competedTrack.UnionWith(tempCellTracker);
        ResetSelectedCells();

        return competedTrack;
    }

    public static void SetSelected(int xIndex, int yIndex, bool selected)
    {
        selectedCells[xIndex, yIndex] = selected;
        UpdateTracker(new Vector2Int(xIndex, yIndex), selected);
    }

    private static void UpdateTracker(Vector2Int point, bool selected)
    {
        if (selected)
        {
            tempCellTracker.Add(point);
        }
        else
        {
            tempCellTracker.Remove(point);
        }
    }

    private static void ResetSelectedCells(bool value = false)
    {
        foreach (var cell in tempCellTracker)
        {
            selectedCells[cell.x, cell.y] = value;
            //todo: overlayPainter.SetCellPainted(cell.x, cell.y, value);
        }

        tempCellTracker.Clear();
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