using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public static class CellConverter
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

   public static Vector3 GetWorldPositionFromCellCenter(SlotData slotData, GridData gridData)
   {
       float worldX = gridData.OriginWorldTransform.position.x + slotData.Center.x * gridData.MinBuildingCellSize;
       float worldZ = gridData.OriginWorldTransform.position.z + slotData.Center.y * gridData.MinBuildingCellSize;
       
       float worldY = gridData.OriginWorldTransform.position.y;
       return new Vector3(worldX, worldY, worldZ);
   }
    
    public static Vector3 GetWorldPositionCenterFromCellIndex(int xIndex, int yIndex, GridData gridData)
    {
        float worldX = gridData.OriginWorldTransform.position.x + (xIndex + 0.5f) * gridData.MinBuildingCellSize; //(xIndex + 0.5f)
        float worldZ = gridData.OriginWorldTransform.position.z + (yIndex + 0.5f) * gridData.MinBuildingCellSize; //(yIndex + 0.5f)

        float worldY = gridData.OriginWorldTransform.position.y;
        return new Vector3(worldX, worldY, worldZ);
    }

    public static Vector2Int GetCellIndexFromWorldPosition(Vector3 worldPosition, GridData gridData)
    {
        int xIndex = Mathf.FloorToInt((worldPosition.x - gridData.OriginWorldTransform.position.x) / gridData.MinBuildingCellSize);
        int yIndex = Mathf.FloorToInt((worldPosition.z - gridData.OriginWorldTransform.position.z) / gridData.MinBuildingCellSize);

        return new Vector2Int(xIndex, yIndex);
    }
    
    public static void Construct(HashSet<Vector2Int> registeredCells, GridData gridData, FloorDatabase floorDb)
    {
       

       /* var floorData = floorDb.GetActiveFloorData();
        ConstructBuildingsOnCells(floorData, registeredCells, gridData, floorDb.Dummy);

        if (TryDeconstructInvisibleIntersections(floorData, floorDb, out var intersectingBuildings))
        {
            DeconstructBuildings(intersectingBuildings.ToList());
        }*/
    }
    
   
    

    /*private static Structure ConstructItem(Vector2Int cell, Structure dummy, FloorData floorData, GridData gridData)
    {
        Vector3 pos = GetWorldPositionCenterFromCellIndex(cell.x, cell.y, gridData);
        pos.y += floorData.GetFloorHeight(gridData.AverageBuildingHeight);

        var dummyInstance = Object.Instantiate(
            dummy, floorData.FloorIdentifier.Root);
        
        dummyInstance.transform.position = pos;
        dummyInstance.transform.rotation = gridData.OriginWorldTransform.rotation;
        
        return dummyInstance;
    }*/

    
    
}

