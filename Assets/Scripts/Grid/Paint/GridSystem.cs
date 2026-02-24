using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GridDataType
{
    GridData,
    PaintData,
    FloorDatabase
}

public class GridSystem : MonoBehaviour
{
    [SerializeField] private UserPreferences userPreferences;
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    [SerializeField] private FloorManagement floorManagement;

    [SerializeField] private MapSizeToGridSize mapSizeToGridSize;
    private GridProjector _projector = new();
    private GridMasker masker = new();

    [SerializeField] private OverlayGridPainter overlayGridPainter;
    private PainterProjected _painterProjected = new();
    private PainterInGrid painter = new();

    private GridToConstruction contstructor = new();

    private Dictionary<GridDataType, IGridRelatedData> gridRelatedData = new();
    private IGridTool[] tools;

    private void Awake()
    {
        Configurations.SetData(userPreferences);
    }

    private void Start()
    {
        AdaptGridSizeToUserCellSize();
        SetTools();
        SubscribeTools();
        StartCoroutine(_painterProjected.PaintRoutine());
    }

    private void OnDisable()
    {
        UnsubscribeTools();
    }

    private void AdaptGridSizeToUserCellSize()
    {
        if (userPreferences.UseMapSizeForGridSize)
            userPreferences.ProjectedGridSize = mapSizeToGridSize.GetToGridSizeFromMesh();

        userPreferences.ProjectedGridSize.x =
            Mathf.RoundToInt(userPreferences.ProjectedGridSize.x / userPreferences.BuildingCellSize);
        userPreferences.ProjectedGridSize.y =
            Mathf.RoundToInt(userPreferences.ProjectedGridSize.y / userPreferences.BuildingCellSize);

        gridData.AdaptiveGridSize = userPreferences.ProjectedGridSize;
        gridData.CellSize = userPreferences.BuildingCellSize;
    }

    private void SetTools()
    {
        tools = new IGridTool[] { painter, _projector, masker, _painterProjected, contstructor };
        overlayGridPainter.Setup(gridData, floorManagement.db); //todo add to tools

        masker.SetOverlayPainter(overlayGridPainter);
        InjectSecondaryTools();
        DistributeData();
    }

    private void InjectSecondaryTools()
    {
        painter.SetSecondaryTools(_projector, masker);
        _painterProjected.SetSecondaryTools(_projector, painter);
    }

    public void DistributeData()
    {
       // gridRelatedData = new IGridRelatedData[] { gridData, paintData, floorManagement.db};
        gridRelatedData.Clear();
        gridRelatedData.Add(GridDataType.GridData, gridData);
        gridRelatedData.Add(GridDataType.PaintData, paintData);
        gridRelatedData.Add(GridDataType.FloorDatabase, floorManagement.db);

        foreach (var tool in tools)
        {
            tool.SetGridRelatedData(gridRelatedData);
        }
    }

    private void SubscribeTools()
    {
        foreach (var tool in tools)
        {
            tool.Subscribe();
        }
    }

    private void UnsubscribeTools()
    {
        foreach (var tool in tools)
        {
            tool.Unsubscribe();
        }
    }
    
    public void ConstructBuildingsOnCells()
    {
        var registeredCells = masker.RegisterTrackedCells();
        contstructor.Construct(registeredCells);
    }

    public void DestroyBuildingsOnCells()
    {
        contstructor.DeconstructBuildingsOnCells();
    }
}