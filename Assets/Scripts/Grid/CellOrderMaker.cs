using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellOrderMaker
{
    //kaç cell olduğunu bilmeden nasıl frequency belirleyebiliriz?


    public void FindOrientations(HashSet<CellData> cellDataSet)
    {
        var boundaryCells = CellRegistry.GetBoundaries(cellDataSet);
        
    }
    
    public void OrderCells(HashSet<CellData> cellDataSet)
    {
        var cityShape = DetectShape(cellDataSet.ToList());
    }

    private CityShape DetectShape(List<CellData> cellDataSet)
    {
        CellData minXCell = cellDataSet[0];
        CellData maxXCell = cellDataSet[0];
        CellData minYCell = cellDataSet[0];
        CellData maxYCell = cellDataSet[0];

        for (int i = 1; i < cellDataSet.Count; i++)
        {
            CellData currentCell = cellDataSet[i];

            if (currentCell.CellIndex.x < minXCell.CellIndex.x)
                minXCell = currentCell;

            if (currentCell.CellIndex.x > maxXCell.CellIndex.x)
                maxXCell = currentCell;

            if (currentCell.CellIndex.y < minYCell.CellIndex.y)
                minYCell = currentCell;

            if (currentCell.CellIndex.y > maxYCell.CellIndex.y)
                maxYCell = currentCell;
        }
        
        var xDistance = maxXCell.CellIndex.x - minXCell.CellIndex.x ;
        var yDistance = maxYCell.CellIndex.y - minYCell.CellIndex.y;

        return xDistance > yDistance ? CityShape.Horizontal : CityShape.Vertical;
    }
}


public enum CityShape
{
    Vertical,
    Horizontal
}