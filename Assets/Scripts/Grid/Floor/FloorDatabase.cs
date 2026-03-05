using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FloorDatabase", menuName = "CityBuilder/FloorDatabase")]
public class FloorDatabase : ScriptableObject, IGridRelatedData
{
    public int AverageBuildingHeight = 2;
    public CellItem Dummy;
    public int ActiveFloorIndex { get; private set; } = 0;
    public Dictionary<int, FloorData> FloorDatas = new();
    public List<FloorData> FloorDatasCache = new();

    public void RestoreCacheIfNeeded()
    {
        //hard restore
        if (GetFloorCount() == 0)
        {
            foreach (var cachedFloorData in FloorDatasCache)
            {
                RestoreFloorData(cachedFloorData);
            }
        }
    }

    public FloorData GetActiveFloorData()
    {
        RestoreCacheIfNeeded();
        return FloorDatas[ActiveFloorIndex];
    } 
    
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
#if UNITY_EDITOR        
        Undo.RecordObject(this, "Add Floor");
        FloorDatas.Add(index, floorData);
        FloorDatasCache.Add(floorData);
        EditorUtility.SetDirty(this);
#endif
    }

    public void RestoreFloorData(FloorData floorData)
    {
        FloorDatas.Add(floorData.Index, floorData);
    }

    public void RemoveFloorData(FloorData floorData)
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Remove Floor");
        FloorDatas.Remove(floorData.Index);
        FloorDatasCache.Remove(floorData);
        EditorUtility.SetDirty(this);
#endif
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