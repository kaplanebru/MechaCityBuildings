using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserMapData
{
    public int BuildingCellSize = 2;
    public bool UseMapSizeForGridSize = true;
    public Vector2Int ProjectedGridSize = new(100, 50);
}

public class GridSystem : MonoBehaviour
{
    public UserMapData userMapData;
    public GridData gridData;
    public PaintData paintData;
    public ConstructionData constructionData;
    [SerializeField] private OverlayGridPainter overlayGridPainter;
    [SerializeField] private MapSizeToGridSize mapSizeToGridSize;
    
    private IGridRelatedData[] gridRelatedData;
    private IGridTool[] tools;
    
    private PainterInGrid painter = new();
    private GridProjector _projector = new();
    private GridMasker masker = new();
    private PainterProjected _painterProjected = new();
    private GridToConstruction contstructor = new();
    
    private void Start()
    {
        AdaptGridSizeToUserCellSize();
        SetTools(); 
        overlayGridPainter.Setup(gridData);
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
        
        contstructor.ConstructBuildingsOnCells(trackedCells, constructionData);
    }

    public void DestroyBuildingsOnCells()
    {
        masker.RestoreSelectedCells();
        contstructor.DeconstructBuildingsOnCells();
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
