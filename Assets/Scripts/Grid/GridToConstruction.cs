using System.Collections.Generic;
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
    
   
    List<Transform> constructedDummies = new List<Transform>();
    public void ConstructBuildingsOnCells(List<Vector2Int> trackedCells, ConstructionData data, float groundHeight)
    {
        if (trackedCells.Count == 0)
        {
            Debug.Log("No tracked cells found");
            return;
        }
        
        trackedCells = BoundaryFinder.GetBoundsWithInner(1, trackedCells);
        foreach (var trackedCell in trackedCells)
        {
            Vector3 pos = GetCellIndexToWorldPositionCenter(trackedCell.x, trackedCell.y);
            pos.y += groundHeight;
            var dummyInstance = Object.Instantiate(data.Dummy, pos, Data.OriginWorldTransform.rotation);
            dummyInstance.transform.SetParent(data.BuildingsRoot);
            constructedDummies.Add(dummyInstance);
        }
    }

    public void DeconstructBuildingsOnCells()
    {
        if (constructedDummies.Count == 0)
        {
            Debug.Log("No constructed dummies found");
            return;
        }
        foreach (var dummy in constructedDummies)
        {
            Object.Destroy(dummy.gameObject);
        }
        constructedDummies.Clear();
    }
}

[System.Serializable]
public class ConstructionData
{
    public Transform BuildingsRoot;
    public Transform Dummy;
}