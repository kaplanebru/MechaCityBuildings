using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FloorManagement : MonoBehaviour //can be made native or static
{
    public GridData gridData;
    public Transform floorsRoot;
    public FloorDatabase db;
    
    public Action<FloorData> OnFloorCreated;
    public Action OnLastFloorRemoved;
    public Action<int> OnFloorClearRequest;

    public void DebugFM()
    {
        print("floor amount: " + db.GetFloorCount());
        print("current floor: " + db.ActiveFloorIndex);

        Reset();
        
        print("floor amount: " + db.GetFloorCount());
        print("current floor: " + db.ActiveFloorIndex);
    }

    private void RestoreCacheIfNeeded()
    {
        HardRestore();
    }
    
    private void AddFloorData(FloorData floorData)
    {
#if UNITY_EDITOR
        Undo.RecordObject(this, "Add Floor");
        db.FloorDatas.Add(floorData);
        EditorUtility.SetDirty(this);
        
        //OnFloorCreated?.Invoke(floorData);
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

        var floorData = new FloorData(floorIndex, newFloor, gridData.AverageBuildingHeight);
        AddFloorData(floorData);
        db.SetActiveFloor(floorIndex);
        
        OnFloorCreated?.Invoke(floorData);
    }

    public void IncreaseFloor()
    {
        RestoreCacheIfNeeded();
        CreateFloor(db.ActiveFloorIndex + 1);
    }

    public void ClearActiveFloor()
    {
        RestoreCacheIfNeeded();
        var activeFloor = db.GetActiveFloorData();
        activeFloor.ClearCells();
        
        OnFloorClearRequest?.Invoke(activeFloor.FloorIdentifier.Index);
      
    }

    public void DeleteLastFloor()
    {
        RestoreCacheIfNeeded();
        if (TryDeleteLastFloor(out var newActiveFloor))
        {
            //todo
        }
    }


    private bool TryDeleteLastFloor(out FloorData newActiveFloor)
    {
        RestoreCacheIfNeeded();
        newActiveFloor = null;
        if (db.GetFloorCount() <= 1) return false;

        var activeFloor = db.GetActiveFloorData();

        ClearActiveFloor();
        
        var activeRoot = activeFloor.FloorIdentifier.Root;
        DestroyImmediate(activeRoot.gameObject);

        RemoveFloorData(activeFloor);
        db.SetActiveFloor(db.GetFloorCount() - 1); //db.FloorDatas.Last().Value.Index

        newActiveFloor = db.GetActiveFloorData();
        
        OnLastFloorRemoved?.Invoke();
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
    
    private void Reset()
    {
        db.FloorDatas.Clear();

        var floors = floorsRoot.GetComponentsInChildren<Transform>().Where(t => t != floorsRoot).ToArray();
        for (var i = floors.Length - 1; i >= 0; i--)
        {
            var floor = floors[i];
            DestroyImmediate(floor.gameObject);
        }

        db.SetActiveFloor(0);
    }

    public void HardRestore()
    {
        
        if (db.GetFloorCount() == 0)
        {
            if (floorsRoot.childCount == 0)
            {
                CreateFloor(0);
                return;
            }

            Debug.LogError("floor roots > floor data"
                           + " floor count: " + db.GetFloorCount()
                           + " root count: " + floorsRoot.childCount);
        }
        else
        {
            if (floorsRoot.childCount == 0)
            {
                Debug.LogError("floor data > floor roots"
                               + " floor count: " + db.GetFloorCount()
                               + " root count: " + floorsRoot.childCount);

                //restore'un pek bir anlamı yok çünkü kaydedilen cell'ler de uçmuş olur
            }
        }
    }
}