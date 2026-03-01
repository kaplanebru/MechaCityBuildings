using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FloorDatabase", menuName = "CityBuilder/FloorDatabase")]
public class FloorDatabase : ScriptableObject, IGridRelatedData
{
    public int ActiveFloorIndex { get; private set; } = 0;
    private Dictionary<int, FloorData> FloorDatas = new();
    
    public List<FloorData> FloorDatasCache = new();

    //todo her floor butonuna basıldığında restore de
    public FloorData GetActiveFloorData() => FloorDatas[ActiveFloorIndex];
    
    public int GetFloorCount() => FloorDatas.Count;
    
    public event Action<FloorData> OnActiveFloorUpdate;
    public event Action<FloorData> OnDeleteLastFloor;
    public void SetActiveFloor(int index)
    {
        ActiveFloorIndex = index;
        OnActiveFloorUpdate?.Invoke(FloorDatas[ActiveFloorIndex]);
    }

    public void InvokeDeleteLastFloor(FloorData newActiveFloor)
    {
        OnDeleteLastFloor?.Invoke(newActiveFloor);
    }

    public void AddFloorData(int index, FloorData floorData)
    {
        FloorDatas.Add(index, floorData);
        FloorDatasCache.Add(floorData);
    }

    public void RestoreFloorData(FloorData floorData)
    {
        FloorDatas.Add(floorData.Index, floorData);
    }

    public void RemoveFloorData(FloorData floorData)
    {
        FloorDatas.Remove(floorData.Index);
        FloorDatasCache.Remove(floorData);
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