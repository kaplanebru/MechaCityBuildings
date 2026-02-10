using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMasker : IGridTool
{
    private bool[,] selectedCells;
    private bool[,] occupiedCells;

    //private List<Vector2Int> cellTracker = new();
    public Dictionary<int, HashSet<Vector2Int>> CellTrackByFloor { get; private set; }= new();
    private int currentFloor = 0;

    private int gridWidthInCells;
    private int gridHeightInCells;

    private GridData Data;
    private OverlayGridPainter overlayPainter;

    public void SetOverlayPainter(OverlayGridPainter overlayPainter)
    {
        this.overlayPainter = overlayPainter;
    }

    public void UpdateCellTrackingFloor(int floor)
    {
        currentFloor = floor;
    }

    public void DeleteLastTracker()  //(int floor)
    {
        int lastFloor = CellTrackByFloor.Count - 1;
        CellTrackByFloor.Remove(lastFloor);
        //cellTrackByFloor.Remove(floor);
    }

    public HashSet<Vector2Int> GetTrackedCells()
    {
        List<Vector2Int> completedTracker = new();
        completedTracker.AddRange(CellTrackByFloor[currentFloor]);

        return completedTracker.ToHashSet();
    }

    public void SetSelected(int xIndex, int yIndex, bool selected)
    {
        selectedCells[xIndex, yIndex] = selected;
        overlayPainter.SetCellPainted(xIndex, yIndex, selected);
        UpdateTracker(new Vector2Int(xIndex, yIndex), selected);
    }

    private void UpdateTracker(Vector2Int point, bool selected)
    {
        if (selected)
        {
            if (!CellTrackByFloor.ContainsKey(currentFloor))
                CellTrackByFloor[currentFloor] = new ();

            CellTrackByFloor[currentFloor].Add(point);
            //cellTracker.Add(point);
        }

        else
        {
            if (CellTrackByFloor.ContainsKey(currentFloor))
                CellTrackByFloor[currentFloor].Remove(point);
            //cellTracker.Remove(point);
        }
    }

    public void SetSecondaryTools(params IGridTool[] secondaryTools)
    {
    }


    public void ClearSelectedCells(bool value = false)
    {
        for (int yIndex = 0; yIndex < gridHeightInCells; yIndex++)
        {
            for (int xIndex = 0; xIndex < gridWidthInCells; xIndex++)
            {
                selectedCells[xIndex, yIndex] = value;
            }
        }
    }

    public void RestoreSelectedCells(bool value = false)
    {
        var tracker = CellTrackByFloor[currentFloor];
        foreach (var cell in tracker)
        {
            selectedCells[cell.x, cell.y] = value;
            overlayPainter.SetCellPainted(cell.x, cell.y, value);
        }

        tracker.Clear();
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
    }

    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData)
    {
        Data = (GridData)gridRelatedData[0];

        gridWidthInCells = Data.AdaptiveGridSize.x;
        gridHeightInCells = Data.AdaptiveGridSize.y;

        if (gridWidthInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridWidthInCells));

        if (gridHeightInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridHeightInCells));

        selectedCells = new bool[gridWidthInCells, gridHeightInCells];
        occupiedCells = new bool[gridWidthInCells, gridHeightInCells];
    }
}