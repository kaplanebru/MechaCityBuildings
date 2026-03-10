using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class FloorDatabase : IGridRelatedData
{
    public CellItem Dummy;
    public int ActiveFloorIndex { get; private set; } = 0;
    [SerializeField] internal List<FloorData> FloorDatas = new(); //public Dictionary<int, FloorData> FloorDatas = new();
    

    public FloorData GetActiveFloorData(Action restoreIfNeededCallback = null)
    {
       // restoreIfNeededCallback();
        return FloorDatas[ActiveFloorIndex];
    } 
    
    public int GetFloorCount() => FloorDatas.Count;
    
    public event Action<FloorData> OnActiveFloorUpdate;
    public event Action<FloorData> OnDeleteLastFloor;
    public void SetActiveFloor(int index)
    {
        ActiveFloorIndex = index;
        
        if(FloorDatas.Count <= 0) return;
        
        OnActiveFloorUpdate?.Invoke(FloorDatas[ActiveFloorIndex]);
        Debug.Log("active floor: " +index);
    }

    public void InvokeDeleteLastFloor(FloorData newActiveFloor)
    {
        OnDeleteLastFloor?.Invoke(newActiveFloor);
    }
    
    public bool TryGetFloorData(int floorIndex, out FloorData floorData)
    {
        floorData = null;
        if(floorIndex < 0 || floorIndex >= FloorDatas.Count) return false;

        floorData = FloorDatas[floorIndex];
        return true;
    }

    public bool TryGetLowerFloorData(int upperFloorIndex, out FloorData lowerFloorData)
    {
        lowerFloorData = null;
        if (upperFloorIndex <= 0) return false;
        
        int lowerFloorIndex = upperFloorIndex - 1;
        return TryGetFloorData(lowerFloorIndex, out lowerFloorData);
    }
}
