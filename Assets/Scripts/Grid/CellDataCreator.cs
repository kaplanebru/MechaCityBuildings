using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellDataCreator
{
    public static HashSet<CellData> ConvertToCellData(HashSet<Vector2Int> cellRecorderCache, int cellUnit)
    {
        var cellDataDict = InitiateCellDatas(cellRecorderCache);

        foreach (var cellData in cellDataDict.Values)
        {
            cellData.SetNeighbors(cellUnit, cellDataDict);
        }

        return cellDataDict.Values.ToHashSet();
    }

    private static Dictionary<Vector2Int, CellData> InitiateCellDatas(HashSet<Vector2Int> cellRecorderCache)
    {
        Dictionary<Vector2Int, CellData> cellDataDict = new();
        foreach (Vector2Int cellIndex in cellRecorderCache)
        {
            cellDataDict.Add(cellIndex, new CellData(cellIndex));
        }
        
        return cellDataDict;
    }

    public static HashSet<CellData> GetBoundaries(HashSet<CellData> cellDataSet)
    {
        return cellDataSet.Where(cellData => cellData.Type == CellData.CellType.Boundary).ToHashSet();
    }
}