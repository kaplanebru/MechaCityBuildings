using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public static class GridToConstruction
{
    //private FloorDatabase floorDb;
    //private FloorManagement floorManagement;

   /* public void Subscribe()
    {
        floorDb.OnDeleteLastFloor += RestoreBuildingsOnFloor;

    }

    public void Unsubscribe()
    {
        floorDb.OnDeleteLastFloor -= RestoreBuildingsOnFloor;
    }*/
    
    public static Vector3 GetCellIndexToWorldPositionCenter(int xIndex, int yIndex, GridData gridData)
    {
        float worldX = gridData.OriginWorldTransform.position.x + (xIndex + 0.5f) * gridData.BuildingCellSize;
        float worldZ = gridData.OriginWorldTransform.position.z + (yIndex + 0.5f) * gridData.BuildingCellSize;


        float worldY = gridData.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }

   
    public static Vector3 CellIndexToWorldPositionCorner(int xIndex, int yIndex, GridData gridData)
    {
        float worldX = gridData.OriginWorldTransform.position.x + xIndex * gridData.BuildingCellSize;
        float worldZ = gridData.OriginWorldTransform.position.z + yIndex * gridData.BuildingCellSize;
        float worldY = gridData.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }
    
    public static void Construct(HashSet<Vector2Int> registeredCells, GridData gridData, FloorDatabase floorDb)
    {
        if (registeredCells.Count == 0)
        {
            Debug.Log("No tracked cells found on Floor");
            return;
        }

        var floorData = floorDb.GetActiveFloorData();
        ConstructBuildingsOnCells(floorData, registeredCells, gridData, floorDb.Dummy);

        if (TryDeconstructInvisibleIntersections(floorData, floorDb, out var intersectingBuildings))
        {
            DeconstructBuildings(intersectingBuildings.ToList());
        }
    }
    
    private static bool TryDeconstructInvisibleIntersections(
        FloorData activeFloorData, 
        FloorDatabase floorDb,
        out HashSet<CellItem> intersectingBuildings)
    {
        intersectingBuildings = null;
        if (floorDb.TryGetLowerFloorData(activeFloorData.Index, out var lowerFloorData))
        {
            intersectingBuildings = FloorIntersectionMasker.GetIntersectionsUnderFloor(activeFloorData, lowerFloorData);
            return true;
        }

        return false;
    }
   
    public static void ConstructBuildingsOnCells(FloorData floorData, HashSet<Vector2Int> registeredCells, GridData gridData, CellItem cellItem)
    {
        if (registeredCells.Count == 0)
        {
            Debug.Log("No tracked cells found");
            return;
        }
        foreach (var cell in registeredCells)
        {
            var dummyInstance = ConstructItem(cell, cellItem, floorData, gridData);
            floorData.AddItemToCell(cell, dummyInstance);
        }
    }

    private static CellItem ConstructItem(Vector2Int cell, CellItem dummy, FloorData floorData, GridData gridData)
    {
        Vector3 pos = GetCellIndexToWorldPositionCenter(cell.x, cell.y, gridData);
        pos.y += floorData.GetFloorHeight(gridData.AverageBuildingHeight);

        var dummyInstance = Object.Instantiate(
            dummy, floorData.Root);
        
        dummyInstance.transform.position = pos;
        dummyInstance.transform.rotation = gridData.OriginWorldTransform.rotation;
        
        return dummyInstance;
    }

   public static void DeconstructBuildings(List<CellItem> buildings)
   {
       for (int i = buildings.Count - 1; i >= 0; i--)
       {
           Object.DestroyImmediate(buildings[i].gameObject);
       }
   }

    public static void DeconstructBuildingsOnCells(FloorDatabase floorDb)
    {
        var floorData = floorDb.GetActiveFloorData();
        var items = floorData.GetItemsByCell().Values.ToHashSet();
        if (items.Count == 0)
        {
            Debug.Log("No constructed dummies found");
            return;
        }
       
        foreach (var dummy in items)
        {
            Object.DestroyImmediate(dummy.gameObject);
        }
        floorData.ClearCells();
    }

    public static void RestoreBuildingsOnFloor(FloorData floorData,GridData gridData)
    {
        HashSet<Vector2Int> keys = floorData.OccupiedCells.ToHashSet();
        foreach (var key in keys)
        {
            if (floorData.HasItemOnCell(key, out var item)) continue;
            
            //floorData.SetItemOnCell(key, ConstructItem(key, floorData, gridData));
        }
    }
}

