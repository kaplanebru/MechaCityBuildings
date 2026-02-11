using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridToConstruction: IGridTool
{
    public GridData Data { get; private set; }

    public void SetGridRelatedData(IGridRelatedData[] gridRelatedData)
    {
        Data = gridRelatedData[0] as GridData;
    }
    public void SetSecondaryTools(params IGridTool[] secondaryTools) {}
    
    public Vector3 GetCellIndexToWorldPositionCenter(int xIndex, int yIndex)
    {
        float worldX = Data.OriginWorldTransform.position.x + (xIndex + 0.5f) * Data.CellSize;
        float worldZ = Data.OriginWorldTransform.position.z + (yIndex + 0.5f) * Data.CellSize;


        float worldY = Data.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }

   
    public Vector3 CellIndexToWorldPositionCorner(int xIndex, int yIndex)
    {
        float worldX = Data.OriginWorldTransform.position.x + xIndex * Data.CellSize;
        float worldZ = Data.OriginWorldTransform.position.z + yIndex * Data.CellSize;
        float worldY = Data.OriginWorldTransform.position.y;

        return new Vector3(worldX, worldY, worldZ);
    }
    
   
    public void ConstructBuildingsOnCells(FloorData floorData, HashSet<Vector2Int> registeredCells)
    {
        if (registeredCells.Count == 0)
        {
            Debug.Log("No tracked cells found");
            return;
        }
        
        //registeredCells = BoundaryFinder.GetBoundsWithInner(1, trackedCells);
        
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
            Data.OriginWorldTransform.rotation,
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

    public void DeconstructBuildingsOnCells(FloorData floorData)
    {
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

