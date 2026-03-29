using System.Collections.Generic;
using UnityEngine;

public class CellData
{
    public Vector2Int CellIndex;
    public CellType Type;
    public List<CellData> Neighbors = new();

    public CellData(Vector2Int cellIndex)
    {
        CellIndex = cellIndex;
    }

    private void SetType()
    {
        Type = Neighbors.Count switch
        {
            1 => CellType.Boundary,
            _ => CellType.Regular
        };
    }

    public void SetNeighbors(int cellUnit, Dictionary<Vector2Int, CellData> cellDataDict)
    {
        Vector2Int[] pendingNeighbors =  new Vector2Int[4];
       
        pendingNeighbors[0] = CellIndex + Vector2Int.right * cellUnit;
        pendingNeighbors[1] = CellIndex + Vector2Int.up * cellUnit;
        pendingNeighbors[2] = CellIndex + Vector2Int.left * cellUnit;
        pendingNeighbors[3] = CellIndex + Vector2Int.down * cellUnit;


        foreach (Vector2Int pendingNeighbor in pendingNeighbors)
        {
            if(cellDataDict.TryGetValue(pendingNeighbor, out var cellData))
            {
                Neighbors.Add(cellData);
            }
        }
        
        SetType();
    }
    
    
    public enum CellType
    {
        Regular,
        Boundary,
    }
    
}