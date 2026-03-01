using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorData
{
    //[HideInInspector]
    public int Index;
    public Transform Root;
    public List<Vector2Int> FloorCells = new ();
    private Dictionary<Vector2Int, Transform> ItemsByCell = new();
    public float FloorGroundHeight => Index * Configurations.UserPreferences.AverageBuildingHeight;

    public Transform GetItemByCell(Vector2Int cell) => ItemsByCell[cell];

    public FloorData(int index, Transform root)
    {
        Index = index;
        Root = root;
    }

    public void SetFloorCells(List<Vector2Int> cells)
    {
        FloorCells.Clear();
        FloorCells.AddRange(cells);
    }

    public Dictionary<Vector2Int, Transform> GetTotalItemsByCell()
    {
        if (ItemsByCell.Count == 0)
        {
            //TODO: Restore
        }
        return ItemsByCell;
    }

    public void AddItemToCell(Vector2Int cell, Transform item)
    {
        if (ItemsByCell.TryAdd(cell, item))
        {
            FloorCells.Add(cell);
        }
    }

    public void RemoveItemFromCell(Vector2Int cell)
    {
        if (ItemsByCell.ContainsKey(cell))
        {
            FloorCells.Remove(cell);
            ItemsByCell.Remove(cell);
        }
    }

    public void ClearCells()
    {
        FloorCells.Clear();
        ItemsByCell.Clear();
    }

    public bool HasItemOnCell(Vector2Int cell)
    {
        return ItemsByCell[cell] != null;
    }

    public void SetItemOnCell(Vector2Int cell, Transform item)
    {
        ItemsByCell[cell] = item;
    }

    
}