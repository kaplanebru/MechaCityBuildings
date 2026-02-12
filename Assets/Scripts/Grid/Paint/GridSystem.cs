using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GridSystem : MonoBehaviour
{
    [SerializeField] private UserPreferences userPreferences;
    [SerializeField] private GridData gridData;
    [SerializeField] private PaintData paintData;
    [SerializeField] private FloorCacheData floorCacheData;
    [SerializeField] private CamShifter camShifter;

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

    private void Awake()
    {
        Configurations.SetData(userPreferences);
    }

    private void Start()
    {
        AdaptGridSizeToUserCellSize();

        _floorProtocols.Setup(floorCacheData);
        
        camShifter.Initialize();
        OnFloorUpdate();

        SetTools();
        overlayGridPainter.Setup(gridData); //todo add to tools
        StartCoroutine(_painterProjected.PaintRoutine());
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
        contstructor.ConstructBuildingsOnCells(floorData, registeredCells);

        if (TryDeconstructInvisibleIntersections(floorData, out var intersectingBuildings))
        {
            contstructor.DeconstructBuildings(intersectingBuildings.ToList());
        }
    }

    private bool TryDeconstructInvisibleIntersections(FloorData activeFloorData,
        out HashSet<Transform> intersectingBuildings)
    {
        intersectingBuildings = null;
        if (_floorProtocols.db.TryGetLowerFloorData(activeFloorData.Index, out var lowerFloorData))
        {
            intersectingBuildings = FloorIntersectionMasker.GetIntersectionsUnderFloor(activeFloorData, lowerFloorData);
            return true;
        }

        return false;
    }

    public void DestroyBuildingsOnCells()
    {
        contstructor.DeconstructBuildingsOnCells(_floorProtocols.db.GetActiveFloorData());
    }

    public void IncreaseFloor()
    {
        _floorProtocols.IncreaseFloorSet();
        OnFloorUpdate();
    }

    public void DeleteLastFloor()
    {
        if (_floorProtocols.TryDeleteLastFloor(out var newActiveFloor))
        {
            contstructor.RestoreBuildingsOnFloor(newActiveFloor);
            OnFloorUpdate();
        }
    }

    public void SwitchFloor(string charCount)
    {
        _floorProtocols.SwitchWorkingFloor(int.Parse(charCount));
        OnFloorUpdate();
    }

    private void OnFloorUpdate()
    {
        float floorRelativeHeight = userPreferences.AverageFloorHeight * _floorProtocols.db.ActiveFloorIndex;
        overlayGridPainter.SetHeight(floorRelativeHeight);
        camShifter.AlignRelativeHeightByFloor(floorRelativeHeight);
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