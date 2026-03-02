using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorManagement : MonoBehaviour //can be made native or static
{
    public FloorDatabase db;
    public Transform floorsRoot;

    public void DebugFM()
    {
        db.FloorDatasCache.Clear();
        db.FloorDatas.Clear();
        HardRestore();
        db.RestoreCacheIfNeeded();
        print("floor amount: " + db.GetFloorCount());
        print("current floor: " + db.ActiveFloorIndex);
    }

    private void RestoreCacheIfNeeded()
    {
        HardRestore();
        db.RestoreCacheIfNeeded();
    }

    public void HardRestore()
    {
        if (floorsRoot.GetComponentsInChildren<Transform>().Length == 0)
        {
            db.FloorDatasCache.Clear();
        }

        if (db.FloorDatasCache.Count == 0)
            CreateFloor(0);
    }


private void CreateFloor(int floorIndex)
{
    var newFloor = new GameObject("Floor " + db.GetFloorCount()).transform;
    newFloor.SetParent(floorsRoot);

    db.AddFloorData(floorIndex, new FloorData(floorIndex, newFloor));
    db.SetActiveFloor(floorIndex);
}

public void IncreaseFloor()
{
    RestoreCacheIfNeeded();
    CreateFloor(db.ActiveFloorIndex + 1);
}

public void ClearActiveFloor()
{
    //if(db.FloorDatas.Count <= 1) return;

    RestoreCacheIfNeeded();
    var activeFloor = db.GetActiveFloorData();
    if (activeFloor.GetTotalItemsByCell().Count == 0) return;

    var items = activeFloor.GetTotalItemsByCell().Values.ToHashSet();
    foreach (var item in items)
    {
        DestroyImmediate(item.gameObject);
        //Undo.DestroyObjectImmediate(activeRoot);
    }

    activeFloor.GetTotalItemsByCell().Clear();
}

public void DeleteLastFloor()
{
    RestoreCacheIfNeeded();
    if (TryDeleteLastFloor(out var newActiveFloor))
    {
        db.InvokeDeleteLastFloor(newActiveFloor);
    }
}


private bool TryDeleteLastFloor(out FloorData newActiveFloor)
{
    RestoreCacheIfNeeded();
    newActiveFloor = null;
    if (db.GetFloorCount() <= 1) return false;

    var activeFloor = db.GetActiveFloorData();

    ClearActiveFloor();
    var activeRoot = activeFloor.Root;
    Destroy(activeRoot.gameObject);

    db.RemoveFloorData(activeFloor);
    db.SetActiveFloor(db.GetFloorCount() - 1); //db.FloorDatas.Last().Value.Index

    newActiveFloor = db.GetActiveFloorData();
    return true;
}

public void SwitchActiveFloor(int floorIndex)
{
    RestoreCacheIfNeeded();
    if (floorIndex >= db.GetFloorCount())
    {
        Debug.LogError("Floor index out of bounds");
        return;
    }

    db.SetActiveFloor(floorIndex);
}
}