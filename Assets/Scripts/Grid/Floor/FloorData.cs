using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FloorIdentifier
{
    public int Index;
    public Transform Root;
}

[System.Serializable]
public class FloorData
{
    public FloorIdentifier FloorIdentifier = new FloorIdentifier();
    public List<Vector2Int> OccupiedCells = new();

    public float GetFloorHeight(int averageBuildingHeight) => FloorIdentifier.Index * averageBuildingHeight;

    public HashSet<Vector2Int> GetCells() => OccupiedCells.ToHashSet();
    public FloorData(int index, Transform root, int averageBuildingHeight)
    {
        //set dirty, also floorcells
        FloorIdentifier.Index = index;
        FloorIdentifier.Root = root;

        ImplementFloorHeight(averageBuildingHeight);
    }

    public void ImplementFloorHeight(int averageBuildingHeight)
    {
        var pos = FloorIdentifier.Root.localPosition;
        pos.y = GetFloorHeight(averageBuildingHeight);
        FloorIdentifier.Root.localPosition = pos;
    }

    public void AddCell(Vector2Int cell)
    {
        if (!OccupiedCells.Contains(cell)) 
            OccupiedCells.Add(cell);
    }


    public void RemoveFromCell(Vector2Int cell, Structure item)
    {
        if (OccupiedCells.Contains(cell))
            OccupiedCells.Remove(cell);
    }

    public void ClearCells()
    {
        OccupiedCells.Clear();
    }

}