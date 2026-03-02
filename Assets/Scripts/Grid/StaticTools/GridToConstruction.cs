using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        float worldX = gridData.OriginWorldTransform.position.x + (xIndex + 0.5f) * gridData.CellSize;
        float worldZ = gridData.OriginWorldTransform.position.z + (yIndex + 0.5f) * gridData.CellSize;


        float worldY = gridData.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }

   
    public static Vector3 CellIndexToWorldPositionCorner(int xIndex, int yIndex, GridData gridData)
    {
        float worldX = gridData.OriginWorldTransform.position.x + xIndex * gridData.CellSize;
        float worldZ = gridData.OriginWorldTransform.position.z + yIndex * gridData.CellSize;
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
        ConstructBuildingsOnCells(floorData, registeredCells, gridData);

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
   
    public static void ConstructBuildingsOnCells(FloorData floorData, HashSet<Vector2Int> registeredCells, GridData gridData)
    {
        if (registeredCells.Count == 0)
        {
            Debug.Log("No tracked cells found");
            return;
        }
        foreach (var cell in registeredCells)
        {
            var dummyInstance = ConstructItem(cell, floorData, gridData);
            floorData.AddItemToCell(cell, dummyInstance);
        }
    }

    private static CellItem ConstructItem(Vector2Int cell, FloorData floorData, GridData gridData)
    {
        Vector3 pos = GetCellIndexToWorldPositionCenter(cell.x, cell.y, gridData);
        pos.y += floorData.FloorGroundHeight;
            
        var dummyInstance = Object.Instantiate(
            Configurations.UserPreferences.Dummy,
            pos,
            gridData.OriginWorldTransform.rotation,
            floorData.Root);
        
        return dummyInstance;
    }

   public static void DeconstructBuildings(List<CellItem> buildings)
   {
       for (int i = buildings.Count - 1; i >= 0; i--)
       {
           Object.Destroy(buildings[i].gameObject);
       }
   }

    public static void DeconstructBuildingsOnCells(FloorDatabase floorDb)
    {
        var floorData = floorDb.GetActiveFloorData();
        if (floorData.GetTotalItemsByCell().Count == 0)
        {
            Debug.Log("No constructed dummies found");
            return;
        }
        foreach (var dummy in floorData.GetTotalItemsByCell().Values)
        {
            Object.Destroy(dummy.gameObject);
        }
        floorData.ClearCells();
    }

    public static void RestoreBuildingsOnFloor(FloorData floorData,GridData gridData)
    {
        HashSet<Vector2Int> keys = floorData.GetTotalItemsByCell().Keys.ToHashSet();
        foreach (var key in keys)
        {
            if (floorData.HasItemOnCell(key)) continue;
            
            floorData.SetItemOnCell(key, ConstructItem(key, floorData, gridData));
        }
    }
}

