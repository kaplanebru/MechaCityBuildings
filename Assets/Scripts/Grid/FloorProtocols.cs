using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloorProtocols
{
    public FloorDatabase db = new FloorDatabase();
    private FloorCacheData cacheData;
    
    public void Setup(FloorCacheData data)
    {
        cacheData = data;
        
        db.FloorDatas.Clear();
        for (var i = 0; i < cacheData.CachedFloorRoots.Count; i++)
        {
            db.FloorDatas.Add(i, new FloorData(i, cacheData.CachedFloorRoots[i]));
        }
    }
    
    public FloorData SwitchWorkingFloor(int floorIndex)
    {
        if (floorIndex >= db.FloorDatas.Count)
        {
            Debug.LogError("Floor index out of bounds");
            return null;
        }
        
        db.SetActiveFloor(floorIndex);
        return db.GetActiveFloorData();
    }

    public FloorData IncreaseFloorSet()
    {
        db.SetActiveFloor(db.ActiveFloorIndex+1);
        
        var newFloor = new GameObject("Floor " + db.FloorDatas.Count).transform;
        newFloor.SetParent(cacheData.Root);
        
        db.FloorDatas.Add(db.ActiveFloorIndex, new FloorData(db.ActiveFloorIndex, newFloor));
        return db.GetActiveFloorData();
    }

    public void ClearActiveFloor()
    {
        if(db.FloorDatas.Count <= 1) return;

        var activeFloor = db.GetActiveFloorData();
            //todo: check henüz null olabilir itemlar cell dolu olsa bile
        if(activeFloor.ItemsByCell.Count == 0) return;

        var items = activeFloor.ItemsByCell.Values.ToHashSet();
        foreach (var item in items)
        {
            Object.Destroy(item.gameObject);
            //Undo.DestroyObjectImmediate(activeRoot);
        }
        activeFloor.ItemsByCell.Clear();
    }

    public bool TryDeleteLastFloor(out FloorData newActiveFloor)
    {
        newActiveFloor = null;
        if (db.FloorDatas.Count <= 1) return false;

        var activeFloor = db.GetActiveFloorData();
        
        ClearActiveFloor();
        var activeRoot = activeFloor.Root;
        Object.Destroy(activeRoot.gameObject);

        db.FloorDatas.Remove(activeFloor.Index);
        db.SetActiveFloor(db.FloorDatas.Last().Value.Index);
        
        newActiveFloor = db.GetActiveFloorData();
        //TODO: contructordan bunu çağırma
        return true;
    }
    
}
