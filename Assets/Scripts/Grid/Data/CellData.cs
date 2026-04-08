using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class CellData
{
    public Vector2Int CellIndex;
    public CellType Type;
    public List<Vector2Int> Neighbors = new();
    public Vector2Int OutwardNormal = Vector2Int.zero;
    public Quaternion Rotation = Quaternion.Euler(Vector3.zero);//Quaternion.identity;
    
    public StructureType StructureType; 
    public int OrderIndex;
    public Vector2 Center;
    public Vector2Int CellSize = new (1, 1);

    public CellData(Vector2Int cellIndex)
    {
        CellIndex = cellIndex;
        Center = new Vector2(CellIndex.x + 0.5f,
            CellIndex.y + 0.5f); //CellIndex + Vector2Int.one/2; //todo: for oonly 1-1
    }

    public StructureType GetStructureType() => StructureType;
    public void ApplyStructureType(StructureType type) => StructureType = type;
    public void SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;

    private void SetType()
    {
        //4=çevresi kadar neighbor'u olur max

        Type = Neighbors.Count < 4
            ? CellType.Boundary
            : CellType.Regular;
    }
    
    /*private void SetType()
    {
        Type = Neighbors.Count switch
        {
            1 => CellType.Boundary,
            _ => CellType.Regular
        };
    }*/

    public void SetNeighbors(
        int cellUnit,
        Dictionary<Vector2Int, CellData> cellDataDict)
    {
        Vector2Int[] pendingNeighbors = new Vector2Int[4];
        pendingNeighbors[0] = CellIndex + Vector2Int.right * cellUnit; //east
        pendingNeighbors[1] = CellIndex + Vector2Int.up * cellUnit; //north
        pendingNeighbors[2] = CellIndex + Vector2Int.left * cellUnit; //west
        pendingNeighbors[3] = CellIndex + Vector2Int.down * cellUnit; //south


        for (var i = 0; i < pendingNeighbors.Length; i++)
        {
            var pendingNeighbor = pendingNeighbors[i];

            if (cellDataDict.TryGetValue(pendingNeighbor, out var cellData))
            {
                Neighbors.Add(cellData.CellIndex);
            }
            else
            {
                OutwardNormal += GetNormalByDirection(i);
            }
        }

        SetType();
    }

    private Vector2Int GetNormalByDirection(int i)
    {
        return i switch
        {
            0 => new(1, 0) //east
            ,
            1 => new(0, 1) //north
            ,
            2 => new(-1, 0) //west
            ,
            3 => new(0, -1) //south
            ,
            _ => Vector2Int.zero
        };
    }
}

public enum CellType
{
    Regular,
    Boundary,
}

public enum DirectionType
{
    East,
    North,
    West,
    South
}