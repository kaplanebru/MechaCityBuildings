using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorManagement: MonoBehaviour
{
    public FloorDatabase db = new FloorDatabase();
    [SerializeField] private FloorCacheData cacheData;

    private void OnEnable()
    {
        Setup();
    }

    public void Setup()
    {
        db.FloorDatas.Clear();
        for (var i = 0; i < cacheData.CachedFloorRoots.Count; i++)
        {
            db.FloorDatas.Add(i, new FloorData(i, cacheData.CachedFloorRoots[i]));
        }
        
        db.SetActiveFloor(0);
    }
    
    public void SwitchWorkingFloor(int floorIndex)
    {
        if (floorIndex >= db.FloorDatas.Count)
        {
            Debug.LogError("Floor index out of bounds");
            return;
        }
        
        db.SetActiveFloor(floorIndex);
    }

    public void IncreaseFloor()
    {
        db.SetActiveFloor(db.ActiveFloorIndex+1);
        
        var newFloor = new GameObject("Floor " + db.FloorDatas.Count).transform;
        newFloor.SetParent(cacheData.Root);
        
        db.FloorDatas.Add(db.ActiveFloorIndex, new FloorData(db.ActiveFloorIndex, newFloor));
    }

    public void ClearActiveFloor()
    {
        if(db.FloorDatas.Count <= 1) return;

        var activeFloor = db.GetActiveFloorData();
        if(activeFloor.ItemsByCell.Count == 0) return;

        var items = activeFloor.ItemsByCell.Values.ToHashSet();
        foreach (var item in items)
        {
            Destroy(item.gameObject);
            //Undo.DestroyObjectImmediate(activeRoot);
        }
        activeFloor.ItemsByCell.Clear();
    }
    public void DeleteLastFloor()
    {
        if (TryDeleteLastFloor(out var newActiveFloor))
        {
            //todo: send event: contstructor.RestoreBuildingsOnFloor(newActiveFloor);
            //OnFloorUpdate();
        }
    }


    private bool TryDeleteLastFloor(out FloorData newActiveFloor)
    {
        newActiveFloor = null;
        if (db.FloorDatas.Count <= 1) return false;

        var activeFloor = db.GetActiveFloorData();
        
        ClearActiveFloor();
        var activeRoot = activeFloor.Root;
        Destroy(activeRoot.gameObject);

        db.FloorDatas.Remove(activeFloor.Index);
        db.SetActiveFloor(db.FloorDatas.Last().Value.Index);
        
        newActiveFloor = db.GetActiveFloorData();
        //TODO: contructordan bunu çağırma
        return true;
    }
    
}
