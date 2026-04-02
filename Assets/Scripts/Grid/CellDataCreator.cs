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

        FindOrientations(cellDataDict.Values.ToHashSet());

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
    
    private static void FindOrientations(HashSet<CellData> cellDataSet)
    {
        var boundaryCells = CellRegistry.GetBoundaries(cellDataSet);

        foreach (var boundaryCell in boundaryCells)
        {
            if (boundaryCell.OutwardNormal == Vector2Int.zero)
            {
                continue;
            }
            
            Vector2Int tangent = new Vector2Int(
                -boundaryCell.OutwardNormal.y, 
                boundaryCell.OutwardNormal.x); //perpendicular
            
            Vector3 forward = new Vector3(tangent.x, 0f, tangent.y);
            boundaryCell.Rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
    }

    
}