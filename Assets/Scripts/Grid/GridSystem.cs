using UnityEngine;


public class GridSystem : MonoBehaviour
{
    [SerializeField] private UserPreferences userPreferences;
    public GridData gridData;
    [SerializeField] private MapSizeToGridSize mapSizeToGridSize;
    public OverlayPainter overlayPainter;

    public void Recalculate()
    {
        AdaptGridSizeToUserCellSize();
        overlayPainter.CreateOverlayMeshIfNeeded(gridData); //todo: cache
        GridMasker.SetGridWithinCells(gridData); 
        //todo: hard reset if needed: yani normal reset gibi gidip tek tek bulup silmeyecek,
        //loop ile her celli dolaşıp silecek hem masktan hem overlayden
        
    }

    private void AdaptGridSizeToUserCellSize()
    {
        if (Configurations.UserPreferences == null) //reload can be tracked from here
            Configurations.SetData(userPreferences);

        if (userPreferences.UseMapSizeForGridSize)
            userPreferences.ProjectedGridSize = mapSizeToGridSize.GetGridSizeFromMesh();

        userPreferences.ProjectedGridSize.x =
            Mathf.RoundToInt(userPreferences.ProjectedGridSize.x / userPreferences.BuildingCellSize);
        userPreferences.ProjectedGridSize.y =
            Mathf.RoundToInt(userPreferences.ProjectedGridSize.y / userPreferences.BuildingCellSize);

        gridData.AdaptiveGridSize = userPreferences.ProjectedGridSize;
        gridData.CellSize = userPreferences.BuildingCellSize;
        gridData.OriginWorldTransform = userPreferences.OriginWorldTransform;
    }
}