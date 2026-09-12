using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;
#endif

using UnityEngine;

public static class FloorManagement
{
    public static void DebugFM(FloorDatabase db)
    {
        Debug.Log("floor amount: " + db.GetFloorCount());
        Debug.Log("current floor: " + db.ActiveFloorIndex);

        Reset(db);
        
        Debug.Log("floor amount: " + db.GetFloorCount());
        Debug.Log("current floor: " + db.ActiveFloorIndex);
    }

    private static void RestoreCacheIfNeeded(FloorDatabase db)
    {
        HardRestore(db);
    }
    
    private static void AddFloorData(FloorData floorData, FloorDatabase db)
    {
#if UNITY_EDITOR
        Undo.RecordObject(db, "Add Floor");
        db.FloorDatas.Add(floorData);
        EditorUtility.SetDirty(db);
#endif
    }

    private static void RemoveFloorData(FloorData floorData, FloorDatabase db)
    {
#if UNITY_EDITOR
        if (db.FloorDatas.Contains(floorData))
        {
            Undo.RecordObject(db, "Remove Floor");
            db.FloorDatas.Remove(floorData);
            EditorUtility.SetDirty(db);
        }
#endif
    }

    private static void CreateFloor(int floorIndex, FloorDatabase db)
    {
        var newFloor = new GameObject("Floor " + db.GetFloorCount()).transform;
        newFloor.SetParent(db.floorsRoot);

        var floorData = new FloorData(floorIndex, newFloor, db.gridData.AverageBuildingHeight);
        AddFloorData(floorData, db);
        db.SetActiveFloor(floorIndex);
        
        db.OnFloorCreated?.Invoke();
    }

    public static void IncreaseFloor(FloorDatabase db)
    {
        RestoreCacheIfNeeded(db);
        CreateFloor(db.ActiveFloorIndex + 1, db);
    }

    public static void ClearActiveFloor(FloorDatabase db)
    {
        RestoreCacheIfNeeded(db);
        var activeFloor = db.GetActiveFloorData();
        db.OnFloorClearRequest?.Invoke(activeFloor.Index);
    }

    private static void ClearLastFloor(FloorDatabase db)
    {
        var lastFloor = db.GetLastFloorData();
        var safeLastFloorMinus = lastFloor.Index > 0 ? lastFloor.Index-1 : 0;
        db.OnFloorClearRequest?.Invoke(safeLastFloorMinus);
    }

    public static void DeleteLastFloor(FloorDatabase db)
    {
        RestoreCacheIfNeeded(db);
        if (TryDeleteLastFloor(db, out var newActiveFloor))
        {
            //todo
        }
    }


    private static bool TryDeleteLastFloor(FloorDatabase db, out FloorData newActiveFloor)
    {
        RestoreCacheIfNeeded(db);
        newActiveFloor = null;
        
        ClearLastFloor(db);
        if (db.GetFloorCount() <= 1) return false;
        
        var lastFloor = db.GetLastFloorData();
        var lastRoot = lastFloor.Root;
        UnityEngine.Object.DestroyImmediate(lastRoot.gameObject);

        RemoveFloorData(lastFloor, db);
        db.SetActiveFloor(db.GetFloorCount() - 1);
        newActiveFloor = db.GetActiveFloorData();
        
        db.OnLastFloorRemoved?.Invoke();
        return true;
    }

    public static void DeleteEveryFloor(FloorDatabase db)
    {
        RestoreCacheIfNeeded(db);

        foreach (var floor in db.FloorDatas)
        {
            //todo
        }
    }

    public static void SwitchActiveFloor(int floorIndex, FloorDatabase db)
    {
        RestoreCacheIfNeeded(db);

        if (floorIndex >= db.GetFloorCount())
        {
            Debug.LogError("Floor index out of bounds");
            return;
        }

        db.SetActiveFloor(floorIndex);
    }

    public static void OnFloorHeightUpdate(FloorDatabase db)
    {
        foreach (var floorData in db.FloorDatas)
        {
            floorData.ImplementFloorHeight(db.gridData.AverageBuildingHeight);
        }
    }
    
    private static void Reset(FloorDatabase db)
    {
        db.FloorDatas.Clear();

        var floors = db.floorsRoot.GetComponentsInChildren<Transform>().
            Where(t => t != db.floorsRoot).ToArray();
        for (var i = floors.Length - 1; i >= 0; i--)
        {
            var floor = floors[i];
            UnityEngine.Object.DestroyImmediate(floor.gameObject);
        }

        db.SetActiveFloor(0);
    }

    public static void HardRestore(FloorDatabase db)
    {
        if (db.GetFloorCount() == 0)
        {
            if (db.floorsRoot.childCount == 0)
            {
                CreateFloor(0, db);
                return;
            }

            Debug.LogError("floor roots > floor data"
                           + " floor count: " + db.GetFloorCount()
                           + " root count: " + db.floorsRoot.childCount);
        }
        else
        {
            if (db.floorsRoot.childCount == 0)
            {
                Debug.LogError("floor data > floor roots"
                               + " floor count: " + db.GetFloorCount()
                               + " root count: " + db.floorsRoot.childCount);

                //restore'un pek bir anlamı yok çünkü kaydedilen cell'ler de uçmuş olur
            }
        }
    }
}