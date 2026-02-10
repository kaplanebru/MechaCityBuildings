using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FloorCacheData
{
    public List<Transform> CachedFloorRoots;
    public Transform Root;
}

public class FloorDatabase
{
    public int ActiveFloorIndex { get; private set; }= 0;
    public Dictionary<int, FloorData> FloorDatas = new();
    
    public FloorData GetActiveFloorData() => FloorDatas[ActiveFloorIndex];
    public void SetActiveFloor(int index) => ActiveFloorIndex = index;

}

public class FloorData
{
    public int Index;
    public Transform Root;
    public Dictionary<Vector2Int, Transform> ItemsByCell = new();
    public FloorData(int index, Transform root)
    {
        Index = index;
        Root = root;
    }
}