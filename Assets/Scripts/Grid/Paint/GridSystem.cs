using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserMapData
{
    [Header("Floor Settings")]
    public int AverageFloorHeight = 2;
    
    [Header("Grid Settings")]
    public int BuildingCellSize = 2;
    public bool UseMapSizeForGridSize = true;
    public Vector2Int ProjectedGridSize = new(100, 50);
}

public class GridSystem : MonoBehaviour
{
    [SerializeField] private  UserMapData userMapData;
    [SerializeField] private  GridData gridData;
    [SerializeField] private  PaintData paintData;
    [SerializeField] private  ConstructionData constructionData;
    [SerializeField] private FloorData floorData;
    
    [SerializeField] private MapSizeToGridSize mapSizeToGridSize;
    private GridProjector _projector = new();
    private GridMasker masker = new();
    
    [SerializeField] private OverlayGridPainter overlayGridPainter;
    private PainterProjected _painterProjected = new();
    private PainterInGrid painter = new();

    private GridToConstruction contstructor = new();
    private FloorProtocoles floorProtocoles = new();
    
    private IGridRelatedData[] gridRelatedData;
    private IGridTool[] tools;
    private float groundHeight;
    
    private void Start()
    {
        AdaptGridSizeToUserCellSize();
        
        floorProtocoles.Setup(floorData);
        UpdateGroundByFloor();
        
        SetTools(); 
        overlayGridPainter.Setup(gridData); //todo add to tools
        StartCoroutine(_painterProjected.PaintRoutine());
    }


    private void AdaptGridSizeToUserCellSize()
    {
        if(userMapData.UseMapSizeForGridSize)
            userMapData.ProjectedGridSize = mapSizeToGridSize.GetToGridSizeFromMesh();
        
        userMapData.ProjectedGridSize.x = Mathf.RoundToInt(userMapData.ProjectedGridSize.x / userMapData.BuildingCellSize);
        userMapData.ProjectedGridSize.y = Mathf.RoundToInt(userMapData.ProjectedGridSize.y / userMapData.BuildingCellSize);
        
        gridData.AdaptiveGridSize =  userMapData.ProjectedGridSize;
        gridData.CellSize = userMapData.BuildingCellSize;

    }

    private void SetTools()
    {
        tools = new IGridTool[] { painter, _projector, masker, _painterProjected, contstructor};
        masker.SetOverlayPainter(overlayGridPainter);
        contstructor.SetFloorProtocoles(floorProtocoles);
        InjectSecondaryTools();
        DistributeData();
    }

    public void ConstructBuildingsOnCells()
    {
        var trackedCells = masker.GetTrackedCells();
        if (trackedCells.Count == 0)
        {
            print("No tracked cells found");
            return;
        }
        
        contstructor.ConstructBuildingsOnCells(trackedCells, constructionData, groundHeight);
        masker.RestoreSelectedCells();

    }

    public void DestroyBuildingsOnCells()
    {
        contstructor.DeconstructBuildingsOnCells();
    }

    private void UpdateGroundByFloor()
    {
        groundHeight = userMapData.AverageFloorHeight * floorProtocoles.WorkingFloor;
        overlayGridPainter.SetHeight(groundHeight);
    }
    
    public void IncreaseFloor()
    {
        floorProtocoles.IncreaseFloorSet();
        UpdateGroundByFloor();
    }

    public void SwitchFloor(string charCount)
    {
       floorProtocoles.SwitchWorkingFloor(charCount.Length);
       UpdateGroundByFloor();
    }

    private void InjectSecondaryTools()
    {
        painter.SetSecondaryTools(_projector, masker);
        _painterProjected.SetSecondaryTools(_projector, painter);
    }
    
    public void DistributeData()
    {
        gridRelatedData = new IGridRelatedData[] { gridData, paintData };

        foreach (var tool in tools)
        {
            tool.SetGridRelatedData(gridRelatedData);
        }
    }
}
