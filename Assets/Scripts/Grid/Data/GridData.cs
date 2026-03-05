using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Data", menuName = "CityBuilder/Grid Data")]
public class GridData: ScriptableObject, IGridRelatedData
{
    public int BuildingCellSize = 2;

    
    [HideInInspector] public Transform OriginWorldTransform;
    [HideInInspector] public Vector2Int AdaptiveGridSize;
    [HideInInspector] public List<Vector2Int> CellRecorderCache = new(); //kaydedilmesi lazım

    public void AddToCellRecordCache(Vector2Int cell)
    {
        if(!CellRecorderCache.Contains(cell))
            CellRecorderCache.Add(cell);
    }

    public void RemoveFromCellRecordCache(Vector2Int cell)
    {
        if(CellRecorderCache.Contains(cell))
            CellRecorderCache.Remove(cell);
    }
}


public interface IGridRelatedData {}