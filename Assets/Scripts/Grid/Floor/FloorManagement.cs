using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorManagement: MonoBehaviour //can be made native or static
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
    
  
    public void IncreaseFloor()
    {
        int newFloorIndex = db.ActiveFloorIndex + 1;
        var newFloor = new GameObject("Floor " + db.FloorDatas.Count).transform;
        newFloor.SetParent(cacheData.Root);
        
        db.FloorDatas.Add(newFloorIndex, new FloorData(newFloorIndex, newFloor));
        db.SetActiveFloor(newFloorIndex);

    }

    private void ClearActiveFloor()
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
            db.InvokeDeleteLastFloor(newActiveFloor);
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
        return true;
    }
    
    public void SwitchActiveFloor(int floorIndex)
    {
        if (floorIndex >= db.FloorDatas.Count)
        {
            Debug.LogError("Floor index out of bounds");
            return;
        }
        
        db.SetActiveFloor(floorIndex);
    }

    
}
