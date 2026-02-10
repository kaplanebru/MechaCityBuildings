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
        }
        activeFloor.ItemsByCell.Clear();
        
        /*var root = cachedFloorRoots[ActiveFloorIndex];
        if (root.childCount == 0) return;

        for (int i = root.childCount - 1; i >= 0; i--)
        {
            var child = root.GetChild(i).gameObject;
            Object.Destroy(child);
            //Undo.DestroyObjectImmediate(child);
        }*/
    }

    public void DeleteLastFloor()
    {
        if (db.FloorDatas.Count == 1) return;

        var activeFloor = db.GetActiveFloorData();
        
        ClearActiveFloor();
        var activeRoot = activeFloor.Root;
        Object.Destroy(activeRoot.gameObject);
        //todo: bunu destroy edince aslında altındakiler de destroyed olur mu?
        //Undo.DestroyObjectImmediate(activeRoot);

        db.FloorDatas.Remove(activeFloor.Index);
        
        if(db.ActiveFloorIndex == db.FloorDatas.Count - 1)
            db.SetActiveFloor(0);
    }
    
}
