using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserMapData
{
    [Header("Floor Settings")] public int AverageFloorHeight = 2;

    [Header("Grid Settings")] public int BuildingCellSize = 2;
    public bool UseMapSizeForGridSize = true;
    public Vector2Int ProjectedGridSize = new(100, 50);
}

public class GridSystem : MonoBehaviour
{
    [SerializeField] private UserMapData userMapData;
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    [SerializeField] private ConstructionData constructionData;
    [SerializeField] private FloorCacheData floorCacheData;

    [SerializeField] private MapSizeToGridSize mapSizeToGridSize;
    private GridProjector _projector = new();
    private GridMasker masker = new();

    [SerializeField] private OverlayGridPainter overlayGridPainter;
    private PainterProjected _painterProjected = new();
    private PainterInGrid painter = new();

    private GridToConstruction contstructor = new();
    private FloorProtocols _floorProtocols = new();

    private IGridRelatedData[] gridRelatedData;
    private IGridTool[] tools;
    private float drawingGroundHeight;

    private void Start()
    {
        AdaptGridSizeToUserCellSize();

        _floorProtocols.Setup(floorCacheData);
        UpdateDrawingGroundByFloor();

        SetTools();
        overlayGridPainter.Setup(gridData); //todo add to tools
        StartCoroutine(_painterProjected.PaintRoutine());
    }


    private void AdaptGridSizeToUserCellSize()
    {
        if (userMapData.UseMapSizeForGridSize)
            userMapData.ProjectedGridSize = mapSizeToGridSize.GetToGridSizeFromMesh();

        userMapData.ProjectedGridSize.x =
            Mathf.RoundToInt(userMapData.ProjectedGridSize.x / userMapData.BuildingCellSize);
        userMapData.ProjectedGridSize.y =
            Mathf.RoundToInt(userMapData.ProjectedGridSize.y / userMapData.BuildingCellSize);

        gridData.AdaptiveGridSize = userMapData.ProjectedGridSize;
        gridData.CellSize = userMapData.BuildingCellSize;
    }

    private void SetTools()
    {
        tools = new IGridTool[] { painter, _projector, masker, _painterProjected, contstructor };
        masker.SetOverlayPainter(overlayGridPainter);
        InjectSecondaryTools();
        DistributeData();
    }

    public void ConstructBuildingsOnCells()
    {
        var registeredCells = masker.RegisterTrackedCells();
        if (registeredCells.Count == 0)
        {
            print("No tracked cells found on Floor");
            return;
        }

        var floorData = _floorProtocols.db.GetActiveFloorData();
        contstructor.ConstructBuildingsOnCells(floorData, registeredCells, constructionData, drawingGroundHeight);
        
        if (TryDeconstructInvisibleIntersections(floorData, out var intersections,out var lowerFloor))
        {
            contstructor.DeconstructBuildingsOnGivenCells(intersections, lowerFloor);
        }
    }
    
    private bool TryDeconstructInvisibleIntersections(FloorData activeFloorData,
        out HashSet<Vector2Int> intersections,
        out FloorData lowerFloor)
    {
        intersections = null;
        lowerFloor = null;
        if(activeFloorData.Index <= 0) return false;
        
        lowerFloor = _floorProtocols.db.GetFloorData(activeFloorData.Index-1);

        if (lowerFloor == null)
        {
            Debug.Log("No floor with that Index:  " + activeFloorData.Index);
            return false;
        }

        intersections = FloorIntersectionMasker.GetIntersectionsUnderFloor(activeFloorData, lowerFloor);
        return true;
    }

    public void DestroyBuildingsOnCells()
    {
        contstructor.DeconstructBuildingsOnCells(_floorProtocols.db.GetActiveFloorData());
    }

    private void UpdateDrawingGroundByFloor()
    {
        drawingGroundHeight = userMapData.AverageFloorHeight * _floorProtocols.db.ActiveFloorIndex;
        overlayGridPainter.SetHeight(drawingGroundHeight);
    }

    public void IncreaseFloor()
    {
        _floorProtocols.IncreaseFloorSet();
        OnFloorUpdate();
    }

    public void SwitchFloor(string charCount)
    {
        _floorProtocols.SwitchWorkingFloor(int.Parse(charCount));
        OnFloorUpdate();
    }

    //todo: floorDistributor bağlantı classı yap: floor implementer/publisher
    private void OnFloorUpdate()
    {
        UpdateDrawingGroundByFloor();
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