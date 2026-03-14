using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class FloorData
{
    //[HideInInspector]
    public int Index;
    public Transform Root;

    public List<Vector2Int> OccupiedCells = new();
    public List<CellItem> Items = new();
    private Dictionary<Vector2Int, CellItem> ItemsByCell = new();
    public float GetFloorHeight(int averageBuildingHeight) => Index * averageBuildingHeight;


    public CellItem GetItemByCell(Vector2Int cell)
    {
        RestoreItemsByCellIfNeeded();
        return ItemsByCell[cell];
    }

    public FloorData(int index, Transform root, int averageBuildingHeight)
    {
        //set dirty, also floorcells
        Index = index;
        Root = root;

        ImplementFloorHeight(averageBuildingHeight);
    }

    public void ImplementFloorHeight(int averageBuildingHeight)
    {
        var pos = Root.localPosition;
        pos.y = GetFloorHeight(averageBuildingHeight);
        Root.localPosition = pos;
    }

    public Dictionary<Vector2Int, CellItem> GetItemsByCell()
    {
        RestoreItemsByCellIfNeeded();
        return ItemsByCell;
    }

    private void RestoreItemsByCellIfNeeded()
    {
        ItemsByCell ??= new();

        if (ItemsByCell.Count == 0)
            ItemsByCell = Items.ToDictionary(i => i.cellMetadata, i => i);
    }

    public void AddItemToCell(Vector2Int cell, CellItem item)
    {
        RestoreItemsByCellIfNeeded();

        if (ItemsByCell.TryAdd(cell, item))
        {
            Items.Add(item);
            OccupiedCells.Add(cell);
            
            item.SetCellMetaData(cell);
        }
    }


    public void RemoveItemFromCell(Vector2Int cell, CellItem item)
    {
        RestoreItemsByCellIfNeeded();
        
        if (ItemsByCell.ContainsKey(cell))
        {
            ItemsByCell.Remove(cell);
            item.ResetCellMetaData();
            Items.Remove(item);
            OccupiedCells.Remove(cell);
        }
    }

    public void ClearCells()
    {
        OccupiedCells.Clear();
        Items.Clear();
        
        RestoreItemsByCellIfNeeded();
        ItemsByCell.Clear();
    }

    public bool HasItemOnCell(Vector2Int cell, out CellItem item)
    {
        RestoreItemsByCellIfNeeded();
        item = ItemsByCell[cell];
        return ItemsByCell[cell] != null;
        
        /*item = null;
        if (OccupiedCells.Contains(cell))
        {
            item = GetItemByCell(cell);
            return true;
        }
        return false;*/
    }

    public void SetItemOnCell(Vector2Int cell, CellItem item)
    {
        AddItemToCell(cell, item);
        //ItemsByCell[cell] = item;
    }
}