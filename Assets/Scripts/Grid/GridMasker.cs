using System;

public class GridMasker: IGridTool
{
    private bool[,] selectedCells;
    private bool[,] occupiedCells;

    private int gridWidthInCells;
    private int gridHeightInCells;

    private GridData Data;
    private OverlayGridPainter overlayPainter;

    public void SetOverlayPainter(OverlayGridPainter overlayPainter)
    {
        this.overlayPainter = overlayPainter;
    }

    public void SetSelected(int xIndex, int yIndex, bool selected)
    {
        selectedCells[xIndex, yIndex] = selected;
        overlayPainter.SetCellPainted(xIndex, yIndex, selected);
    }
    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData)
    {
        Data = (GridData)gridRelatedData[0];
        
        gridWidthInCells = Data.GridSize.x;
        gridHeightInCells = Data.GridSize.y;
        
        if (gridWidthInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridWidthInCells));

        if (gridHeightInCells <= 0)
            throw new ArgumentOutOfRangeException(nameof(gridHeightInCells));
        
        selectedCells = new bool[gridWidthInCells, gridHeightInCells];
        occupiedCells = new bool[gridWidthInCells, gridHeightInCells];
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

}
