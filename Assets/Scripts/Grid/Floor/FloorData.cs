using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorData
{
    //[HideInInspector]
    public int Index;
    public Transform Root;
    public List<Vector2Int> FloorCells;
    public int AverageBuildingHeight = 2;
    private Dictionary<Vector2Int, CellItem> ItemsByCell;
    public float FloorGroundHeight(int averageBuildingHeight) => Index * averageBuildingHeight;

    public CellItem GetItemByCell(Vector2Int cell) => ItemsByCell[cell];

    public FloorData(int index, Transform root, int averageBuildingHeight)
    {
        //set dirty, also floorcells
        Index = index;
        Root = root;
        ItemsByCell = new();
        FloorCells = new();
    }

    public void SetFloorCells(List<Vector2Int> cells)
    {
        FloorCells.Clear();
        FloorCells.AddRange(cells);
    }

    public Dictionary<Vector2Int, CellItem> GetTotalItemsByCell()
    {
        if (ItemsByCell.Count == 0)
        {
            //TODO: Restore
        }

        return ItemsByCell;
    }

    private void RestoreItemsByCellIfNeeded()
    {
        if(ItemsByCell != null)
        {
            if(ItemsByCell.Count == FloorCells.Count)
                return;
            
            ItemsByCell = null;
        }
        ItemsByCell = new Dictionary<Vector2Int, CellItem>();
       /* foreach (var cell in FloorCells)
        {
            //TODO: ItemsByCell.Add(cell, );
        }*/
    }

    public void AddItemToCell(Vector2Int cell, CellItem item)
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

    public void SetItemOnCell(Vector2Int cell, CellItem item)
    {
        ItemsByCell[cell] = item;
    }
}