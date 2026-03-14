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
    public List<CellItem> CellItems = new();

    private Dictionary<Vector2Int, CellItem> ItemsByCell = new();
    public float GetFloorHeight(int averageBuildingHeight) => Index * averageBuildingHeight;

    public List<CellItem> GetCellItems() => CellItems;

    public CellItem GetItemByCell(Vector2Int cell) =>
        CellItems.FirstOrDefault(i => i.cellMetadata == cell); //ItemsByCell[cell];

    public FloorData(int index, Transform root, int averageBuildingHeight)
    {
        //set dirty, also floorcells
        Index = index;
        Root = root;

        //ItemsByCell = new();
        //FloorCells = new();

        ImplementFloorHeight(averageBuildingHeight);
    }

    public void ImplementFloorHeight(int averageBuildingHeight)
    {
        var pos = Root.localPosition;
        pos.y = GetFloorHeight(averageBuildingHeight);
        Root.localPosition = pos;
    }

    public void SetFloorCells(List<Vector2Int> cells)
    {
        OccupiedCells.Clear();
        OccupiedCells.AddRange(cells);
    }

    public Dictionary<Vector2Int, CellItem> GetItemsByCell()
    {
        ItemsByCell ??= new();

        if (ItemsByCell.Count == 0)
            ItemsByCell = CellItems.ToDictionary(i => i.cellMetadata, i => i);

        return ItemsByCell;
    }

    public void AddItemToCell(Vector2Int cell, CellItem item)
    {
        if (OccupiedCells.Contains(cell)) return;


        item.SetCellMetaData(cell);
        CellItems.Add(item);
        OccupiedCells.Add(cell);


        /*if (ItemsByCell.TryAdd(cell, item))
        {
            Cells.Add(cell);
        }*/
    }


    public void RemoveItemFromCell(Vector2Int cell, CellItem item)
    {
        if (!OccupiedCells.Contains(cell)) return;
        item.ResetCellMetaData();
        CellItems.Remove(item);
        OccupiedCells.Remove(cell);

        /*if (ItemsByCell.ContainsKey(cell))
        {
            Cells.Remove(cell);
            ItemsByCell.Remove(cell);
        }*/
    }

    public void ClearCells()
    {
        OccupiedCells.Clear();
        CellItems.Clear();

        //ItemsByCell.Clear();
    }

    public bool HasItemOnCell(Vector2Int cell, out CellItem item)
    {
        item = null;
        if (OccupiedCells.Contains(cell))
        {
            item = GetItemByCell(cell);
            return true;
        }
        return false;
        //return ItemsByCell[cell] != null;
    }

    public void SetItemOnCell(Vector2Int cell, CellItem item)
    {
        AddItemToCell(cell, item);
        //ItemsByCell[cell] = item;
    }
}