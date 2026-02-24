using System;
using System.Collections.Generic;
using UnityEngine;

public class FloorDatabase : IGridRelatedData
{
    public int ActiveFloorIndex { get; private set; } = 0;
    public Dictionary<int, FloorData> FloorDatas = new();

    public FloorData GetActiveFloorData() => FloorDatas[ActiveFloorIndex];
    
    public event Action<FloorData> OnActiveFloorUpdate;
    public void SetActiveFloor(int index)
    {
        ActiveFloorIndex = index;
        OnActiveFloorUpdate?.Invoke(FloorDatas[ActiveFloorIndex]);
    }

    public bool TryGetFloorData(int floorIndex) => FloorDatas.TryGetValue(floorIndex, out FloorData data);
    public bool TryGetLowerFloorData(int upperFloorIndex, out FloorData lowerFloorData)
    {
        lowerFloorData = null;
        if (upperFloorIndex <= 0) return false;
        
        int lowerFloorIndex = upperFloorIndex - 1;
        return FloorDatas.TryGetValue(lowerFloorIndex, out lowerFloorData);
    }
}
public interface IGridDatabase {}

public class FloorData
{
    public int Index;
    public Transform Root;
    public Dictionary<Vector2Int, Transform> ItemsByCell = new();
    public float FloorGroundHeight => Index * Configurations.UserPreferences.AverageFloorHeight;

    public FloorData(int index, Transform root)
    {
        Index = index;
        Root = root;
    }
}