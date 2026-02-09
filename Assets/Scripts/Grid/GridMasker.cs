using System;
using System.Collections.Generic;
using UnityEngine;

public class GridMasker: IGridTool
{
    private bool[,] selectedCells;
    private bool[,] occupiedCells;
    
    private List<Vector2Int> cellTracker = new();

    private int gridWidthInCells;
    private int gridHeightInCells;

    private GridData Data;
    private OverlayGridPainter overlayPainter;

    public void SetOverlayPainter(OverlayGridPainter overlayPainter)
    {
        this.overlayPainter = overlayPainter;
    }

    public List<Vector2Int> GetTrackedCells()
    { 
        List<Vector2Int> completedTracker = new();
        completedTracker.AddRange(cellTracker);
        
        //cellTracker.Clear(); 
        return completedTracker;
    }

    public void SetSelected(int xIndex, int yIndex, bool selected)
    {
        selectedCells[xIndex, yIndex] = selected;
        overlayPainter.SetCellPainted(xIndex, yIndex, selected);
        UpdateTracker(new Vector2Int(xIndex, yIndex), selected);
    }

    private void UpdateTracker(Vector2Int point, bool selected)
    {
        if(selected) 
            cellTracker.Add(point);
        else
            cellTracker.Remove(point);
    }

    public void SetSecondaryTools(params IGridTool[] secondaryTools) {}


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
        foreach (var cell in cellTracker)
        {
            selectedCells[cell.x, cell.y] = value;
            overlayPainter.SetCellPainted(cell.x, cell.y, value);
        }
        cellTracker.Clear();
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
