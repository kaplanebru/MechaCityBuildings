using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridToConstruction: IGridTool
{
    public GridData GridData { get; private set; }
    private FloorDatabase floorDb;
    private FloorManagement floorManagement;

    public void Subscribe()
    {
        floorDb.OnDeleteLastFloor += RestoreBuildingsOnFloor;

    }

    public void Unsubscribe()
    {
        floorDb.OnDeleteLastFloor -= RestoreBuildingsOnFloor;
    }

    public void SetGridRelatedData(Dictionary<GridDataType, IGridRelatedData> gridRelatedData)
    {
        GridData = gridRelatedData[GridDataType.GridData] as GridData;
        floorDb = gridRelatedData[GridDataType.FloorDatabase] as FloorDatabase;

    }
    public void SetSecondaryTools(params IGridTool[] secondaryTools) {}
    
    public Vector3 GetCellIndexToWorldPositionCenter(int xIndex, int yIndex)
    {
        float worldX = GridData.OriginWorldTransform.position.x + (xIndex + 0.5f) * GridData.CellSize;
        float worldZ = GridData.OriginWorldTransform.position.z + (yIndex + 0.5f) * GridData.CellSize;


        float worldY = GridData.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }

   
    public Vector3 CellIndexToWorldPositionCorner(int xIndex, int yIndex)
    {
        float worldX = GridData.OriginWorldTransform.position.x + xIndex * GridData.CellSize;
        float worldZ = GridData.OriginWorldTransform.position.z + yIndex * GridData.CellSize;
        float worldY = GridData.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }
    
    public void Construct(HashSet<Vector2Int> registeredCells)
    {
        if (registeredCells.Count == 0)
        {
            Debug.Log("No tracked cells found on Floor");
            return;
        }

        var floorData = floorDb.GetActiveFloorData();
        ConstructBuildingsOnCells(floorData, registeredCells);

        if (TryDeconstructInvisibleIntersections(floorData, out var intersectingBuildings))
        {
            DeconstructBuildings(intersectingBuildings.ToList());
        }
    }
    
    private bool TryDeconstructInvisibleIntersections(FloorData activeFloorData, out HashSet<Transform> intersectingBuildings)
    {
        intersectingBuildings = null;
        if (floorDb.TryGetLowerFloorData(activeFloorData.Index, out var lowerFloorData))
        {
            intersectingBuildings = FloorIntersectionMasker.GetIntersectionsUnderFloor(activeFloorData, lowerFloorData);
            return true;
        }

        return false;
    }
   
    public void ConstructBuildingsOnCells(FloorData floorData, HashSet<Vector2Int> registeredCells)
    {
        if (registeredCells.Count == 0)
        {
            Debug.Log("No tracked cells found");
            return;
        }
        foreach (var cell in registeredCells)
        {
            var dummyInstance = ConstructItem(cell, floorData);
            floorData.ItemsByCell.TryAdd(cell, dummyInstance);
        }
    }

    private Transform ConstructItem(Vector2Int cell, FloorData floorData)
    {
        Vector3 pos = GetCellIndexToWorldPositionCenter(cell.x, cell.y);
        pos.y += floorData.FloorGroundHeight;
            
        var dummyInstance = Object.Instantiate(
            Configurations.UserPreferences.Dummy,
            pos,
            GridData.OriginWorldTransform.rotation,
            floorData.Root);
        
        return dummyInstance;
    }

   public void DeconstructBuildings(List<Transform> buildings)
   {
       for (int i = buildings.Count - 1; i >= 0; i--)
       {
           Object.Destroy(buildings[i].gameObject);
       }
   }

    public void DeconstructBuildingsOnCells()
    {
        var floorData = floorDb.GetActiveFloorData();
        if (floorData.ItemsByCell.Count == 0)
        {
            Debug.Log("No constructed dummies found");
            return;
        }
        foreach (var dummy in floorData.ItemsByCell.Values)
        {
            Object.Destroy(dummy.gameObject);
        }
        floorData.ItemsByCell.Clear();
    }

    public void RestoreBuildingsOnFloor(FloorData floorData)
    {
        HashSet<Vector2Int> keys = floorData.ItemsByCell.Keys.ToHashSet();
        foreach (var key in keys)
        {
            if (floorData.ItemsByCell[key] != null) continue;
            
            floorData.ItemsByCell[key] = ConstructItem(key, floorData);
        }
    }
}

