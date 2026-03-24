using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[Serializable]
public class FloorDatabase: MonoBehaviour
{ 
    public int ActiveFloorIndex { get; private set; } = 0;

    [SerializeField]
    internal List<FloorData> FloorDatas = new();

    public GridData gridData;
    public Transform floorsRoot;
    
    public Action OnFloorCreated;
    public Action OnLastFloorRemoved;
    public Action<int> OnFloorClearRequest;
    
    public FloorData GetActiveFloorData(Action restoreIfNeededCallback = null)
    {
        restoreIfNeededCallback?.Invoke();
        return FloorDatas[ActiveFloorIndex];
    }

    public FloorData GetLastFloorData(Action restoreIfNeededCallback = null)
    {
        restoreIfNeededCallback?.Invoke();
        return FloorDatas[^1];
    }
    
    public FloorData GetFloorData(int floorIndex) => FloorDatas[floorIndex];

    public int GetFloorCount()
    {
        FloorDatas ??= new List<FloorData>();
        return FloorDatas.Count;
    }

    public event Action<FloorData> OnActiveFloorUpdate;

    public void SetActiveFloor(int index)
    {
        ActiveFloorIndex = index;

        if (FloorDatas.Count <= 0) return;

        OnActiveFloorUpdate?.Invoke(FloorDatas[ActiveFloorIndex]);
        Debug.Log("active floor: " + index);
    }

    public bool TryGetFloorData(int floorIndex, out FloorData floorData)
    {
        floorData = null;
        if (floorIndex < 0 || floorIndex >= FloorDatas.Count) return false;

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