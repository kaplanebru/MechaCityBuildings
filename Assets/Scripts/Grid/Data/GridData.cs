using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Grid Data", menuName = "CityBuilder/Grid Data")]
public class GridData: ScriptableObject, IGridRelatedData
{
    [HideInInspector] public Transform OriginWorldTransform;
    [HideInInspector] public float CellSize;
    [HideInInspector] public Vector2Int AdaptiveGridSize;
    
    [HideInInspector] public List<Vector2Int> cellRecorderCache = new();

    public void AddToCellRecordCache(Vector2Int cell)
    {
        if(!cellRecorderCache.Contains(cell))
            cellRecorderCache.Add(cell);
    }

    public void RemoveFromCellRecordCache(Vector2Int cell)
    {
        if(cellRecorderCache.Contains(cell))
            cellRecorderCache.Remove(cell);
    }
}


public interface IGridRelatedData {}