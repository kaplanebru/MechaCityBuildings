using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FloorManagement : MonoBehaviour //can be made native or static
{
    public GridData gridData;
    public FloorDatabase db;
    public Transform floorsRoot;

    public void DebugFM()
    {
        print("floor amount: " + db.GetFloorCount());
        print("current floor: " + db.ActiveFloorIndex);
        
        Reset();
    }

    private void RestoreCacheIfNeeded()
    {
        HardRestore();
    }

    private void Reset()
    {
        db.FloorDatas.Clear();

        var floors = floorsRoot.GetComponentsInChildren<Transform>().
            Where(t => t != floorsRoot).ToArray();
        for (var i = floors.Length - 1; i >= 0; i--)
        {
            var floor = floors[i];
            DestroyImmediate(floor.gameObject);
        }

        db.SetActiveFloor(0);
    }

    public void HardRestore()
    {
        if (floorsRoot.childCount == 0 && db.GetFloorCount() > 0)
        {
            Debug.LogError("floor data > floor roots" 
                           + " floor count: " + db.GetFloorCount() 
                           + " root count: " + floorsRoot.childCount);
            
            //restore'un pek bir anlamı yok çünkü kaydedilen cell'ler de uçmuş olur
        }

        if (db.GetFloorCount() == 0)
        {
            CreateFloor(0);
        }
        
    }

    private void AddFloorData(FloorData floorData)
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Add Floor");
        db.FloorDatas.Add(floorData);
        EditorUtility.SetDirty(this);
#endif
    }
    
    private void RemoveFloorData(FloorData floorData)
    {
#if UNITY_EDITOR
        if (db.FloorDatas.Contains(floorData))
        {
            Undo.RecordObject(this, "Remove Floor");
            db.FloorDatas.Remove(floorData);
            EditorUtility.SetDirty(this);
        }
#endif
    }

    private void CreateFloor(int floorIndex)
    {
        var newFloor = new GameObject("Floor " + db.GetFloorCount()).transform;
        newFloor.SetParent(floorsRoot);
        //zaten add floor'da set dirty yapılıyor

        AddFloorData(new FloorData(floorIndex, newFloor, gridData.AverageBuildingHeight));
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
        DestroyImmediate(activeRoot.gameObject);

        RemoveFloorData(activeFloor);
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

    public void OnFloorHeightUpdate()
    {
        foreach (var floorData in db.FloorDatas)
        {
            floorData.ImplementFloorHeight(gridData.AverageBuildingHeight);
        }
    }
}