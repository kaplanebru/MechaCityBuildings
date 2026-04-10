using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class SlotData
{
    public Vector2Int[] Cells;
    public SlotType Type;
    public StructureType StructureType; 
    public List<NeighborCell> Neighbors = new();
    public Vector2 Center;
    
    public Vector2Int OutwardNormal = Vector2Int.zero;
    public Quaternion Rotation = Quaternion.Euler(Vector3.zero);//Quaternion.identity;
    public int OrderIndex;
    public Vector2Int SlotSize = new (1, 1);

    public SlotData(Vector2Int[] cells)
    {
        Cells = cells;
    }

    public StructureType GetStructureType() => StructureType;
    public void ApplyStructureType(StructureType type) => StructureType = type;
    public void SetOrderIndex(int orderIndex) => OrderIndex = orderIndex;

    private void SetType()
    {
        //4=çevresi kadar neighbor'u olur max

        Type = Neighbors.Count < 4
            ? SlotType.Boundary
            : SlotType.Regular;
    }

    /*public void SetNeighbors(
        int cellUnit,
        HashSet<Vector2Int> allCells)
    {
        Vector2Int[] pendingNeighbors = new Vector2Int[4];
        pendingNeighbors[0] = Cells + Vector2Int.right * cellUnit; //east
        pendingNeighbors[1] = Cells + Vector2Int.up * cellUnit; //north
        pendingNeighbors[2] = Cells + Vector2Int.left * cellUnit; //west
        pendingNeighbors[3] = Cells + Vector2Int.down * cellUnit; //south


        for (var i = 0; i < pendingNeighbors.Length; i++)
        {
            var pendingNeighbor = pendingNeighbors[i];

            if(allCells.Contains(pendingNeighbor))
            {
                Neighbors.Add(new NeighborCell(pendingNeighbor));
            }
            else
            {
                OutwardNormal += GetNormalByDirection(i);
            }
        }

        SetType();
    }*/

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

public enum SlotType
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